using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace GeneratorPytan
{
    /// <summary>
    /// Interaction logic for PasekStanu.xaml
    /// Obszar odpowiedzialny za skromne informacje na pasku stanu.
    /// </summary>
    public partial class PasekStanu : UserControl
    {
        private MainWindow mainWindow;
        private ProgressBar progressBar;

        public PasekStanu()
        {
            InitializeComponent();
            mainWindow = MainWindow.mainWindowObject;
        }

        public void OznaczonoBaze(BazaDanych bazaDanych)
        {
            stackInformacji.Children.Clear();
            stackStanu.Children.Clear();

            stackInformacji.Children.Add(new TextBlock() { Text = "Baza: " });

            TextBlock nazwaBazy = new TextBlock();
            stackInformacji.Children.Add(nazwaBazy);
            Binding bindingNazwyBazy = new Binding("NazwaBazy") { Source = bazaDanych.Ustawienia };
            nazwaBazy.SetBinding(TextBlock.TextProperty, bindingNazwyBazy);

            stackInformacji.Children.Add(new TextBlock() { Text = "  Liczba pytań: " });

            TextBlock liczbaPytan = new TextBlock();
            stackInformacji.Children.Add(liczbaPytan);
            Binding bindingLiczbyPytan = new Binding("PytaniaCount") { Source = bazaDanych, Mode = BindingMode.OneWay };
            liczbaPytan.SetBinding(TextBlock.TextProperty, bindingLiczbyPytan);

            TextBlock zmiany = new TextBlock();
            Binding bindingModyfikacji = new Binding("CzyModyfikowana") { Source = bazaDanych, Mode = BindingMode.OneWay, Converter = new BoolToZmianaConventer() };
            zmiany.SetBinding(TextBlock.TextProperty, bindingModyfikacji);

            stackStanu.Children.Add(zmiany);

        }

        public void OdznaczonoBaze()
        {
            stackInformacji.Children.Clear();
            stackStanu.Children.Clear();

            stackInformacji.Children.Add(new TextBlock() { Text = "Generator Pytań by TukanHan"});
            stackStanu.Children.Add(new TextBlock() { Text = "Gotów do działania" });
        }

        public void DzialanieWTle(double postep, string tekst)
        {
            if(postep==0)
            {
                stackStanu.Children.Clear();
                stackStanu.Children.Add(new TextBlock() { Text = tekst });

                stackStanu.Children.Add(progressBar = new ProgressBar() { Width = 100 });
            }
            else if(postep==-1)
            {
                progressBar = null;

                if (mainWindow.AktualnieWybranaBaza != null)
                    OznaczonoBaze(mainWindow.AktualnieWybranaBaza);
                else
                    OdznaczonoBaze();                      
            }
            else
            {
                progressBar.Value = 100 * postep;
            }
        }
    }

    public class BoolToZmianaConventer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Modyfikowana" : "Zapisana";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
