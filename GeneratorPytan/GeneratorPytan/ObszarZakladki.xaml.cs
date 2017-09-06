using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace GeneratorPytan
{
    /// <summary>
    /// Interaction logic for ObszarZakladki.xaml
    /// Miejsce odpowiedzialne za pobieranie danych z pytań i zapisywanie ich w klasach BazaDanych
    /// oraz całe procesy wizualne jak dodawanie odpowiedzi, otwieranie okien pytań / ustawień.
    /// </summary>
    public partial class ObszarZakladki : UserControl
    {
        public Pytanie AktualnePytanie { get; private set; }

        private string szablonOdpowiedzi;
        private MainWindow mainWindow;

        public ObszarZakladki(BazaDanych bazaDanych)
        {
            InitializeComponent();

            mainWindow = MainWindow.mainWindowObject;
            szablonOdpowiedzi = XamlWriter.Save(SzablonOdpowiedzi);
            listaUchwyt.Items.Remove(SzablonOdpowiedzi);

            Binding bindingNazwyBazy = new Binding("NazwaBazy") { Source = bazaDanych.Ustawienia, Mode = BindingMode.TwoWay };
            blokNazwaBazy.SetBinding(TextBox.TextProperty, bindingNazwyBazy);

            Binding bindingAutoraBazy = new Binding("AutorBazy") { Source = bazaDanych.Ustawienia, Mode = BindingMode.TwoWay };
            blokAutorBazy.SetBinding(TextBox.TextProperty, bindingAutoraBazy);

            Binding bindingOpisBazy = new Binding("OpisBazy") { Source = bazaDanych.Ustawienia, Mode = BindingMode.TwoWay };
            blokOpisBazy.SetBinding(TextBox.TextProperty, bindingOpisBazy);

            Binding bindingCzasEgzaminu = new Binding("CzasEgzaminu") { Source = bazaDanych.Ustawienia, Mode = BindingMode.TwoWay };
            blokCzasEgzaminu.SetBinding(TextBox.TextProperty, bindingCzasEgzaminu);

            Binding bindingLiczbaPytan = new Binding("LiczbaPytan") { Source = bazaDanych.Ustawienia, Mode = BindingMode.TwoWay };
            blokLiczbaPytanEgzaminacyjnych.SetBinding(TextBox.TextProperty, bindingLiczbaPytan);
        }

        public void OtworzPytania(Pytanie pytanie)
        {
            AktualnePytanie = pytanie;
            mainWindow.pasekMenu.OznaczonoPytanie();

            if (warstwaPytania.Visibility == Visibility.Hidden)
            {
                warstwaPytania.Visibility = Visibility.Visible;
                warstwaUstawien.Visibility = Visibility.Hidden;
            }

            if (pytanie.trybPytania == TrybPytania.Jednokrotne)
                typJednokrotnego.IsChecked = true;
            else
                typWielkokrotnego.IsChecked = true;

            int obecnyIndex = mainWindow.AktualnieWybranaBaza.pytania.FindIndex((pytanko) => pytanko == AktualnePytanie);
            przyciskPoprzedni.IsEnabled = obecnyIndex != 0;
            mainWindow.pasekMenu.WidocznoscPrzyciskPoprzedni(obecnyIndex != 0);

            BindingOperations.ClearBinding(trescPytania, TextBox.TextProperty);
            Binding bindingPytanie = new Binding("pytanie") { Source = pytanie, Mode = BindingMode.TwoWay };
            trescPytania.SetBinding(TextBox.TextProperty, bindingPytanie);

            OdswierzOdpowiedzi(pytanie.odpowiedzi);
        }

        public void OtworzUstawienia()
        {
            if (warstwaUstawien.Visibility == Visibility.Hidden)
            {
                warstwaUstawien.Visibility = Visibility.Visible;
                warstwaPytania.Visibility = Visibility.Hidden;
            }

            mainWindow.pasekMenu.OdznaczonoPytanie();
        }

        public void DodajOdpowiedz(Odpowiedz odpowiedz)
        {
            StringReader stringReader = new StringReader(szablonOdpowiedzi);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Grid nowaOdpowiedz = (Grid)XamlReader.Load(xmlReader);
            listaUchwyt.Items.Add(nowaOdpowiedz);

            //przesówam przycisk Dodaj Odpowiedź na koniec
            Grid temp = listaUchwyt.Items[listaUchwyt.Items.Count - 2] as Grid;
            listaUchwyt.Items.Remove(temp);
            listaUchwyt.Items.Add(temp);

            Binding bindingOdpowiedz = new Binding("tresc") { Source = odpowiedz, Mode = BindingMode.TwoWay };
            (nowaOdpowiedz.FindName("trescOdpowiedzi") as TextBox).SetBinding(TextBox.TextProperty, bindingOdpowiedz);

            //fuszera (ale próbowałem z radiobutton / checkbox z bildingiem i nic nie działało)
            if (AktualnePytanie.trybPytania == TrybPytania.Jednokrotne)
            {
                (nowaOdpowiedz.FindName("checkBoxOdpowiedzi") as CheckBox).IsChecked = odpowiedz.czyPoprawna;
                (nowaOdpowiedz.FindName("checkBoxOdpowiedzi") as CheckBox).Checked += (s, args) =>
                {
                    for (int i = 0; i < listaUchwyt.Items.Count - 1; ++i)
                    {
                        CheckBox cbx = (listaUchwyt.Items[i] as Grid).FindName("checkBoxOdpowiedzi") as CheckBox;
                        if (cbx == s)
                            AktualnePytanie.odpowiedzi[i].czyPoprawna = true;
                        else
                            cbx.IsChecked = AktualnePytanie.odpowiedzi[i].czyPoprawna = false;
                    }
                };
                (nowaOdpowiedz.FindName("checkBoxOdpowiedzi") as CheckBox).Unchecked += (s, args) =>
                {
                    for (int i = 0; i < listaUchwyt.Items.Count - 1; ++i)
                    {
                        if ((listaUchwyt.Items[i] as Grid).FindName("checkBoxOdpowiedzi") == s)
                        {
                            AktualnePytanie.odpowiedzi[i].czyPoprawna = false;
                            break;
                        }
                    }
                };
            }
            else
            {
                Binding bindingPoprawnaOdpowiedz = new Binding("czyPoprawna") { Source = odpowiedz, Mode = BindingMode.TwoWay };
                (nowaOdpowiedz.FindName("checkBoxOdpowiedzi") as CheckBox).SetBinding(CheckBox.IsCheckedProperty, bindingPoprawnaOdpowiedz);
            }

            (nowaOdpowiedz.FindName("przyciskUsunOdpowiedz") as Button).Click += (s, args) =>
            {
                listaUchwyt.Items.Remove(nowaOdpowiedz);
                AktualnePytanie.UsunOdpowiedz(odpowiedz);

                mainWindow.obszarPomocniczy.UsunOdpowiedz(odpowiedz);
            };
        }

        private void przyciskDodajOdpowiedz_Click(object sender, RoutedEventArgs e)
        {
            Odpowiedz nowaOdpowiedz = new Odpowiedz(AktualnePytanie);
            AktualnePytanie.DodajOdpowiedz(nowaOdpowiedz);

            mainWindow.obszarPomocniczy.DodajOdpowiedz(nowaOdpowiedz);
            DodajOdpowiedz(nowaOdpowiedz);
        }

        private void przyciskPoprzedni_Click(object sender, RoutedEventArgs e)
        {
            int obecnyIndex = mainWindow.AktualnieWybranaBaza.pytania.FindIndex((pytanko) => pytanko == AktualnePytanie);
            OtworzPytania(mainWindow.AktualnieWybranaBaza.pytania[obecnyIndex - 1]);
        }

        private void przyciskNastepny_Click(object sender, RoutedEventArgs e)
        {
            int obecnyIndex = mainWindow.AktualnieWybranaBaza.pytania.FindIndex((pytanko) => pytanko == AktualnePytanie);
            if (mainWindow.AktualnieWybranaBaza.pytania.Count == obecnyIndex + 1)
                mainWindow.DodajPytanie(new Pytanie(mainWindow.AktualnieWybranaBaza, ZwrocTrybPytania()));
            else
                OtworzPytania(mainWindow.AktualnieWybranaBaza.pytania[obecnyIndex + 1]);
        }

        private void przyciskUsun_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.UsunPytanie(AktualnePytanie);
        }

        private void typJednokrotnego_Checked(object sender, RoutedEventArgs e)
        {
            AktualnePytanie.UstawTrybPojedynczy();
            OdswierzOdpowiedzi(AktualnePytanie.odpowiedzi);
        }

        private void typWielkokrotnego_Checked(object sender, RoutedEventArgs e)
        {
            AktualnePytanie.UstawTrybWielokrotny();
            OdswierzOdpowiedzi(AktualnePytanie.odpowiedzi);
        }

        public TrybPytania ZwrocTrybPytania()
        {
            return typJednokrotnego.IsChecked == true ? TrybPytania.Jednokrotne : TrybPytania.Wielokrotne;
        }

        private void OdswierzOdpowiedzi(List<Odpowiedz> odpowiedzi)
        {
            var doWywalenia = listaUchwyt.Items.OfType<Grid>().Where<Grid>((obj) => obj.Name.Equals("SzablonOdpowiedzi")).ToArray();

            foreach (var grid in doWywalenia)
            {
                listaUchwyt.Items.Remove(grid);
            }

            foreach (Odpowiedz odpowiedz in odpowiedzi)
            {
                DodajOdpowiedz(odpowiedz);
            }
        }
    }
}
