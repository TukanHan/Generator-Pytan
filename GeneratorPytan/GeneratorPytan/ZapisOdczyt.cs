using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace GeneratorPytan
{
    /// <summary>
    /// Statyczna klasa służąca do zapisywania i wczytywania informacji w plikach:
    /// (txt) - w przypadku pojedyńczych pytań
    /// (baza) - w przypadku działającego pliku bazy danych
    /// (proj) - w przypadku projektu
    /// </summary>

    static class ZapisOdczyt
    {
        private static Encoding Kodowanie = Encoding.GetEncoding("Windows-1250");

        public static void ZapisProjekt(BazaDanych bazaDanych, string lokalizacja, Action<double, string> MetodaZwrotna)
        {
            try
            {
                MetodaZwrotna(0, "Zapisywanie bazy ");
                bazaDanych.Zapisano();

                using (FileStream sw = new FileStream(lokalizacja, FileMode.OpenOrCreate))
                {
                    var serializer = new BinaryFormatter();
                    MetodaZwrotna(0.25,String.Empty);
                    serializer.Serialize(sw, bazaDanych);
                }              
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                MetodaZwrotna(-1,String.Empty);
            }
        }

        public static BazaDanych WczytajProjekt(string lokalizacja, Action<double, string> MetodaZwrotna)
        {
            try
            {
                MetodaZwrotna(0, "Wczytywanie bazy ");
                using (FileStream sw = new FileStream(lokalizacja, FileMode.Open))
                {
                    var serializer = new BinaryFormatter();
                    MetodaZwrotna(0.25, String.Empty);
                    BazaDanych bazaDanych = (BazaDanych)serializer.Deserialize(sw);
                    bazaDanych.Ustawienia.PropertyChanged += (sender, args) => { bazaDanych.Modyfikacja(); };

                    for(int i=0; i< bazaDanych.PytaniaCount; ++i)
                    {
                        bazaDanych.pytania[i].PropertyChanged += (sender, args) => { bazaDanych.Modyfikacja(); };
                        for(int j =0; j< bazaDanych.pytania[i].OdpowiedziCount; ++j)
                        {
                            bazaDanych.pytania[i].odpowiedzi[j].PropertyChanged += (sender, args) => { bazaDanych.Modyfikacja(); };
                        }
                        MetodaZwrotna( (1/ bazaDanych.PytaniaCount)*i, "Wczytywanie bazy ");
                    }

                    return bazaDanych;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
            finally
            {
                MetodaZwrotna(-1, String.Empty);
            }
        }

        public static void EksportujBaze(BazaDanych bazaDanych, string lokalizacja, Action<double, string> MetodaZwrotna)
        {
            try
            {
                MetodaZwrotna(0, "Eksportowanie bazy ");

                using (StreamWriter sw = new StreamWriter(lokalizacja,false, Kodowanie))
                {
                    sw.WriteLine(bazaDanych.Ustawienia.NazwaBazy);
                    sw.WriteLine(bazaDanych.Ustawienia.OpisBazy);
                    sw.WriteLine(bazaDanych.Ustawienia.AutorBazy);
                    sw.WriteLine(bazaDanych.Ustawienia.LiczbaPytan);
                    sw.WriteLine(bazaDanych.Ustawienia.CzasEgzaminu);
                    sw.WriteLine(bazaDanych.PytaniaCount);

                    MetodaZwrotna(0.2, String.Empty);

                    foreach (Pytanie pytanie in bazaDanych.pytania)
                    {
                        sw.WriteLine(pytanie.pytanie);
                        sw.WriteLine(pytanie.trybPytania);
                        sw.WriteLine(pytanie.OdpowiedziCount);

                        foreach (Odpowiedz odpowiedz in pytanie.odpowiedzi)
                        {
                            sw.WriteLine(odpowiedz.tresc);
                            sw.WriteLine(odpowiedz.czyPoprawna);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                MetodaZwrotna(-1, String.Empty);
            }
        }

        public static void EksportujPytania(BazaDanych bazaDanych, string lokalizacja, Action<double, string> MetodaZwrotna)
        {
            try
            {
                MetodaZwrotna(0, "Eksportowanie pytań ");

                for(int i=0; i<bazaDanych.PytaniaCount; ++i)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(lokalizacja, $"{i+1}.txt"),false, Kodowanie))
                    {
                        sw.WriteLine(bazaDanych.pytania[i].pytanie);
                        var odpowiedzi = bazaDanych.pytania[i].odpowiedzi.Select((obiekt, index) => new { obiekt, ind = index+1 }).Where(pytanie => pytanie.obiekt.czyPoprawna).Select(pytanie => pytanie.ind).ToArray();
                        sw.WriteLine(String.Join(" ", odpowiedzi));

                        foreach(Odpowiedz odpowiedz in bazaDanych.pytania[i].odpowiedzi)
                        {
                            sw.WriteLine(odpowiedz.tresc);
                        }
                    }
                    MetodaZwrotna( (1/ bazaDanych.PytaniaCount)*i, String.Empty);
                }           
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                MetodaZwrotna(-1, String.Empty);
            }
        }

        public static void ImportujPytanie(BazaDanych bazaDanych, string[] lokalizacje, Action<double, string> MetodaZwrotna)
        {
            MetodaZwrotna(0, "Importowanie pytań ");

            for (int i = 0; i < lokalizacje.Length; ++i)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(lokalizacje[i], Kodowanie))
                    {
                        Pytanie pytanie = new Pytanie(bazaDanych, TrybPytania.Wielokrotne);
                        pytanie.pytanie = sr.ReadLine();

                        string[] poprawneOdpowiedzi = sr.ReadLine().Split();
                        string dane;
                        while ((dane = sr.ReadLine()) != null)
                        {
                            pytanie.DodajOdpowiedz(new Odpowiedz(pytanie) { tresc = dane });
                        }

                        foreach (string poprawnaOdpowiedz in poprawneOdpowiedzi)
                        {
                            pytanie.odpowiedzi[int.Parse(poprawnaOdpowiedz) - 1].czyPoprawna = true;
                        }

                        bazaDanych.DodajPytanie(pytanie);
                    }

                    MetodaZwrotna((1 / lokalizacje.Length) * i, String.Empty);
                }
                catch
                {
                    MessageBox.Show($"Problem z odczytem pliku {lokalizacje[i]}");
                }
            }

            MetodaZwrotna(-1, String.Empty);
        }
    }
}