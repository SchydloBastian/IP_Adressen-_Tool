# ***1. Was ist Blazor und wie funktioniert der Code?***

Blazor ist ein Web-Framework von Microsoft, mit dem man interaktive Webseiten mit C# und HTML baut. JavaScript wird dafür nicht benötigt.



Eine Blazor-Seite (.razor) kombiniert HTML-Inhalt für die Oberfläche und C#-Code für die Logik in einer einzigen Datei:



**Razor CSHTML**

**@page "/ip-pruefen"**

**@rendermode InteractiveServer**



**<h3>IP-Adresse prüfen</h3>**



**<!-- HTML \& Blazor-Anbindung -->**

**<input @bind="ipEingabe" placeholder="192.168.1.1" />**

**<button @onclick="Pruefen">Prüfen</button>**



**<p>Status: @ergebnis</p>**



**<!-- C#-Code und Logik -->**

**@code {**

&#x20;   **string ipEingabe = "";**

&#x20;   **string ergebnis = "";**



&#x20;   **void Pruefen()**

&#x20;   **{**

&#x20;       **ergebnis = "Gültige IP-Adresse";**

&#x20;   **}**

**}**





## ***Die wichtigsten Befehle auf einen Blick:***

@page "/...": Legt die Webadresse (URL/Route) fest, unter der die Seite erreichbar ist (z. B. https://localhost:7164/ip-pruefen).



@rendermode InteractiveServer: Wichtig! Diese Zeile muss oben stehen, damit Klicks (@onclick) und Eingaben (@bind) live auf dem Server verarbeitet werden. Ohne sie reagiert die Seite nicht auf Eingaben.



@bind="Variable": Verknüpft ein Eingabefeld direkt mit einer C#-Variable.



@onclick="Methode": Führt beim Klick auf einen Button eine C#-Funktion aus.



@code { ... }: Dieser Block enthält alle C#-Variablen, Methoden und die Berechnungslogik der Seite.







## ***2. Orientierung im Projekt (Wo liegt was?)***

Seiten (HTML \& UI): Liegen im Ordner Components/Pages/ (z. B. Home.razor).



C#-Logik \& Berechnungen: Komplexe Berechnungen gehören nicht direkt in die Seite, sondern in reine C#-Klassen (.cs-Dateien) im Ordner Logik/ (z. B. SubnetzRechner.cs).



Globales CSS: Liegt in wwwroot/app.css und gilt für das gesamte Projekt.



Seitenspezifisches CSS: Erstelle im selben Ordner eine Datei mit dem Namen der Seite (z. B. Home.razor.css). Visual Studio ordnet diese automatisch unter der jeweiligen .razor-Datei ein. Die Styles gelten dann nur für diese eine Seite.



Hauptmenü \& Rahmen: Liegt unter Components/Layout/MainLayout.razor (bzw. in der Navigationskomponente).







## ***3. Neue Seite erstellen \& im Hauptmenü einbinden***

Schritt 1: Neue Seite anlegen

Rechtsklick im Projektmappen-Explorer auf den Ordner Components/Pages → Hinzufügen → Razor-Komponente.



Einen passenden Namen vergeben (z. B. Subnetz.razor).



Ganz oben in der neuen Datei immer diese beiden Zeilen einfügen:



**Razor CSHTML**

**@page "/subnetz"**

**@rendermode InteractiveServer**

Schritt 2: Ins Hauptmenü einfügen

Öffne die Datei Components/Layout/MainLayout.razor (oder deine Navigationsleiste) und füge den Link zur neuen Seite an der passenden Stelle ein:



**HTML**

**<a href="subnetz">Subnetz-Rechner</a>**





## ***4. Schnellübersicht: Neue Funktion Schritt für Schritt einbauen***

Logik schreiben: C#-Klasse für die Berechnungen im Ordner Logik/ erstellen.



Seite erstellen: Neue .razor-Datei im Ordner Components/Pages/ anlegen.



Kopfzeilen eintragen: @page "/..." und @rendermode InteractiveServer oben auf der Seite einfügen.



Menü verlinken: Den Link zur neuen Route in MainLayout.razor hinzufügen.

