# Generator-pytan
Program w typowej windowsowej konwencji graficznej służący do tworzenia testów.

# Pomoc:

Cel programu:

Program umożliwia tworzenie baz danych wykożystywanych przez program do testowania.
  
Nowa Baza:

Przez (Menu->Plik->Nowa Baza) możemy utworzyć nową bazę którą możemy eksportoać do pliku używanego przez program testujący (.baza) lub zapisać do pliku (.proj) by móc ją puźniej odczytać.
Każda baza podzielona jest na dwie części.: Ustawienia - dotyczącze informacji o całej bazie. Pytań - które pojawią się w programie testowym.

Ustawienia Bazy:

Każda baza zawiera swoje prywatne ustawienia.
Nazwa Bazy - wymagany element reprezentujący bazę w programie testującymy.
Opis Bazy - element który któtko streśći harakterystykę bazy.
Autor Bazy - Użytkownika tego programu.
Lczbę pytań - ilość pytań wylosowanych z całej puli podczas egzaminu.
Czas egzaminu - czas dany użytkownikowi testu na jego rozwiązanie.

Pytania:

Pytania mogą być jednokrotnego lub wielokrotnego wyboru. Opcję tą wybieramy przez wybór radio buttona.
Treść pytania jest ustalona w okienku pytanie.
Pytanie jednokrotnego wyboru powinno mieć co najmniej 2 odpowiedzi i dokładnie 1 poprawną odpowiedź.
Pytanie wielokrotnego wyboru powinny mieć co najmniej 1 odpowiedź i dowolną liczbę poprawnych odpowiedzi.

Nawigacja:

Pytania można przewijać oraz usówać w okienku pytania.
Pytania można przewijać oraz usówać na pasku menu.
Za pomocą paska nawigacji po prawej stronie możemy przenieść się do dowolnie wybranego pytania w bazie lub ustawień bazy.

Import / Eksport:

Program udostępnia możliwość zapisu projektu do pliku (.proj): Menu->Baza->Zapisz Baze.
Gotowy projekt w formacie (.proj) można wczytać: Menu->Plik->Wczytaj Baze.
Kompletną bazę można eksportować do pliku (.baza). Program uprzednio sprawdzi poprawność bazy:.  Menu->Baza->Eksportuj Bazę.
Do programu można importować pytania w formacie (.txt):  Menu->Baza->Importuj Pytania.
Pytania można eksportować do pliku (.txt):  Menu->Baza->Eksportuj Pytania.
