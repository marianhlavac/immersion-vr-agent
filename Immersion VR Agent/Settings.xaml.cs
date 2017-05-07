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

namespace Immersion_VR_Agent {
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>


    public partial class Settings : Window {

        public Settings() {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".exe";
            dlg.Filter = "Executable (*.exe)|*.exe";
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true) {
                path.Text = dlg.FileName;
            }
        }
    }
}
