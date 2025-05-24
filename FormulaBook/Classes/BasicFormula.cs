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
            StorageFile saveFile = (StorageFile)await folder.TryGetItemAsync(name+".txt");
            if (saveFile is null)
            {
                saveFile = await folder.CreateFileAsync(name+".txt", CreationCollisionOption.ReplaceExisting);
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
    }
}
