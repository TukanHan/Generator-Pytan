using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;

namespace GeneratorPytan
{
    /// <summary>
    /// Kontener przechodujący info o ustawieniach bazy.
    /// </summary>

    [Serializable]
    public class UstawieniaBazy : WartownikZmian
    {
        public BazaDanych BazaRodzic { get; private set; }

        private string _NazwaBazy = "Nowa Baza";
        public string NazwaBazy
        {
            get { return _NazwaBazy;  }
            set
            {
                _NazwaBazy = value;
                OnPropertyChanged();
            }
        }

        private string _OpisBazy = "";
        public string OpisBazy
        {
            get { return _OpisBazy; }
            set
            {
                _OpisBazy = value;
                OnPropertyChanged();
            }
        }

        private string _AutorBazy = "";
        public string AutorBazy
        {
            get { return _AutorBazy; }
            set
            {
                _AutorBazy = value;
                OnPropertyChanged();
            }
        }

        private byte _CzasEgzaminu = 0;
        public byte CzasEgzaminu
        {
            get { return _CzasEgzaminu; }
            set
            {
                _CzasEgzaminu = value;
                OnPropertyChanged();
            }
        }

        private byte _LiczbaPytan = 0;
        public byte LiczbaPytan
        {
            get { return _LiczbaPytan; }
            set
            {
                _LiczbaPytan = value;
                OnPropertyChanged();
            }
        }

        public UstawieniaBazy(BazaDanych bazaDanych)
        {
            BazaRodzic = bazaDanych;
            PropertyChanged += (sender, e) => { BazaRodzic.Modyfikacja(); };
        }

        public bool CzyPoprawna(int liczbaPytanWBazie)
        {
            if(LiczbaPytan > liczbaPytanWBazie)
            {
                throw new Exception("Liczba losowanych pytań nie może być większa niż liczba pytań w bazie");
            }
            if(LiczbaPytan==0)
            {
                throw new Exception("Konieczne jest wylosowanie conajmniej jednego pytania");
            }
            if (NazwaBazy == "")
            {
                throw new Exception("Baza musi mieć nazwę");
            }
            if (((double)(CzasEgzaminu) * 60) / LiczbaPytan < 20)
            {
                throw new Exception("Minimalny czas na 1 pytanie to 20 sekund");
            }

            return true;
        }
    }
}
