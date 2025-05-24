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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FormulaBook
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class BasicFormulaPage : Page
    {
        private BasicFormula? formula;
        private StorageFolder folder=ApplicationData.Current.LocalFolder;
        public BasicFormulaPage()
        {
            InitializeComponent();
            formula = new BasicFormula("TestFormula");
            Loaded += LoadFormula;
        }
        private async void LoadFormula(object sender, RoutedEventArgs e)
        {
            if(formula is not null)
            {
                await formula.Load();
                FormulaEditor.Text = formula.GetFormula();
                NameInput.Text = formula.name;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is BasicFormula form)
            {
                formula = form;
            }
            else
            {
                formula = new BasicFormula("untitled Formula");
            }
        }
        public async Task SaveAsync()
        {
            if(formula is not null)
            {
                formula.name = NameInput.Text;
                formula.SetFormula(FormulaEditor.Text);
                await formula.SaveAsync();
            }
        }
        public async Task DeleteAsync()
        {
            if (formula is not null)
            {

                await formula.DeleteAsync();
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await SaveAsync();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            await DeleteAsync();
            if (Frame.CanGoBack == true)
            {
                Frame.GoBack();
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack == true)
            {
                Frame.GoBack();
            }
        }
    }
}
