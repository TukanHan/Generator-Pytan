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
    /// Interaction logic for MainWindow.xaml
    /// Główny plik programu, zawiera ramki z innymi plikami.
    /// Służy jako węzeł który rozkazuje innym plikom wykonywać poszczególne operacje.
    /// Tu zarządza się operacjami na bazach danych.
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindowObject;

        private List<BazaDanych> bazyDanych;
        public BazaDanych AktualnieWybranaBaza { get; private set; }

        public MainWindow()
        {
            mainWindowObject = this;
            InitializeComponent();
            bazyDanych = new List<BazaDanych>();
            pasekStanu.OdznaczonoBaze();
            this.Closing += (sender, e) => { e.Cancel = !ZamknijAplikacje(); };
        }

        public void DodajBaze(BazaDanych baza)
        {
            bazyDanych.Add(baza);
            obszarRoboczy.DodajZakladke(baza);
        }

        public void DodajPytanie(Pytanie pytanie)
        {
            AktualnieWybranaBaza.DodajPytanie(pytanie);
            obszarPomocniczy.DodajPytanie(pytanie);
            obszarRoboczy.ZwrocOtwartaZakladke().OtworzPytania(pytanie);
        }

        public void OznaczBaze(BazaDanych bazaDanych)
        {
            AktualnieWybranaBaza = bazaDanych;

            pasekMenu.OznaczonoBaze();
            pasekStanu.OznaczonoBaze(bazaDanych);
            obszarPomocniczy.DodajDrzewo(bazaDanych);          
        }

        public void OdznaczBaze()
        {
            AktualnieWybranaBaza = null;
            pasekMenu.OdznaczonoBaze();
            pasekStanu.OdznaczonoBaze();
        }

        public void UsunBaze(BazaDanych bazaDanych)
        {
            if (AktualnieWybranaBaza == bazaDanych)
                OdznaczBaze();           
            obszarPomocniczy.UsunDrzewo();
            bazyDanych.Remove(bazaDanych);
        }

        public void UsunPytanie(Pytanie pytanie)
        {
            int id = AktualnieWybranaBaza.pytania.FindIndex((pytanko) => pytanko == pytanie);

            obszarPomocniczy.UsunPytanie(pytanie);
            AktualnieWybranaBaza.UsunPytanie(pytanie);

            if (id == 0)
            {
                if (AktualnieWybranaBaza.pytania.Count == 0)
                {
                    obszarRoboczy.ZwrocOtwartaZakladke().OtworzUstawienia();
                }
                else
                {
                    obszarRoboczy.ZwrocOtwartaZakladke().OtworzPytania(AktualnieWybranaBaza.pytania[0]);
                }
            }
            else
            {
                obszarRoboczy.ZwrocOtwartaZakladke().OtworzPytania(AktualnieWybranaBaza.pytania[id-1]);
            }            
        }
        
        public bool ZamknijAplikacje()
        {
            foreach(BazaDanych bazaDanych in bazyDanych)
            {
                if(bazaDanych.CzyModyfikowana)
                {
                    MessageBoxResult dialog = MessageBox.Show( $"Baza {bazaDanych.Ustawienia.NazwaBazy} została zmodyfikowana. Czy chcesz zapisać bazę przed wyjściem?",
                    "Wyjście", MessageBoxButton.YesNoCancel);

                    if(dialog == MessageBoxResult.Cancel)
                    {
                        return false;
                    }
                    else if(dialog == MessageBoxResult.Yes)
                    {
                        PrzygotowanieDoZapisu(bazaDanych);
                    }
                }
            }

            Application.Current.Shutdown();
            return true;
        }

        //Metody które wyświetlają windowsowe okienka pod wybór miejsca zapisu/odczytu
        //oraz wywołują metody klasy ZapisOdczyt.
        #region Wczytywanie/Zapisywanie
        public void PrzygotowanieDoZapisu(BazaDanych bazaDanych)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Plik projektu (*.proj)|*.proj";
            saveFileDialog.InitialDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "Projekty");
            if (saveFileDialog.ShowDialog() == true)
            {
                ZapisOdczyt.ZapisProjekt(bazaDanych, saveFileDialog.FileName, pasekStanu.DzialanieWTle);
            }
        }

        public void PrzygotiwanieDoWczytania()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Plik projektu (*.proj)|*.proj";
            openFileDialog.InitialDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "Projekty");
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    DodajBaze(ZapisOdczyt.WczytajProjekt(openFileDialog.FileName, pasekStanu.DzialanieWTle));
                }
                catch
                {
                    MessageBox.Show("Nie można wczytać bazy");
                }
            }
        }

        public void PrzygotowanieDoEksportu(BazaDanych bazaDanych)
        {
            if(bazaDanych.CzyPoprawna())
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Plik bazy (*.baza)|*.baza";
                saveFileDialog.InitialDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "Bazy");
                if (saveFileDialog.ShowDialog() == true)
                {
                    ZapisOdczyt.EksportujBaze(bazaDanych, saveFileDialog.FileName, pasekStanu.DzialanieWTle);
                }
            }          
        }

        public void PrzygotowanieDoEksportuPytan(BazaDanych bazaDanych)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if(result == System.Windows.Forms.DialogResult.OK)
                {
                    ZapisOdczyt.EksportujPytania(bazaDanych, dialog.SelectedPath, pasekStanu.DzialanieWTle);
                }
            }
        }

        public void PrzygotowanieDoImportuPytan()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Plik tekstowy (*.txt)|*.txt";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    ZapisOdczyt.ImportujPytanie(AktualnieWybranaBaza,openFileDialog.FileNames, pasekStanu.DzialanieWTle);                  
                }
                catch
                {
                    MessageBox.Show("Nie można wczytać bazy");
                }
                finally
                {
                    BazaDanych tymczasowa = AktualnieWybranaBaza;
                    OdznaczBaze();                   
                    OznaczBaze(tymczasowa);
                }
            }
        }
        #endregion
    }
}
