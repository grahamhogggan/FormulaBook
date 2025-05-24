using FormulaBook.Classes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using FormulaBook;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FormulaBook
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FormulaBrowser : Page
    {
        public ObservableCollection<BasicFormula> Formulas { get; set; } =
                                    new ObservableCollection<BasicFormula>();
        public FormulaBrowser()
        {
            InitializeComponent();
            LoadFormulas();
        }
        private async void LoadFormulas()
        {
            Formulas.Clear();
            // Get the folder where the notes are stored.
            StorageFolder storageFolder =
                          ApplicationData.Current.LocalFolder;
            await GetFilesInFolderAsync(storageFolder);
        }
        private async Task GetFilesInFolderAsync(StorageFolder folder)
        {
            // Each StorageItem can be either a folder or a file.
            IReadOnlyList<IStorageItem> storageItems =
                                        await folder.GetItemsAsync();
            foreach (IStorageItem item in storageItems)
            {
                if (item.IsOfType(StorageItemTypes.Folder))
                {
                    // Recursively get items from subfolders.
                    await GetFilesInFolderAsync((StorageFolder)item);
                }
                else if (item.IsOfType(StorageItemTypes.File))
                {
                    StorageFile file = (StorageFile)item;
                    BasicFormula formula = new BasicFormula(
                        file.Name.Substring(0,file.Name.Length-4));
                    await formula.Load();
                    Formulas.Add(formula);
                }
            }
        }
        private void ClickOnFormula(ItemsView sender, ItemsViewItemInvokedEventArgs args)
        {
            Frame.Navigate(typeof(BasicFormulaPage), args.InvokedItem);
        }

        private void NewFormulaButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BasicFormulaPage));
        }
    }
}
