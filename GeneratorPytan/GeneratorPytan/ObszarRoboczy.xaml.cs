using System;
using System.IO;
using System.Collections.Generic;
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
using System.Threading;

namespace GeneratorPytan
{
    public enum TrybPytania { Jednokrotne, Wielokrotne };

    /// <summary>
    /// Interaction logic for ObszarRoboczy.xaml
    /// Obszar odpowiedzialny za obsługę zakładek baz danych.
    /// </summary>
    public partial class ObszarRoboczy : UserControl
    {
        private MainWindow mainWindow;
        private TabItem aktualneOkienko;
        private string szablonKarty;

        public ObszarRoboczy()
        {
            InitializeComponent();

            szablonKarty = XamlWriter.Save(SzablonKarty);
            zakladki.Items.Remove(SzablonKarty);

            mainWindow = MainWindow.mainWindowObject;
        }

        public void DodajZakladke(BazaDanych baza)
        {
            //Wczytanie obiektu
            StringReader stringReader = new StringReader(szablonKarty);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            TabItem nowaZakladka = (TabItem)XamlReader.Load(xmlReader);
            zakladki.Items.Add(nowaZakladka);

            TextBlock nazwaZakladki = nowaZakladka.FindName("nazwaZakladki") as TextBlock;
            
            Binding bindingNazwyZakladki = new Binding("NazwaBazy") { Source = baza.Ustawienia };
            nazwaZakladki.SetBinding(TextBlock.TextProperty, bindingNazwyZakladki);

            //Zaznaczanie bazy danych
            SelectionChangedEventHandler ZmianaSelekcji = (s, args) =>
            {              
                if(args.AddedItems.Count != 0 && args.AddedItems[0] == nowaZakladka)
                {
                    mainWindow.OznaczBaze(baza);
                    aktualneOkienko = nowaZakladka;
                }
            };

            zakladki.SelectionChanged += ZmianaSelekcji;

            (nowaZakladka.FindName("przyciskZamkijKarte") as Button).Click += (s, args) =>
            {
                if (baza.CzyModyfikowana)
                {
                    MessageBoxResult dialog = MessageBox.Show($"Baza {baza.Ustawienia.NazwaBazy} została zmodyfikowana. Czy chcesz zapisać bazę przed zamknięciem?",
                    "Zamknij kartę", MessageBoxButton.YesNoCancel);

                    if (dialog == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    else if (dialog == MessageBoxResult.Yes)
                    {
                        mainWindow.PrzygotowanieDoZapisu(baza);
                    }
                }

                if (mainWindow.AktualnieWybranaBaza == baza)                
                    aktualneOkienko = null;
                
                mainWindow.UsunBaze(baza);

                zakladki.SelectionChanged -= ZmianaSelekcji;
                zakladki.Items.Remove(nowaZakladka);      
            };

            nowaZakladka.Content = new ObszarZakladki(baza);
        }

        public ObszarZakladki ZwrocOtwartaZakladke()
        {
            return aktualneOkienko.Content as ObszarZakladki;
        }
    }
}
