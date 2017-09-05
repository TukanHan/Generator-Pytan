using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorPytan
{
    /// <summary>
    /// Klasa która sprawdza czy dane w bazie zostały zmienione a w takim wypadku
    /// wypycha te informacje do miejsc w których są nasłuchiwane.
    /// </summary>

    [Serializable]
    public abstract class WartownikZmian : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
