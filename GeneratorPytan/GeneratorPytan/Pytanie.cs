using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeneratorPytan
{
    /// <summary>
    /// Kontener przechowujący info o pytaniu.
    /// </summary>

    [Serializable]
    public class Pytanie : WartownikZmian
    {
        public BazaDanych BazaRodzic { get; private set; }

        private string _pytanie = "";
        public string pytanie
        {
            get { return _pytanie; }
            set
            {
                _pytanie = value;
                OnPropertyChanged();
            }
        }

        private TrybPytania _trybPytania;
        public TrybPytania trybPytania
        {
            get { return _trybPytania; }
            set
            {
                _trybPytania = value;
                OnPropertyChanged();
            }
        }

        public List<Odpowiedz> odpowiedzi { get; private set; }
        public int OdpowiedziCount { get { return odpowiedzi.Count; } }

        public Pytanie(BazaDanych bazaDanych, TrybPytania trybPytania) 
        {
            BazaRodzic = bazaDanych;
            odpowiedzi = new List<Odpowiedz>();
            this.trybPytania = trybPytania;

            PropertyChanged += (sender, e) => { BazaRodzic.Modyfikacja(); };
        }

        public void DodajOdpowiedz(Odpowiedz odpowiedz)
        {
            odpowiedzi.Add(odpowiedz);
            OnPropertyChanged("OdpowiedziCount");
        }

        public void UsunOdpowiedz(Odpowiedz odpowiedz)
        {
            odpowiedzi.Remove(odpowiedz);
            OnPropertyChanged("OdpowiedziCount");
        }

        public void UstawTrybPojedynczy()
        {
            bool pierwszy = false;
            for(int i=0; i< odpowiedzi.Count; ++i)
            {
                if(odpowiedzi[i].czyPoprawna && !pierwszy)               
                    pierwszy = true;
                else
                    odpowiedzi[i].czyPoprawna = false;                
            }
            trybPytania = TrybPytania.Jednokrotne;
        }

        public void UstawTrybWielokrotny()
        {
            trybPytania = TrybPytania.Wielokrotne;
        }

        public bool CzyPoprawne()
        {
            if(pytanie == "")
            {
                MessageBox.Show("Pytanie nie może być puste");
                return false;
            }
            if(trybPytania == TrybPytania.Jednokrotne && odpowiedzi.Count<2)
            {
                MessageBox.Show("Pytania jednokrotnego wyboru powinny mieć conajmniej 2 odpowiedzi");
                return false;
            }
            if (trybPytania == TrybPytania.Wielokrotne && odpowiedzi.Count < 1)
            {
                MessageBox.Show("Pytania wielokrotnego wyboru powinny mieć conajmniej 1 odpowiedź");
                return false;
            }

            int poprawne = 0;
            for (int i=0; i< odpowiedzi.Count; ++i)
            {             
                if(odpowiedzi[i].tresc == "")
                {
                    MessageBox.Show("Odpowiedź nie może być pusta");
                    return false;
                }
                if (odpowiedzi[i].czyPoprawna)
                    poprawne++;
            }

            if(trybPytania == TrybPytania.Jednokrotne && poprawne!=1)
            {
                MessageBox.Show("Pytania jednokrotnego wyboru muszą mieć jedną poprawną odpowiedź");
                return false;
            }

            return true;
        }
    }
}
