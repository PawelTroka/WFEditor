using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using Microsoft.Win32;
namespace WFEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WorkflowDesigner _designer;
        public MainWindow()
        {
            InitializeComponent();
            RegisterMetadata();
            InitializeDesigner();
            AddToolBox();
        }

        private void AddDesigner()
        {
            _designer = new WorkflowDesigner();
            Grid.SetColumn(_designer.View, 1);
            Grid.SetRow(_designer.View, 1);
            grid1.Children.Add(_designer.View);
        }

        private void RegisterMetadata()
        {
            var dm = new DesignerMetadata();
            dm.Register();
        }

        private ToolboxControl GetToolboxControl()
        {
            var toolbox = new ToolboxControl();
            PopulateToolboxCategoryFromAssembly(toolbox, typeof(Assign).Assembly, "Standard");
            return toolbox;
        }

        private void PopulateToolboxCategoryFromAssembly(ToolboxControl toolbox, Assembly assembly, string categoryName)
        {
            var tools = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Activity))
                    && t.IsPublic
                    && !t.IsAbstract
                    && HasParameterlessContructor(t))
                .Select(t => new ToolboxItemWrapper(t.FullName, t.Assembly.FullName, null, t.Name))
                .OrderBy(t => t.DisplayName);

            var category = new ToolboxCategory(categoryName);
            foreach (var t in tools)
            {
                category.Add(t);
            }
            toolbox.Categories.Add(category);
        }

        private bool HasParameterlessContructor(Type t)
        {
            var ctors = t.GetConstructors();
            var parameterless = ctors.Where(c => c.GetParameters().Count() == 0)
                .FirstOrDefault();
            return parameterless != null;
        }

        private void AddToolBox()
        {
            var tc = GetToolboxControl();
            Grid.SetColumn(tc, 0);
            Grid.SetRow(tc, 1);
            grid1.Children.Add(tc);
        }

        private void AddPropertyInspector()
        {
            Grid.SetColumn(_designer.PropertyInspectorView, 2);
            Grid.SetRow(_designer.PropertyInspectorView, 1);
            grid1.Children.Add(_designer.PropertyInspectorView);
        }

        private void OnOpenClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog().Value)
            {
                InitializeDesigner();
                _designer.Load(dlg.FileName);
            }
        }

        private void OnSaveClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            if (dlg.ShowDialog().Value)
            {
                _designer.Save(dlg.FileName);
            }
        }

        private void InitializeDesigner()
        {
            AddDesigner();
            AddPropertyInspector();
        }
    }
}
