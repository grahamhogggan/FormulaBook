using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static System.Net.Mime.MediaTypeNames;

namespace FormulaBook.Classes
{
    public class BasicFormula
    {
        public string name;
        public string formula;
        private StorageFolder folder=ApplicationData.Current.LocalFolder;
        public BasicFormula(string name)
        {
            this.name = name;
            formula = "not loaded";
        }
        public async Task Load()
        {
            StorageFile file = (StorageFile)await folder.TryGetItemAsync(name+".txt");
            if (file is not null)
            {
                formula = await FileIO.ReadTextAsync(file);
            }
        }
        public async Task SaveAsync()
        {
            StorageFile saveFile = (StorageFile)await folder.TryGetItemAsync(name.Replace("/", "_") + ".txt");
            if (saveFile is null)
            {
                saveFile = await folder.CreateFileAsync(name.Replace("/","_")+".txt", CreationCollisionOption.ReplaceExisting);
            }
            await FileIO.WriteTextAsync(saveFile, formula);
        }
        public async Task DeleteAsync()
        {
            StorageFile saveFile = (StorageFile)await folder.TryGetItemAsync(name + ".txt");
            if (saveFile is not null)
            {
                await saveFile.DeleteAsync();
            }
        }
        public string GetFormula()
            { return formula; }
        public void SetFormula(string formula)
        {
            this.formula = formula;
        }
        public string ValidityRemark()
        {
            string compressed = Compressed();
            if (!compressed.Contains("=")) return "Invalid formula, must have an equals sign";
            if (compressed.Contains("/")) return "Invalid formula, divison must be removed by rearranging";

            return "";
        }
        public bool isValid()
        {
            string compressed = Compressed();
            if (!compressed.Contains("=")) return false;
            if (compressed.Contains("/")) return true;

            return true;
        }
        public string[] GetLeftElements()
        {
            if (!isValid()) return new string[0];
            return RemoveDups(Compressed().Split("=")[0].Split("*"));
        }
        public string[] GetRightElements()
        {
            if (!isValid()) return new string[0];
            return RemoveDups(Compressed().Split("=")[1].Split("*"));
        }
        public string[] GetElements()
        {
            string[] l = GetLeftElements();
            string[] r = GetRightElements();
            string[] joined = l.Concat(r).ToArray();
            return RemoveDups(joined);
        }
        string Compressed()
        {
            return formula.Replace(" ", "");
        }
        string[] RemoveDups(string[] input)
        {
            List<string> strings = new List<string>();
            foreach(string str in input)
            {
                if(!strings.Contains(str)) strings.Add(str);
            }
            return strings.ToArray();  

        }
        public bool ContainsElement(string element)
        {
            return GetElements().Contains(element);
        }
        public double Solve(Dictionary<String,double> elements,string ElementToSolveFor)
        {
            int countOfSolve = 0;
            double RunningProduct = 1;
            foreach(String element in Compressed().Split("=")[0].Split("*"))
            {
                if(element==ElementToSolveFor)
                {
                    countOfSolve++;
                }
                else
                {
                    if (elements[element] == 0) return 0;
                    RunningProduct /= elements[element];
                }
            }
            foreach (String element in Compressed().Split("=")[1].Split("*"))
            {
                if (element == ElementToSolveFor)
                {
                    countOfSolve--;
                }
                else
                {
                    RunningProduct *= elements[element];
                }
            }
            if (countOfSolve == 0)
            {
                return 0;
            }
            else
            {
                return Round3(Math.Pow(RunningProduct, 1.0 / countOfSolve));
            }
        }
        public static double Round3(double value)
        {
            return Math.Round(value * 1000) / 1000;
        }
    }
}
