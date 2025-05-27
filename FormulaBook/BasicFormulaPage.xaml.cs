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
using Windows.System;

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
        private List<StackPanel> ElementEditors;
        private CheckBox? currentlySolvingFor;
        public BasicFormulaPage()
        {
            InitializeComponent();
            formula = new BasicFormula("TestFormula");
            Loaded += LoadFormula;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            ElementEditors = new List<StackPanel>();
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
        private void CompositionTarget_Rendering(object sender, object e)
        {
            if(formula is not null)
            {
                ValidityRemark.Text = formula.ValidityRemark();
                foreach(string element in formula.GetElements())
                {
                   if(!EEContains(element))
                   {
                        StackPanel newPanel = new StackPanel();
                        newPanel.Name = "Element_Editor_" + element;
                        newPanel.Orientation= Orientation.Horizontal;
                        ElementEditorStack.Children.Add(newPanel);
                        ElementEditors.Add(newPanel);
                        TextBlock block1 = new TextBlock();
                        block1.TextAlignment = TextAlignment.Center;
                        block1.HorizontalAlignment = HorizontalAlignment.Center;
                        block1.Text = "Solve? ";
                        block1.Padding = new Thickness(10);
                        newPanel.Children.Add(block1);
                        CheckBox box = new CheckBox();
                        box.Padding = new Thickness(10);
                        box.MinWidth = 0;
                        box.HorizontalAlignment= HorizontalAlignment.Right;
                        box.Checked += SolveToggleOn;
                        newPanel.Children.Add(box);
                        TextBlock block2 = new TextBlock();
                        block2.TextAlignment = TextAlignment.Center;
                        block2.HorizontalAlignment = HorizontalAlignment.Center;
                        block2.Text = element+" =   ";
                        block2.Padding = new Thickness(10);
                        newPanel.Children.Add(block2);
                        TextBox tbox = new TextBox();
                        tbox.Text = "0";
                        tbox.Padding = new Thickness(10);
                        newPanel.Children.Add(tbox);
                   }
                }
                Dictionary<string, double> elementsDictionary = new Dictionary<string, double>();
                foreach (StackPanel row in ElementEditorStack.Children)
                {
                    if(row is not null)
                    {
                        string name = row.Name.Substring(15);
                        if (!formula.ContainsElement(name))
                        {
                            ElementEditorStack.Children.Remove(row);
                        }
                        if(!elementsDictionary.ContainsKey(name))
                        {
                            elementsDictionary.Add(name, GetTextBoxVal(row));
                        }
                        else
                        {
                            elementsDictionary[name] = GetTextBoxVal(row);
                        }
                    }
                }
                if(currentlySolvingFor !=null && currentlySolvingFor.IsChecked== true)
                {
                    StackPanel solveRow = (StackPanel)currentlySolvingFor.Parent;
                    TextBox solvebox = GetTextBox(solveRow);
                    if(solvebox is not null)
                    {
                        double result = formula.Solve(elementsDictionary, solveRow.Name.Substring(15));
                        string newText = result.ToString();
                        if (result > 100000)
                        {
                            newText = ScientificNotate(result);
                        }
                        solvebox.Text = newText;
                    }

                }

                
            }

        }

        private void Box_Checked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private bool EEContains(string element)
        {
            foreach (StackPanel panel in ElementEditors)
            {
                if(panel.Name=="Element_Editor_"+element)
                {
                    return true;
                }
            }
            return false;
        }
        private void SolveToggleOn(object sender, RoutedEventArgs e)
        {
            if(currentlySolvingFor==null)
            {
                currentlySolvingFor = (CheckBox)sender;
            }
            else
            {
                if(currentlySolvingFor!=(CheckBox)sender) currentlySolvingFor.IsChecked = false;
                currentlySolvingFor = (CheckBox)sender;
                //currentlySolvingFor.IsChecked = true;
            }
        }
        private double GetTextBoxVal(StackPanel row)
        {
            TextBox box = GetTextBox(row);
            if(box!=null)
            {
                try
                {
                    return Double.Parse(box.Text);
                }
                catch
                {
                    return 0;
                }
            }
            return -1;
        }
        private TextBox GetTextBox(StackPanel row)
        {
            if (row == null) return null;
            foreach(UIElement element in row.Children)
            {
                if(element.GetType()==typeof(TextBox))
                {
                    return (TextBox)element;
                }
            }
            return null;
        }
        private string ScientificNotate(double val)
        {
            int exp = (int)Math.Log10(val);
            double co = BasicFormula.Round3(val/Math.Pow(10,exp));
            return co + "E" + exp;
        }

    }
}
