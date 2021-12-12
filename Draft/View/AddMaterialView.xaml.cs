using Draft.ViewModels;
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
using System.Windows.Shapes;

namespace Draft.View
{
    /// <summary>
    /// Логика взаимодействия для AddMaterial.xaml
    /// </summary>
    public partial class AddMaterialView : Window
    {
        public AddMaterialView()
        {
            InitializeComponent();
            DataContext = new AddMaterial(null);
        }
        public AddMaterialView(Material material)
        {
            InitializeComponent();
            DataContext = new AddMaterial(material);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

    }
}
