using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.JSInterop;

namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public class ExportIPv4
{
    public string IP { get; set; } = "";
    public string CIDR { get; set; } = "";
    public string Art { get; set; } = "";
    public string Klasse { get; set; } = "";
    public string Maske { get; set; } = "";
    public string Wildcard { get; set; } = "";
    public string Netz { get; set; } = "";
    public string ErsterHost { get; set; } = "";
    public string LetzterHost { get; set; } = "";
    public string Broadcast { get; set; } = "";
    public string NutzbareHosts { get; set; } = "";
    public string Hex { get; set; } = "";
    public string Ganzzahl { get; set; } = "";
    public string ReverseDns { get; set; } = "";
    public string VorherigesNetz { get; set; } = "";
    public string NaechstesNetz { get; set; } = "";
    public string Standort { get; set; } = "";
    public string Anbieter { get; set; } = "";

    // Nur für den Stapel: Fehlertext einer fehlerhaften Zeile (sonst leer).
    // Die Spalte "Fehler" kommt nur in die Datei, wenn mindestens eine Zeile einen Fehler hat.
    public string Fehler { get; set; } = "";
}

public class ExportIPv6
{
    public string Eingabe { get; set; } = "";
    public string Praefix { get; set; } = "";
    public string Art { get; set; } = "";
    public string Komprimiert { get; set; } = "";
    public string Expandiert { get; set; } = "";
    public string Netz { get; set; } = "";
    public string LetzteAdresse { get; set; } = "";
    public string Anzahl { get; set; } = "";
    public string EingebetteteIPv4 { get; set; } = "";
    public string ReverseDns { get; set; } = "";
    public string Standort { get; set; } = "";
}

public class ExportVlsm
{
    public string Hosts { get; set; } = "";
    public string Netz { get; set; } = "";
    public string CIDR { get; set; } = "";
    public string Erster { get; set; } = "";
    public string Letzter { get; set; } = "";
    public string Broadcast { get; set; } = "";
    public string Nutzbar { get; set; } = "";
    public string Fehler { get; set; } = "";
}

// Macht aus fertigen Ergebnissen Text. Hier wird nichts berechnet.
public static class Ergebnis_Export
{
    public const string MimeTxt = "text/plain;charset=utf-8";
    public const string MimeCsv = "text/csv;charset=utf-8";
    public const string MimeJson = "application/json;charset=utf-8";

    private const string Zeilenende = "\r\n";

    // ---------- IPv4 ----------
    public static string IPv4Txt(IEnumerable<ExportIPv4> daten) => Txt(IPv4Felder(daten));
    public static string IPv4Csv(IEnumerable<ExportIPv4> daten) => Csv(IPv4Felder(daten));
    public static string IPv4Json(IEnumerable<ExportIPv4> daten) => Json(IPv4Felder(daten));

    // ---------- IPv6 ----------
    public static string IPv6Txt(IEnumerable<ExportIPv6> daten) => Txt(IPv6Felder(daten));
    public static string IPv6Csv(IEnumerable<ExportIPv6> daten) => Csv(IPv6Felder(daten));
    public static string IPv6Json(IEnumerable<ExportIPv6> daten) => Json(IPv6Felder(daten));

    // ---------- VLSM ----------
    public static string VlsmTxt(IEnumerable<ExportVlsm> daten) => Txt(VlsmFelder(daten));
    public static string VlsmCsv(IEnumerable<ExportVlsm> daten) => Csv(VlsmFelder(daten));
    public static string VlsmJson(IEnumerable<ExportVlsm> daten) => Json(VlsmFelder(daten));

    // ---------- Download (Script: wwwroot/js/download.js) ----------
    public static async Task HerunterladenAsync(IJSRuntime js, string dateiname, string inhalt, string mime)
    {
        try
        {
            await js.InvokeVoidAsync("dateiHerunterladen", dateiname, inhalt, mime);
        }
        catch (JSException)
        {
            // Der Download ist fehlgeschlagen, die Seite bleibt aber verbunden
        }
    }

    // ---------- Felderlisten (Name + fertiger Wert) ----------
    private static List<List<(string Name, string Wert)>> IPv4Felder(IEnumerable<ExportIPv4> daten)
    {
        var liste = daten.ToList();
        bool mitFehler = liste.Any(d => !string.IsNullOrEmpty(d.Fehler));
        var ergebnis = new List<List<(string Name, string Wert)>>();

        foreach (var d in liste)
        {
            var felder = new List<(string Name, string Wert)>
            {
                ("IP", d.IP),
                ("CIDR", d.CIDR),
                ("Art", d.Art),
                ("Klasse", d.Klasse),
                ("Maske", d.Maske),
                ("Wildcard", d.Wildcard),
                ("Netz", d.Netz),
                ("erster Host", d.ErsterHost),
                ("letzter Host", d.LetzterHost),
                ("Broadcast", d.Broadcast),
                ("nutzbare Hosts", d.NutzbareHosts),
                ("Hex", d.Hex),
                ("Ganzzahl", d.Ganzzahl),
                ("Reverse-DNS", d.ReverseDns),
                ("vorheriges Netz", d.VorherigesNetz),
                ("nächstes Netz", d.NaechstesNetz),
                ("Standort", d.Standort),
                ("Anbieter", d.Anbieter)
            };
            if (mitFehler) felder.Add(("Fehler", d.Fehler));
            ergebnis.Add(felder);
        }
        return ergebnis;
    }

