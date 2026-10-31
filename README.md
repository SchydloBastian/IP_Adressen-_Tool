# IP-Adressen-Tool (Blazor Web App)

Ein webbasiertes Tool zur Analyse, Validierung und Berechnung von IPv4-Adressen und Subnetzen. Das Projekt basiert auf **Blazor (InteractiveServer)** mit **.NET 10** und C#.

---

##  Projektbeschreibung

Dieses Projekt bietet eine Benutzeroberfläche für Netzwerkberechnungen und die Analyse von IPv4-Adressen. Ziel ist es, komplexe Adress- und Subnetzberechnungen übersichtlich darzustellen und wesentliche Netzwerkdaten direkt aufzuschlüsseln.

---

##  Geplante & Umgesetzte Funktionen

- [ ] **IPv4-Prüfung:** Validierung von IP-Adressen auf Syntax und Gültigkeit sowie Erkennung von Adressklassen.
- [ ] **Subnetzberechnung:** Automatische Ermittlung von Netzwerkadresse, Broadcast-Adresse und dem nutzbaren Hostbereich.
- [ ] **Host-Berechnung:** Ermittlung der maximal nutzbaren Host-Anzahl im jeweiligen Subnetz.
- [ ] **Binärdarstellung:** Gegenüberstellung von IP-Adresse und Subnetzmaske in Oktett-Binärschreibweise.
- [ ] **CIDR-Unterstützung:** Auswertung und Umrechnung von Subnetzmasken in CIDR-Notation (z. B. `/24`).

*(Hinweis: Weitere Funktionen werden fortlaufend ergänzt.)*

---

##  Beteiligte Personen & Rollen

| Name / Alias | Rolle / Hauptaufgaben |
|---|---|
| **Bastian** | Projekt-Setup, Sebnet_Calculator, Projekt Erweiterung, CSS zweite Seite, js Datei erstellen, Export Funktion, CIDR IPv4/IPv6 verbessert, EUI64, Werkzeug   |
| **Mahdi** | HTML Hauptwebseite, Hostberechnung, HTML zweite Seite, CIDR Unterstützung, Stapel razor/css, Stapel berechnung, Fix erste /zweite Seite / css, |
| **Marten** | IP-Validierung & Logik, CIDR unterstützung, GeoLocation, DNS, Netz_Befehle |
| **Hassan** | Dokumentation/Presentaion & CSS-Styling, Binärdarstellung, IPv6 Converter, VlSM  |
---

##  Schnellstart & Installation

### Voraussetzungen
- Visual Studio mit der Workload **ASP.NET und Webentwicklung**
- **.NET 10 SDK**
- Git

### Schritt-für-Schritt

1. **Repository klonen:**
   ```bash
   git clone [https://github.com/SchydloBastian/IP-Adressen-Tool.git](https://github.com/SchydloBastian/IP-Adressen-Tool.git)