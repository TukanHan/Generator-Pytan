using System;
using System.Collections;
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
    /// Interaction logic for ObszarPomocniczy.xaml
    /// Obszar odpowiedzialny za budowanie drzewa nawigacji
    /// aktualizowanie danych i interakcję.
    /// </summary>
    public partial class ObszarPomocniczy : UserControl
    {
        private MainWindow mainWindow;

        TreeViewItem korzen;
        Dictionary<Pytanie, TreeViewItem> powiazaniePytanieDrzewo;
        Dictionary<Odpowiedz, TreeViewItem> powiazanieOdpowiedzDrzewo;

        public ObszarPomocniczy()
        {
            InitializeComponent();
            powiazaniePytanieDrzewo = new Dictionary<Pytanie, TreeViewItem>();
            powiazanieOdpowiedzDrzewo = new Dictionary<Odpowiedz, TreeViewItem>();
            mainWindow = MainWindow.mainWindowObject;
        }

        public void DodajDrzewo(BazaDanych bazaDanych)
        {
            UsunDrzewo();

            korzen = new TreeViewItem() { IsExpanded = true };

            Binding bindingNazwyBazy = new Binding("NazwaBazy") { Source = bazaDanych.Ustawienia };
            korzen.SetBinding(TreeViewItem.HeaderProperty, bindingNazwyBazy);

            korzen.Selected += (s, args) =>
            {                
                mainWindow.obszarRoboczy.ZwrocOtwartaZakladke().OtworzUstawienia();
            };

            for (int i = 0; i < bazaDanych.pytania.Count; ++i)
            {
                DodajPytanie(bazaDanych.pytania[i]);
                for (int j = 0; j < bazaDanych.pytania[i].odpowiedzi.Count; ++j)
                {
                    DodajOdpowiedz(bazaDanych.pytania[i].odpowiedzi[j]);
                }
            }

            drzewo.Items.Add(korzen);
        }

        public void DodajPytanie(Pytanie pytanie)
        {
            powiazaniePytanieDrzewo.Add(pytanie, new TreeViewItem() { IsExpanded = true });

            Binding bindingNazwyPytania = new Binding("pytanie") { Source = pytanie };
            powiazaniePytanieDrzewo[pytanie].SetBinding(TreeViewItem.HeaderProperty, bindingNazwyPytania);

            powiazaniePytanieDrzewo[pytanie].Selected += (s, args) =>
            {
                mainWindow.obszarRoboczy.ZwrocOtwartaZakladke().OtworzPytania(pytanie);

                args.Handled = true;
            };

            korzen.Items.Add(powiazaniePytanieDrzewo[pytanie]);
        }

        public void DodajOdpowiedz(Odpowiedz odpowiedz)
        {
            powiazanieOdpowiedzDrzewo.Add(odpowiedz, new TreeViewItem());       

            Binding bindingNazwyOdpowiedzi = new Binding("tresc") { Source = odpowiedz };
            powiazanieOdpowiedzDrzewo[odpowiedz].SetBinding(TreeViewItem.HeaderProperty, bindingNazwyOdpowiedzi);

            powiazaniePytanieDrzewo[odpowiedz.PytanieRodzic].Items.Add(powiazanieOdpowiedzDrzewo[odpowiedz]);
        }

        public void UsunDrzewo()
        {
            drzewo.Items.Clear();
            korzen = null;
            powiazaniePytanieDrzewo.Clear();
            powiazanieOdpowiedzDrzewo.Clear();
        }

        public void UsunPytanie(Pytanie pytanie)
        {
            for(int i=0; i<pytanie.odpowiedzi.Count; ++i)
            {
                powiazanieOdpowiedzDrzewo.Remove(pytanie.odpowiedzi[i]);
            }

            korzen.Items.Remove(powiazaniePytanieDrzewo[pytanie]);
            powiazaniePytanieDrzewo.Remove(pytanie);
        }

        public void UsunOdpowiedz(Odpowiedz odpowiedz)
        {
            powiazaniePytanieDrzewo[odpowiedz.PytanieRodzic].Items.Remove(powiazanieOdpowiedzDrzewo[odpowiedz]);
            powiazanieOdpowiedzDrzewo.Remove(odpowiedz);
        }
    }
}
