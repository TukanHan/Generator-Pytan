using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeneratorPytan
{
    /// <summary>
    /// Główny kontener na dane i kluczowy podmiot programu.
    /// </summary>

    [Serializable]
    public class BazaDanych : WartownikZmian
    {
        public bool CzyModyfikowana { get; private set; } = true;

        public UstawieniaBazy Ustawienia { get; private set; }

        public List<Pytanie> pytania { get; private set; }
        public int PytaniaCount { get { return pytania.Count; } }

        public BazaDanych()
        {
            Ustawienia = new UstawieniaBazy(this);
            pytania = new List<Pytanie>();
        }

        public void DodajPytanie(Pytanie pytanie)
        {
            pytania.Add(pytanie);
            OnPropertyChanged("PytaniaCount");
            Modyfikacja();
        }

        public void UsunPytanie(Pytanie pytanie)
        {
            pytania.Remove(pytanie);
            OnPropertyChanged("PytaniaCount");
            Modyfikacja();
        }

        public bool CzyPoprawna()
        {
            try
            {
                Ustawienia.CzyPoprawna(pytania.Count);
                    
                foreach(Pytanie pytanie in pytania)               
                    pytanie.CzyPoprawne();

                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }       
        }

        public void Modyfikacja()
        {
            CzyModyfikowana = true;
            OnPropertyChanged("CzyModyfikowana");
        }

        public void Zapisano()
        {
            CzyModyfikowana = false;
            OnPropertyChanged("CzyModyfikowana");
        }
    }
}
