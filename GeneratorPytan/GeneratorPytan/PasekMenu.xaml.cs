using Microsoft.Win32;
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


namespace GeneratorPytan
{
    /// <summary>
    /// Interaction logic for PasekMenu.xaml
    /// Plik zawiera w głównej mierze odwołania do ważniejszych miejsc w kodzie przez eventy z paska menu
    /// Pasek menu podzielony jest na sekcje Plik / Baza / Pytanie / Pomoc
    /// </summary>
    public partial class PasekMenu : UserControl
    {
        private MainWindow mainWindow;

        public PasekMenu()
        {
            InitializeComponent();
            mainWindow = MainWindow.mainWindowObject;
        }

        #region Plik
        private void menuNowaBaza_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.DodajBaze(new BazaDanych());
        }

        private void menuWczytajBaze_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.PrzygotiwanieDoWczytania();
        }

        private void menuWyjdz_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #region Baza
        public void OznaczonoBaze()
        {
            menuBazaDanych.IsEnabled = true;
        }

        public void OdznaczonoBaze()
        {
            menuBazaDanych.IsEnabled = false;
            OdznaczonoPytanie();
        }

        private void menuPytania_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.AktualnieWybranaBaza.pytania.Count == 0)
                mainWindow.DodajPytanie(new Pytanie(mainWindow.AktualnieWybranaBaza,mainWindow.obszarRoboczy.ZwrocOtwartaZakladke().ZwrocTrybPytania()));
            else
                mainWindow.obszarRoboczy.ZwrocOtwartaZakladke().OtworzPytania(mainWindow.AktualnieWybranaBaza.pytania[0]);
        }

        private void menuUstawieniaBazy_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.obszarRoboczy.ZwrocOtwartaZakladke().OtworzUstawienia();
        }

        private void menuZapiszBaze_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.PrzygotowanieDoZapisu(mainWindow.AktualnieWybranaBaza);
        }

        private void menuEksportujBaze_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.PrzygotowanieDoEksportu(mainWindow.AktualnieWybranaBaza);
        }

        private void menuEksportujPytania_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.PrzygotowanieDoEksportuPytan(mainWindow.AktualnieWybranaBaza);
        }

        private void menuImportujPytania_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.PrzygotowanieDoImportuPytan();
        }
        #endregion

        #region Pytanie
        public void OznaczonoPytanie()
        {
            menuPytanie.IsEnabled = true;
        }

        public void OdznaczonoPytanie()
        {
            menuPytanie.IsEnabled = false;
        }

        public void WidocznoscPrzyciskPoprzedni(bool widocznosc)
        {
            menuPoprzedniePytanie.IsEnabled = widocznosc;
        }

        private void menuPoprzedniePytanie_Click(object sender, RoutedEventArgs e)
        {
            int obecnyIndex = mainWindow.AktualnieWybranaBaza.pytania.FindIndex((pytanko) => pytanko == mainWindow.obszarRoboczy.ZwrocOtwartaZakladke().AktualnePytanie);
            mainWindow.obszarRoboczy.ZwrocOtwartaZakladke().OtworzPytania(mainWindow.AktualnieWybranaBaza.pytania[obecnyIndex - 1]);
        }

        private void menuNastepnePytanie_Click(object sender, RoutedEventArgs e)
        {
            int obecnyIndex = mainWindow.AktualnieWybranaBaza.pytania.FindIndex((pytanko) => pytanko == mainWindow.obszarRoboczy.ZwrocOtwartaZakladke().AktualnePytanie);
            if (mainWindow.AktualnieWybranaBaza.pytania.Count == obecnyIndex + 1)
                mainWindow.DodajPytanie(new Pytanie(mainWindow.AktualnieWybranaBaza,mainWindow.obszarRoboczy.ZwrocOtwartaZakladke().ZwrocTrybPytania()));
            else
                mainWindow.obszarRoboczy.ZwrocOtwartaZakladke().OtworzPytania(mainWindow.AktualnieWybranaBaza.pytania[obecnyIndex + 1]);
        }

        private void menuUsun_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.UsunPytanie(mainWindow.obszarRoboczy.ZwrocOtwartaZakladke().AktualnePytanie);
        }
        #endregion

        #region Pomoc
        private void menuOProgramie_Click(object sender, RoutedEventArgs e)
        {
            Window okienko = new OknoOProgramie();
            okienko.Show();
        }

        private void menuPomoc_Click(object sender, RoutedEventArgs e)
        {
            Window okienko = new OknoPomoc();
            okienko.Show();
        }
        #endregion
    }
}
