using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            StorageFile saveFile = (StorageFile)await folder.TryGetItemAsync(CleanFilePath(name) + ".txt");
            if (saveFile is null)
            {
                saveFile = await folder.CreateFileAsync(CleanFilePath(name) + ".txt", CreationCollisionOption.ReplaceExisting);
            }
            await FileIO.WriteTextAsync(saveFile, formula);
        }
        public async Task DeleteAsync()
        {
            StorageFile saveFile = (StorageFile)await folder.TryGetItemAsync(CleanFilePath(name) + ".txt");
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
        public string[] GetLeftElements(bool removeDups = true)
        {
            if (!isValid()) return new string[0];
            if (IsSimple())
            {
                if (removeDups)
                    return RemoveDups(Compressed().Split("=")[0].Split("*"));
                else
                    return Compressed().Split("=")[0].Split("*");
            }


            return RemoveDups(ExpressionSplit(Compressed().Split("=")[0]));



        }
        public string[] GetRightElements(bool removeDups = true)
        {
            if (!isValid()) return new string[0];
            if (IsSimple())
            {
                if (removeDups)
                    return RemoveDups(Compressed().Split("=")[1].Split("*"));
                else
                    return Compressed().Split("=")[1].Split("*");
            }
            return RemoveDups(ExpressionSplit(Compressed().Split("=")[1]));

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
            foreach (string str in input)
            {
                if (!strings.Contains(str))
                {
                    if (!double.TryParse(str, out _))
                    {
                        strings.Add(str);
                    }
                }
            }
                return strings.ToArray();  
            
        }
        public bool ContainsElement(string element)
        {
            return GetElements().Contains(element);
        }
        public double Solve(Dictionary<String,double> elements,string ElementToSolveFor)
        {
            if(IsSimple())
            {
                int countOfSolve = 0;
                double RunningProduct = 1;
                foreach (String element in GetLeftElements(false))
                {
                    if (element == ElementToSolveFor)
                    {
                        countOfSolve++;
                    }
                    else
                    {
                        double constant;
                        if (double.TryParse(element, out constant))
                        {
                            if (constant != 0)
                                RunningProduct /= constant;
                        }
                        else
                        {
                            if (elements[element] == 0) return 0;
                            RunningProduct /= elements[element];
                        }

                    }
                }
                foreach (String element in GetRightElements(false))
                {
                    if (element == ElementToSolveFor)
                    {
                        countOfSolve--;
                    }
                    else
                    {
                        double constant;
                        if (double.TryParse(element, out constant))
                        {
                            RunningProduct *= constant;
                        }
                        else
                        {
                            RunningProduct *= elements[element];
                        }
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
            else
            {
                return ComplexSolve(elements, ElementToSolveFor);
            }
           
        }
        public static double Round3(double value)
        {
            return Math.Round(value * 1000) / 1000;
        }
        private static string CleanFilePath(string path)
        {
            path = path.Replace("/", "_");
            path = path.Replace("\\", "_");
            path = path.Replace("<", "_");
            path = path.Replace(">", "_");
            path = path.Replace("?", "_");

            path = path.Replace("!", "-");
            path = path.Replace("@", "-");
            path = path.Replace("#", "-");
            path = path.Replace("$", "-");
            path = path.Replace("%", "-");
            path = path.Replace("^", "-");
            path = path.Replace("&", "-");
            path = path.Replace("*", "-");
            path = path.Replace("(", "-");
            path = path.Replace(")", "-");


            return path;

        }
        private bool IsSimple()
        {
            if (formula.Contains('/')) return false;
            if (formula.Contains('+')) return false;
            if (formula.Contains('-')) return false;
            if (formula.Contains('^')) return false;
            return true;
        }
        private string[] ExpressionSplit(string expression)
        {
            List<string> elements = new List<string>();
            string[] multSplit = expression.Split("*");
            foreach (string s in multSplit)
            {
                string[] plusSplit = s.Split("+");
                foreach (string s2 in plusSplit)
                {
                    string[] minusSplit = s2.Split("-");
                    foreach (string s3 in minusSplit)
                    {
                        string[] divSplit = s3.Split("/");
                        foreach (string s4 in divSplit)
                        {
                            string[] expSplit = s4.Split("^");
                            foreach (string s5 in expSplit)
                            {
                                string[] openSplit = s5.Split("(");
                                foreach (string s6 in openSplit)
                                {
                                    string[] closeSplit = s6.Split(")");
                                    foreach (string s7 in closeSplit)
                                    {
                                        if (s7.Length > 0) 
                                        elements.Add(s7);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return elements.ToArray();
        }
        private double ComplexSolve(Dictionary<String, double> elements, string ElementToSolveFor)
        {
            Debug.Print("->" + formula);
            string equation = Compressed();
            foreach(string element in GetElements())
            {
                if(double.TryParse(element, out _))
                {
                    
                }
                else if(element != ElementToSolveFor)
                {
                    equation = equation.Replace(element, elements[element].ToString());
                }
            }
            return Equation.SolveEquation(equation, ElementToSolveFor);
        }
    }
}
