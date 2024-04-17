using System.Xml.Serialization;

namespace BalancedBooks_API.ExternalIntegrations.CompanyRegistry.Models;

public enum CegAllapot
{
    [XmlEnum("Bejegyzés alatt")] BegjegyzesAlatt,
    [XmlEnum("Hatályos")] Hatalyos,
    [XmlEnum("Törlés alatt")] TorlesAlatt,
    [XmlEnum("Törölve")] Torolve,
    [XmlEnum("Nem bejegyzett")] NemBejegyzett
}

public enum AdoszamAllapot
{
    [XmlEnum("érvényes adószám")] ErvenyesAdoszam
}

[XmlRoot(ElementName = "CégAdatlap",
    Namespace = "http://occsz.e-cegjegyzek.hu/info/page/schema/2006/XmlOccsz_v6_20170911#")]
public class CegAdatlap
{
    [XmlAttribute("fejléc")] public required string Fejlec;
    [XmlAttribute("rovat")] public required string Rovat;

    [XmlElement(ElementName = "Cégjegyzékszám")]
    public required string Cegjegyzekszam;

    [XmlElement(ElementName = "Név")] public required string Nev;

    [XmlElement(ElementName = "RövidNév")] public required string RovidNev;

    [XmlElement(ElementName = "Székhely")] public required string Szekhely;

    [XmlElement(ElementName = "Adószám")] public required string Adoszam;

    [XmlElement(ElementName = "AdószámÁllapot")]
    public required AdoszamAllapot AdoszamAllapot;

    [XmlElement(ElementName = "Állapot")] public required CegAllapot Allapot;

    [XmlElement(ElementName = "EUID")] public required string Euid;
}

public enum KiadmanyForma
{
    [XmlEnum("Kivonat")] Kivonat,
    [XmlEnum("Másolat")] Masolat
}

[XmlRoot(ElementName = "Kiadmány",
    Namespace = "http://occsz.e-cegjegyzek.hu/info/page/schema/2006/XmlOccsz_v6_20170911#")]
public class Kiadmany
{
    [XmlAttribute("adószám")] public required string Adoszam;
    [XmlAttribute("cím")] public required string Cim;
    [XmlAttribute("dátum")] public required DateTime Datum;
    [XmlAttribute("forma")] public required KiadmanyForma KiadmanyForma;
    [XmlAttribute("kezdősorszám")] public required int Kezdosorszam;
    [XmlAttribute("oldalszám")] public required int Oldalszam;
    [XmlAttribute("végsorszám")] public required int Vegsorszam;

    [XmlElement("CégAdatlap")] public required List<CegAdatlap> CegAdatlapok;
};

[XmlRoot(ElementName = "Cégjegyzék",
    Namespace = "http://occsz.e-cegjegyzek.hu/info/page/schema/2006/XmlOccsz_v6_20170911#")]
public class Cegjegyzek
{
    /// <summary>
    /// Can be null when hibajegyzek is null or not null (megadott feltételekkel a cégnyilvántartásban nincs nyilvántartott adat)
    /// </summary>
    [XmlElement("Kiadmány")] public Kiadmany? Kiadmany;

    /// <summary>
    /// Can be null when kiadmany is null
    /// </summary>
    [XmlElement(ElementName = "Hibajegyzék")]
    public Hibajegyzek? Hibajegyzek;
}

/* HIBA */

[XmlRoot(ElementName = "Hibajegyzék",
    Namespace = "http://occsz.e-cegjegyzek.hu/info/page/schema/2006/XmlOccsz_v6_20170911#")]
public class Hibajegyzek
{
    [XmlElement(ElementName = "Hiba")] public required Hiba Hiba;
}

public enum HibaKod
{
    [XmlEnum("11")] MissingRequestParams,
    [XmlEnum("41")] ResultTooBigOrSystemDoesNotAllowIt,
    [XmlEnum("121")] NoResultsByGivenFilters,
    [XmlEnum("122")] NoResultsByTaxNumber,
    [XmlEnum("123")] NoResultsByRegistrationNumber,
    [XmlEnum("114")] TooShortQueryFilters
}

[XmlRoot(ElementName = "Hiba", Namespace = "http://occsz.e-cegjegyzek.hu/info/page/schema/2006/XmlOccsz_v6_20170911#")]
public class Hiba
{
    [XmlAttribute("kód")] public required HibaKod Kod;
    [XmlText] public required string Text;
}

/* QUERY MODELS */

public enum LegalForm
{
    Nevfoglalas = 00,
    Vallalat = 01,
    Szovegkezet = 02,
    KozkeresetiTarsasag = 03,
    GazdasagiMunkakozosseg = 04,
    JogiSzemelyFelelossegvallalasavalMukodoGazdasagiMunkakozosseg = 05,
    BetetiTarsasag = 06,
    Egyesules = 07,
    KozosVallalat = 08,
    KorlatoltFelelosseguTarsasag = 09,
    Reszvenytarsasag = 10,
    EgyeniCeg = 11,
    KulfoldiekMagyarorszagiKozvetlenKereskedelmiKepviselete = 12,
    OktatoiMunkakozosseg = 13,
    KozhasznuTarsasag = 14,
    ErdobirtokossagiTarsulat = 15,
    VizgazdalkodasiTarsulat = 16,
    KulfoldiVallalkozasMagyarorszagiFioktelelepe = 17,
    VegrehajtoiIroda = 18,
    EuropaiGazdasagiEgyesules = 19,
    EuropaiReszvenytarsasag = 20,
    KozjegyzoiIroda = 21,
    KulfoldiSzekhelyuEuropaiGazdasagiEgyesulesMagyarorszagiTelephelye = 22,
    EuropaiSzovetkezet = 23
}

// State equals to the courts data
public enum StateCourts
{
    Orszagos = 00,
    Fovarosi = 01,
    Pecsi = 02,
    Kecskemeti = 03,
    Gyulai = 04,
    Miskolci = 05,
    Szegedi = 06,
    Szekesfehervari = 07,
    Gyori = 08,
    Debreceni = 09,
    Egri = 10,
    Tatabanyai = 11,
    Balassagyarmati = 12,
    BudapestKornyeki = 13,
    Kaposvari = 14,
    Nyiregyhazi = 15,
    Szolnoki = 16,
    Szekszardi = 17,
    Szombathelyi = 18,
    Veszpremi = 19,
    Zalaegerszegi = 20
}