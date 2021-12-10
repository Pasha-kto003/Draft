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
    /// Логика взаимодействия для EditCount.xaml
    /// </summary>
    public partial class EditCount : Window
    {
        public EditCount(List<Material> materials)
        {
            InitializeComponent();
            DataContext = new EditCountViewModel(materials);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
