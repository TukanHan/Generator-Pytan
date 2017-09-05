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

namespace GeneratorPytan
{
    /// <summary>
    /// Interaction logic for OProgramie.xaml
    /// Okienko z informacjami o programie od strony twórczej
    /// </summary>
    public partial class OknoOProgramie : Window
    {
        public OknoOProgramie()
        {
            InitializeComponent();
        }

        private void Hiperlacze(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://" + ((Button)sender).Tag);
        }
    }
}
