using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorPytan
{
    /// <summary>
    /// Kontener przechowujący info o odpowiedziach.
    /// </summary>

    [Serializable]
    public class Odpowiedz : WartownikZmian
    {
        public Pytanie PytanieRodzic { get; private set; }

        private string _tresc = "";
        public string tresc
        {
            get { return _tresc; }
            set
            {
                _tresc = value;
                OnPropertyChanged();
            }
        }

        private bool _czyPoprawna = false;
        public bool czyPoprawna
        {
            get { return _czyPoprawna; }
            set
            {
                _czyPoprawna = value;
                OnPropertyChanged();
            }
        }

        public Odpowiedz(Pytanie pytanie)
        {
            PytanieRodzic = pytanie;
            PropertyChanged += (sender, e) => { PytanieRodzic.BazaRodzic.Modyfikacja(); };
        }
    }
}