    private static List<List<(string Name, string Wert)>> IPv6Felder(IEnumerable<ExportIPv6> daten)
    {
        var ergebnis = new List<List<(string Name, string Wert)>>();
        foreach (var d in daten)
        {
            ergebnis.Add(new List<(string Name, string Wert)>
            {
                ("Eingabe", d.Eingabe),
                ("Präfix", d.Praefix),
                ("Art", d.Art),
                ("komprimiert", d.Komprimiert),
                ("expandiert", d.Expandiert),
                ("Netz", d.Netz),
                ("letzte Adresse", d.LetzteAdresse),
                ("Anzahl", d.Anzahl),
                ("eingebettete IPv4", d.EingebetteteIPv4),
                ("Reverse-DNS", d.ReverseDns),
                ("Standort", d.Standort)
            });
        }
        return ergebnis;
    }

    private static List<List<(string Name, string Wert)>> VlsmFelder(IEnumerable<ExportVlsm> daten)
    {
        var ergebnis = new List<List<(string Name, string Wert)>>();
        foreach (var d in daten)
        {
            ergebnis.Add(new List<(string Name, string Wert)>
            {
                ("Hosts", d.Hosts),
                ("Netz", d.Netz),
                ("CIDR", d.CIDR),
                ("erster", d.Erster),
                ("letzter", d.Letzter),
                ("Broadcast", d.Broadcast),
                ("nutzbar", d.Nutzbar),
                ("Fehler", d.Fehler)
            });
        }
        return ergebnis;
    }

    // ---------- Formate ----------
    private static string Txt(List<List<(string Name, string Wert)>> eintraege)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < eintraege.Count; i++)
        {
            if (i > 0) sb.Append(Zeilenende);

            int breite = eintraege[i].Max(f => f.Name.Length);
            foreach (var feld in eintraege[i])
            {
                sb.Append(feld.Name.PadRight(breite)).Append(" : ").Append(feld.Wert).Append(Zeilenende);
            }
        }
        return sb.ToString();
    }

    private static string Csv(List<List<(string Name, string Wert)>> eintraege)
    {
        var sb = new StringBuilder();
        sb.Append('\uFEFF'); // UTF-8-BOM, damit Excel die Umlaute richtig liest

        if (eintraege.Count == 0) return sb.ToString();

        sb.Append(string.Join(";", eintraege[0].Select(f => CsvFeld(f.Name)))).Append(Zeilenende);
        foreach (var eintrag in eintraege)
        {
            sb.Append(string.Join(";", eintrag.Select(f => CsvFeld(f.Wert)))).Append(Zeilenende);
        }
        return sb.ToString();
    }

    private static string CsvFeld(string? wert)
    {
        string text = wert ?? "";

        // Excel führt Zellen als Formel aus, die mit = + - @ beginnen. Ein Apostroph davor verhindert das.
        // Ein einzelnes "-" und negative Zahlen bleiben unverändert.
        bool formelStart = text.Length > 0 &&
            (text[0] == '=' || text[0] == '@' ||
             ((text[0] == '+' || text[0] == '-') && text.Length > 1 && !char.IsDigit(text[1])));
        if (formelStart) text = "'" + text;

        if (text.IndexOfAny(new[] { ';', '"', '\r', '\n' }) >= 0)
        {
            text = "\"" + text.Replace("\"", "\"\"") + "\"";
        }
        return text;
    }

    private static string Json(List<List<(string Name, string Wert)>> eintraege)
    {
        var optionen = new JsonWriterOptions
        {
            Indented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Umlaute bleiben lesbar
        };

        using var strom = new MemoryStream();
        using (var schreiber = new Utf8JsonWriter(strom, optionen))
        {
            schreiber.WriteStartArray();
            foreach (var eintrag in eintraege)
            {
                schreiber.WriteStartObject();
                foreach (var feld in eintrag)
                {
                    schreiber.WriteString(feld.Name, feld.Wert ?? "");
                }
                schreiber.WriteEndObject();
            }
            schreiber.WriteEndArray();
        }
        return Encoding.UTF8.GetString(strom.ToArray());
    }
}
