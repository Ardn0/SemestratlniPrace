using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Interpreter;

public class Parser
{
    private Lexer _lex;
    private List<Promenna> _promenneGlobal;
    public List<Promenna> PromenneLocal;
    private List<Funkce> _funkce;
    private Promenna? _pr;
    private PocitaniCisla? _pocc;
    private PocitaniString? _pocs;
    private bool _jsemVIf;
    private string _input;
    private bool _pokracuj;
    private string _vysput;
    private string _vystupChyba;

    public string Input
    {
        get => _input;
        set => _input = value;
    }

    public bool Pokracuj
    {
        get => _pokracuj;
        set => _pokracuj = value;
    }

    public string Vystup
    {
        get => _vysput;
        private set => _vysput = value;
    }

    public string VystpuChyba
    {
        get => _vystupChyba;
        private set => _vystupChyba = value;
    }

    public Parser(Lexer lex)
    {
        _promenneGlobal = new List<Promenna>();
        PromenneLocal = new List<Promenna>();
        _funkce = new List<Funkce>();
        _lex = lex;
        _funkce.Add(RandomInt());
        _funkce.Add(RandomDouble());
    }

    public void VymazVystup()
    {
        Vystup = "";
        VystpuChyba = "";
    }

    private Funkce RandomInt()
    {
        Promenna min = new Promenna("", "int", "min");
        Promenna max = new Promenna("", "int", "max");
        Promenna random = new Promenna("", "int", "random");

        Funkce randint = new Funkce("randint", "int", new List<string>() { "return random" },
            new List<string>() { "min: int", "max: int" }, new List<Promenna>() { min, max, random });

        return randint;
    }

    private Funkce RandomDouble()
    {
        Promenna min = new Promenna("", "double", "min");
        Promenna max = new Promenna("", "double", "max");
        Promenna random = new Promenna("", "double", "random");

        Funkce randdouble = new Funkce("randdouble", "double", new List<string>() { "return random" },
            new List<string>() { "min: double", "max: double" }, new List<Promenna>() { min, max, random });

        return randdouble;
    }

    public void Print(string[] radkySplit, int j)
    {
        Vystup += FunkcePrint(radkySplit[j]).Hodnota + "\n";
    }

    public void PretypovadinaDouble(string slovoHlavni)
    {
        string[] test = slovoHlavni.Split('(');
        string[] test1 = test[1].Split(')');

        if (ZnamPromennou(test1[0]))
        {
            Promenna? pr = DejPromennou(test1[0]);
            if (pr != null)
            {
                pr.DatovejTyp = "double";
                pr.Hodnota += ".0";
            }
        }
    }

    public void PretypovaninaInt(string slovoHlavni)
    {
        string[] test = slovoHlavni.Split('(');
        string[] test1 = test[1].Split(')');

        if (ZnamPromennou(test1[0]))
        {
            Promenna? pr = DejPromennou(test1[0]);
            if (pr != null)
            {
                pr.DatovejTyp = "int";
                int convert = (int)double.Parse(pr.Hodnota);
                pr.Hodnota = convert.ToString();
            }
        }
    }

    public void VolaniFce(string slovoHlavni)
    {
        string[] test = slovoHlavni.Split('(');
        string[] test1 = test[1].Split(')');
        string[] test2 = test1[0].Split(',');

        if (ZnamFunkci(test[0]))
        {
            Funkce? def = DejFunkci(test[0]);

            if (def != null && ((def.Parametry.Count != 0 && test1[0] != "") ||
                                (def.Parametry.Count == 0 && test1[0] == "")))
            {
                string vstupDef = "";

                for (int i = 0; i < def.TeloFce.Count; i++)
                {
                    vstupDef += def.TeloFce[i] + "\n";
                }

                if (def.Parametry.Count != 0 && test1[0] != "")
                {
                    for (int i = 0; i < def.Parametry.Count; i++)
                    {
                        try
                        {
                            def.PromenneDef[i].Hodnota = test2[i];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Chybi parametr u funkce: " + def.Nazev);
                            Environment.Exit((int)ExitCode.ParameterMissing);
                        }
                    }
                }

                _lex.CtiSlovo(vstupDef, def.PromenneDef);
                PromenneLocal.Clear();
            }
        }
    }

    public int VytvoreniFce(int j, string[] slova, string[] radkySplit)
    {
        int pocet = j + 1;
        string[] nazevFce = slova[1].Split('(');

        string jmeno = nazevFce[0];
        string navratovejTyp = "";
        List<string> parametry = new List<string>();
        List<Promenna> promenne = new List<Promenna>();
        List<String> teloFce = new List<string>();


        if (ZnamFunkci(jmeno))
        {
            Vystup += "Funkce uz existuje: " + jmeno;
            Environment.Exit((int)ExitCode.DefExist);
        }
        else
        {
            if (slova.Length > 2)
            {
                string spoj = "";
                for (int i = 1; i < slova.Length; i++)
                {
                    spoj += slova[i];
                }

                string[] test = spoj.Split("(");
                string[] test2 = test[1].Split(")");
                string[] test3 = test2[0].Split(',');

                if (test2[test2.Length - 1].Contains("->"))
                {
                    string[] test4 = test2[test2.Length - 1].Split("->");
                    string[] test5 = test4[1].Split(':');
                    navratovejTyp = test5[0];
                }

                foreach (var variable in test3)
                {
                    parametry.Add(variable);
                    string[] test4 = variable.Split(':');
                    Promenna pr = new Promenna("", test4[1], test4[0]);
                    promenne.Add(pr);
                }
            }

            try
            {
                while (radkySplit[pocet].Contains('\t'))
                {
                    teloFce.Add(radkySplit[pocet]);
                    pocet++;
                }
            }
            catch (IndexOutOfRangeException)
            {
            }

            j = pocet - 1;

            Funkce novaFunkce = new Funkce(nazevFce[0], navratovejTyp, teloFce, parametry, promenne);

            _funkce.Add(novaFunkce);

            return j;
        }

        return j;
    }

    private (Promenna, Promenna, string[], string[]) WhileRozdelovac(string[] split, Promenna jedna, Promenna dva,
        List<string> list, string[] radkySplit, int pozice, List<string> listWhile)
    {
        string[] rovnoPr1 = split[0].Split(" ");
        string[] rovnoPr2 = split[1].Split(" ");

        if (split[0].Any(char.IsNumber))
        {
            if (split[0].Contains("."))
            {
                jedna.DatovejTyp = "double";
            }
            else
            {
                jedna.DatovejTyp = "int";
            }
        }

        if (split[1].Any(char.IsNumber))
        {
            if (split[1].Contains("."))
            {
                dva.DatovejTyp = "double";
            }
            else
            {
                dva.DatovejTyp = "int";
            }
        }

        rovnoPr1 = OdeberMezery(list, rovnoPr1);
        rovnoPr2 = OdeberMezery(list, rovnoPr2);


        if (ZnamPromennou(rovnoPr1[0]))
        {
            var jednaDatovejTyp = DejPromennou(rovnoPr1[0])?.DatovejTyp;
            if (jednaDatovejTyp != null) jedna.DatovejTyp = jednaDatovejTyp;
        }

        if (ZnamPromennou(rovnoPr2[0]))
        {
            var dvaDatovejTyp = DejPromennou(rovnoPr2[0])?.DatovejTyp;
            if (dvaDatovejTyp != null) dva.DatovejTyp = dvaDatovejTyp;
        }

        ZjistiCoTamje(rovnoPr1, jedna, 0);
        ZjistiCoTamje(rovnoPr2, dva, 0);

        try
        {
            while (radkySplit[pozice].Contains('\t'))
            {
                listWhile.Add(radkySplit[pozice]);
                pozice++;
            }
        }
        catch (IndexOutOfRangeException)
        {
        }

        return (jedna, dva, rovnoPr1, rovnoPr2);
    }

    public int While(string[] radkySplit, int j, List<string> list, List<Promenna> listy)
    {
        string[] whileSplit1 = radkySplit[j].Split(':');
        string[] whileSplit2 = whileSplit1[0].Split("while");
        string[] rovnoPr1;
        string[] rovnoPr2;
        Promenna pr1 = new Promenna("", "", "");
        Promenna pr2 = new Promenna("", "", "");


        if (whileSplit2[1].Contains("<"))
        {
            string[] rovnoRovno = whileSplit2[1].Split("<");
            List<string> listWhile = new List<string>();
            int pozice = j + 1;

            (pr1, pr2, rovnoPr1, rovnoPr2) = WhileRozdelovac(rovnoRovno, pr1, pr2, list, radkySplit, pozice, listWhile);

            j = pozice - 1;
            int test = 0;

            while (double.Parse(pr1.Hodnota) < double.Parse(pr2.Hodnota)-1)
            {
                while (test < listWhile.Count)
                {
                    _lex.CtiSlovo(listWhile[test], listy);
                    test++;
                }

                test = 0;
                ZjistiCoTamje(rovnoPr1, pr1, 0);
                ZjistiCoTamje(rovnoPr2, pr2, 0);
            }

            return j;
        }

        if (whileSplit2[1].Contains(">"))
        {
            string[] rovnoRovno = whileSplit2[1].Split(">");
            List<string> listWhile = new List<string>();
            int pozice = j + 1;

            (pr1, pr2, rovnoPr1, rovnoPr2) = WhileRozdelovac(rovnoRovno, pr1, pr2, list, radkySplit, pozice, listWhile);

            j = pozice - 1;
            int test = 0;

            while (double.Parse(pr1.Hodnota) > double.Parse(pr2.Hodnota))
            {
                while (test < listWhile.Count)
                {
                    _lex.CtiSlovo(listWhile[test], listy);
                    test++;
                }

                test = 0;
                ZjistiCoTamje(rovnoPr1, pr1, 0);
                ZjistiCoTamje(rovnoPr2, pr2, 0);
            }

            return j;
        }

        if (whileSplit2[1].Contains("=="))
        {
            string[] rovnoRovno = whileSplit2[1].Split("==");
            List<string> listWhile = new List<string>();
            int pozice = j + 1;

            (pr1, pr2, rovnoPr1, rovnoPr2) = WhileRozdelovac(rovnoRovno, pr1, pr2, list, radkySplit, pozice, listWhile);

            j = pozice - 1;
            int test = 0;

            while (double.Parse(pr1.Hodnota) == double.Parse(pr2.Hodnota))
            {
                while (test < listWhile.Count)
                {
                    _lex.CtiSlovo(listWhile[test], listy);
                    test++;
                }

                test = 0;
                ZjistiCoTamje(rovnoPr1, pr1, 0);
                ZjistiCoTamje(rovnoPr2, pr2, 0);
            }

            return j;
        }

        if (whileSplit2[1].Contains("!="))
        {
            string[] rovnoRovno = whileSplit2[1].Split("!=");
            List<string> listWhile = new List<string>();
            int pozice = j + 1;

            (pr1, pr2, rovnoPr1, rovnoPr2) = WhileRozdelovac(rovnoRovno, pr1, pr2, list, radkySplit, pozice, listWhile);

            j = pozice - 1;
            int test = 0;

            while (double.Parse(pr1.Hodnota) != double.Parse(pr2.Hodnota))
            {
                while (test < listWhile.Count)
                {
                    _lex.CtiSlovo(listWhile[test], listy);
                    test++;
                }

                test = 0;
                ZjistiCoTamje(rovnoPr1, pr1, 0);
                ZjistiCoTamje(rovnoPr2, pr2, 0);
            }

            return j;
        }

        return j;
    }

    public int If(string[] radkySplit, int j, List<string> list, List<Promenna> listy)
    {
        _jsemVIf = true;
        string[] ifSplit1 = radkySplit[j].Split(':');
        string[] ifSplit2 = ifSplit1[0].Split("if");
        string elseUroven;

        if (ifSplit1[0].Contains("          if"))
        {
            elseUroven = "          else:";
        }
        else if (ifSplit1[0].Contains("      if"))
        {
            elseUroven = "      else:";
        }
        else if (ifSplit1[0].Contains("   if"))
        {
            elseUroven = "    else:";
        }
        else
        {
            elseUroven = "else:";
        }

        if (ifSplit2[1].Contains("=="))
        {
            return VneIf("==", ifSplit2, list, j, radkySplit, listy, elseUroven);
        }
        else if (ifSplit2[1].Contains(">"))
        {
            return VneIf(">", ifSplit2, list, j, radkySplit, listy, elseUroven);
        }
        else if (ifSplit2[1].Contains("<"))
        {
            return VneIf("<", ifSplit2, list, j, radkySplit, listy, elseUroven);
        }

        _jsemVIf = false;
        PromenneLocal.Clear();

        return 0;
    }

    public void VytvoreniPromenne(string slovoHlavni, string[] slova)
    {
        string[] splitSlova = slovoHlavni.Split(':');
        _pr = new Promenna("", "", splitSlova[0]);

        if (ZnamPromennou(splitSlova[0]) == false)
        {
            ZjistiCoTamje(slova, _pr, 1);
            if (_jsemVIf == false)
            {
                _promenneGlobal.Add(_pr);
            }
            else
            {
                PromenneLocal.Add(_pr);
            }
        }
        else
        {
            VystpuChyba = "Promenna " + slova[0] + " uz existuje ";
            Environment.Exit((int)ExitCode.VariableExist);
        }
    }

    public void PridelHodnotuPromenne(string[] slova)
    {
        if (slova[0].Contains('\t'))
        {
            List<char> list;
            list = slova[0].ToList();
            while (list.Contains('\t'))
            {
                list.Remove(list[0]);
            }

            string novej = "";
            for (int i = 0; i < list.Count; i++)
            {
                novej += list[i];
            }

            slova[0] = novej;
        }

        if (ZnamPromennou(slova[0]))
        {
            foreach (var variable in _promenneGlobal)
            {
                if (variable.Nazev == slova[0])
                {
                    ZjistiCoTamje(slova, variable, 1);
                }
            }

            foreach (var variable in PromenneLocal)
            {
                if (variable.Nazev == slova[0])
                {
                    ZjistiCoTamje(slova, variable, 1);
                }
            }
        }
        else
        {
            VystpuChyba = "Zadana promnenna neexistuje: " + slova[0];
            Environment.Exit((int)ExitCode.VariableDoNotExist);
        }
    }

    private void ZjistiCoTamje(string[] slova, Promenna promenna, int zacatek)
    {
        string pocitaniVstup = "";
        _pocs = new PocitaniString();

        for (int i = zacatek; i < slova.Length; i++)
        {
            string slovo = slova[i];

            if (slovo.Contains('(') && slovo.Contains(')')) // jestli to je volani fce
            {
                string[] test = slovo.Split('(');
                string[] test1 = test[1].Split(')');
                string[] test2 = test1[0].Split(',');

                if (ZnamFunkci(test[0]))
                {
                    Funkce? def = DejFunkci(test[0]);

                    bool maReturn = false;
                    if (def != null)
                    {
                        foreach (var variable in def.TeloFce)
                        {
                            if (variable.Contains("return"))
                            {
                                maReturn = true;
                            }
                        }

                        if (maReturn)
                        {
                            if ((def.Parametry.Count != 0 && test1[0] != "") ||
                                (def.Parametry.Count == 0 && test1[0] == ""))
                            {
                                string vstupDef = "";

                                for (int j = 0; j < def.TeloFce.Count; j++)
                                {
                                    vstupDef += def.TeloFce[j] + "\n";
                                }

                                if (def.Parametry.Count != 0 && test1[0] != "")
                                {
                                    int pocetPar = def.Parametry.Count;
                                    if (def.Nazev.Contains("rand"))
                                    {
                                        pocetPar = def.Parametry.Count + 1;
                                    }

                                    for (int j = 0; j < pocetPar; j++)
                                    {
                                        try
                                        {
                                            if (def.PromenneDef[j].Nazev == "random")
                                            {
                                                if (def.NavratovejTyp == "int")
                                                {
                                                    Random ran = new Random();
                                                    int pokus = ran.Next(int.Parse(def.PromenneDef[0].Hodnota),
                                                        int.Parse(def.PromenneDef[1].Hodnota));
                                                    def.PromenneDef[j].Hodnota = pokus.ToString();
                                                }
                                                else
                                                {
                                                    Random ran = new Random();
                                                    double pokus = ran.NextDouble() *
                                                                   (double.Parse(def.PromenneDef[1].Hodnota) -
                                                                    double.Parse(def.PromenneDef[0].Hodnota)) +
                                                                   double.Parse(def.PromenneDef[0].Hodnota);
                                                    def.PromenneDef[j].Hodnota =
                                                        pokus.ToString(CultureInfo.InvariantCulture);
                                                }
                                            }
                                            else
                                            {
                                                def.PromenneDef[j].Hodnota = test2[j];
                                            }
                                        }
                                        catch (IndexOutOfRangeException)
                                        {
                                            Console.WriteLine("Chybi parametr u funkce: " + def.Nazev);
                                            Environment.Exit((int)ExitCode.ParameterMissing);
                                        }
                                    }
                                }

                                _lex.CtiSlovo(vstupDef, def.PromenneDef);
                                var hodnota = PromenneLocal?[PromenneLocal.Count - 1].Hodnota;
                                if (hodnota != null)
                                    slovo = hodnota;
                                PromenneLocal?.Clear();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Funkce je void nebo nema return: " + def.Nazev);
                            Environment.Exit((int)ExitCode.DefType);
                        }
                    }
                }

                if (slovo.Contains("input"))
                {
                    while (Pokracuj == false)
                    {
                        Task.Delay(20);
                    }

                    slovo = Input;
                }
            }

            //jestli to je dotevej typ
            if (slovo == "int" | slovo == "string" | slovo == "double" | slovo == "bool")
            {
                if (slovo == "int")
                {
                    promenna.DatovejTyp = "int";
                }
                else if (slovo == "double")
                {
                    promenna.DatovejTyp = "double";
                }
                else if (slovo == "string")
                {
                    promenna.DatovejTyp = "string";
                }
                else if (slovo == "bool")
                {
                    promenna.DatovejTyp = "bool";
                }
            }
            // jestli to je number
            else if (slovo.Any(char.IsNumber))
            {
                if (slovo.Contains("."))
                {
                    if (promenna.DatovejTyp == "double")
                    {
                        pocitaniVstup += slovo + " ";

                        if (i == slova.Length - 1)
                        {
                            _pocc = new PocitaniCisla(pocitaniVstup);
                            _pocc.NactiVyraz();
                            promenna.Hodnota = _pocc.ZasobnikCisel.ElementAt(0).ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    else
                    {
                        VystpuChyba = "Spatnej datovej typ u promenne: " + promenna.Nazev;
                        //Environment.Exit((int)ExitCode.InvalidDataType);
                    }
                }
                else
                {
                    if (promenna.DatovejTyp == "int")
                    {
                        if (i == slova.Length - 1)
                        {
                            pocitaniVstup += slovo;
                            _pocc = new PocitaniCisla(pocitaniVstup);
                            _pocc.NactiVyraz();
                            promenna.Hodnota = _pocc.ZasobnikCisel.ElementAt(0).ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            pocitaniVstup += slovo + " ";
                        }
                    }
                    else
                    {
                        VystpuChyba = "Spatnej datovej typ u promenne: " + promenna.Nazev;
                        //Environment.Exit((int)ExitCode.InvalidDataType);
                    }
                }
            }
            // jestli to je string
            else if (slovo.Contains('"'))
            {
                string[] testSplit = slovo.Split('"');

                if (testSplit.Length == 3)
                {
                    pocitaniVstup += slovo;

                    if (promenna.DatovejTyp == "string")
                    {
                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            string noveSlovo = _pocs.NactiVyraz(pocitaniVstup);
                            promenna.Hodnota = noveSlovo;
                        }
                    }
                    else
                    {
                        VystpuChyba = "Spatnej datovej typ u promenne: " + promenna.Nazev;
                        //Environment.Exit((int)ExitCode.InvalidDataType);
                    }
                }
                else
                {
                    var skladaneSlovo = slovo;
                    int j = i + 1;

                    try
                    {
                        while (true)
                        {
                            if (!slova[j].Contains('"'))
                            {
                                skladaneSlovo += " " + slova[j];
                                j++;
                            }
                            else
                            {
                                skladaneSlovo += " " + slova[j];
                                break;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }

                    pocitaniVstup += skladaneSlovo;

                    i = j;

                    if (promenna.DatovejTyp == "string")
                    {
                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            string noveSlovo = _pocs.NactiVyraz(pocitaniVstup);
                            promenna.Hodnota = noveSlovo;
                        }
                    }
                    else
                    {
                        VystpuChyba = "Spatnej datovej typ u promenne: " + promenna.Nazev;
                        //Environment.Exit((int)ExitCode.InvalidDataType);
                    }
                }
            }
            else if (slovo == "true" || slovo == "false")
            {
                if (promenna.DatovejTyp == "bool")
                {
                    if (slovo == "true")
                    {
                        promenna.Hodnota = "true";
                    }
                    else
                    {
                        promenna.Hodnota = "false";
                    }
                }
            }
            else if (slovo is "+" or "-" or "*" or "/" or "==" or ">" or "<" or "and" or "or")
            {
                if (slovo == "+" && promenna.DatovejTyp is "int" or "double")
                {
                    pocitaniVstup += "+ ";
                }
                else if (slovo == "-" && promenna.DatovejTyp is "int" or "double")
                {
                    pocitaniVstup += "- ";
                }
                else if (slovo == "*" && promenna.DatovejTyp is "int" or "double")
                {
                    pocitaniVstup += "* ";
                }
                else if (slovo == "/" && promenna.DatovejTyp is "int" or "double")
                {
                    pocitaniVstup += "/ ";
                }
                else if (slovo == "==")
                {
                    pocitaniVstup += "== ";
                }
                else if (slovo == ">")
                {
                    pocitaniVstup += "== ";
                }
            }
            else if (slovo != "=")
            {
                if (ZnamPromennou(slovo))
                {
                    Promenna? prNova = DejPromennou(slovo);
                    if (prNova?.DatovejTyp == promenna.DatovejTyp && promenna.DatovejTyp == "string")
                    {
                        pocitaniVstup += '"' + prNova.Hodnota + '"';

                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            string noveSlovo = _pocs.NactiVyraz(pocitaniVstup);
                            promenna.Hodnota = noveSlovo;
                        }
                    }
                    else if (prNova?.DatovejTyp == promenna.DatovejTyp && promenna.DatovejTyp == "int")
                    {
                        if (pocitaniVstup == "")
                        {
                            pocitaniVstup += int.Parse(prNova.Hodnota) + " ";
                        }
                        else
                        {
                            pocitaniVstup += int.Parse(prNova.Hodnota);
                        }


                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            _pocc = new PocitaniCisla(pocitaniVstup);
                            _pocc.NactiVyraz();
                            promenna.Hodnota = _pocc.ZasobnikCisel.ElementAt(0).ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    else if (prNova?.DatovejTyp == promenna.DatovejTyp && promenna.DatovejTyp == "double")
                    {
                        pocitaniVstup += double.Parse(prNova.Hodnota);

                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            _pocc = new PocitaniCisla(pocitaniVstup);
                            _pocc.NactiVyraz();
                            promenna.Hodnota = _pocc.ZasobnikCisel.ElementAt(0).ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    else
                    {
                        VystpuChyba = "Spatnej datovej typ u promenne: " + promenna.Nazev;
                        //Environment.Exit((int)ExitCode.InvalidDataType);
                    }
                }
                else
                {
                    VystpuChyba = "Toto neexistuje: " + slovo;
                    //Environment.Exit((int)ExitCode.UnknownError);
                }
            }
        }
    }

    private Promenna FunkcePrint(string slovoPrint)
    {
        string[] uvozovka1 = slovoPrint.Split("(");
        string[] uvozovka2 = uvozovka1[uvozovka1.Length - 1].Split(")");
        string[] slova = uvozovka2[0].Split(',');
        string pocitaniVstup = "";
        _pocs = new PocitaniString();
        string skladaneSlovo;

        string hodnota = "";
        string datovejTyp = "";

        for (int i = 0; i < slova.Length; i++)
        {
            string slovo = slova[i];

            // jestli to je number
            if (slovo.Any(char.IsNumber))
            {
                pocitaniVstup += slovo + " ";

                if (i == slova.Length - 1)
                {
                    string noveSlovo = _pocs.NactiVyraz(pocitaniVstup);
                    hodnota = noveSlovo;
                }
            }
            // jestli to je string
            else if (slovo.Contains('"'))
            {
                string[] testSplit = slovo.Split('"');

                if (testSplit.Length == 3)
                {
                    pocitaniVstup += slovo;

                    if (i == slova.Length || i == slova.Length - 1)
                    {
                        string noveSlovo = _pocs.NactiVyraz(pocitaniVstup);
                        hodnota = noveSlovo;
                    }
                }
                else
                {
                    skladaneSlovo = slovo;
                    int j = i + 1;

                    try
                    {
                        while (true)
                        {
                            if (!slova[j].Contains('"'))
                            {
                                skladaneSlovo += " " + slova[j];
                                j++;
                            }
                            else
                            {
                                skladaneSlovo += " " + slova[j];
                                break;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }

                    pocitaniVstup += skladaneSlovo;

                    i = j;

                    if (i == slova.Length || i == slova.Length - 1)
                    {
                        string noveSlovo = _pocs.NactiVyraz(pocitaniVstup);
                        hodnota = noveSlovo;
                    }
                }
            }
            else if (slovo == "true" || slovo == "false")
            {
                if (datovejTyp == "bool")
                {
                    if (slovo == "true")
                    {
                        hodnota = "true";
                    }
                    else
                    {
                        hodnota = "false";
                    }
                }
            }
            else if (slovo is not "=" or "+" or "-" or "*" or "/")
            {
                if (ZnamPromennou(slovo))
                {
                    Promenna? prNova = DejPromennou(slovo);
                    pocitaniVstup += '"' + prNova?.Hodnota + '"';

                    if (i == slova.Length || i == slova.Length - 1)
                    {
                        string noveSlovo = _pocs.NactiVyraz(pocitaniVstup);
                        hodnota = noveSlovo;
                    }
                }
                else
                {
                    VystpuChyba = "Toto v mem jazyku neexistuje: " + slovo;
                }
            }
        }

        return new Promenna(hodnota, datovejTyp, "");
    }

    private bool ZnamPromennou(string slovo)
    {
        foreach (var variable in _promenneGlobal)
        {
            if (variable.Nazev == slovo)
            {
                return true;
            }
        }

        foreach (var variable in PromenneLocal)
        {
            if (variable.Nazev == slovo)
            {
                return true;
            }
        }

        return false;
    }

    private bool ZnamFunkci(string slovo)
    {
        foreach (var variable in _funkce)
        {
            if (variable.Nazev == slovo)
            {
                return true;
            }
        }

        return false;
    }

    private Promenna? DejPromennou(string slovo)
    {
        foreach (var variable in _promenneGlobal)
        {
            if (variable.Nazev == slovo)
            {
                return variable;
            }
        }

        foreach (var variable in PromenneLocal)
        {
            if (variable.Nazev == slovo)
            {
                return variable;
            }
        }

        return null;
    }

    private Funkce? DejFunkci(string slovo)
    {
        foreach (var variable in _funkce)
        {
            if (variable.Nazev == slovo)
            {
                return variable;
            }
        }

        return null;
    }

    private int VneIf(string znak, string[] ifSplit2, List<string> list, int j, string[] radkySplit,
        List<Promenna> listy, string elseUroven)
    {
        string[] rovnoRovno = ifSplit2[1].Split(znak);
        string[] rovnoPr1 = rovnoRovno[0].Split(" ");
        string[] rovnoPr2 = rovnoRovno[1].Split(" ");
        string datovejTypPr1 = "";
        string datovejTypPr2 = "";

        if (rovnoRovno[0].Any(char.IsNumber))
        {
            if (rovnoRovno[0].Contains("."))
            {
                datovejTypPr1 = "double";
            }
            else
            {
                datovejTypPr1 = "int";
            }
        }

        if (rovnoRovno[1].Any(char.IsNumber))
        {
            if (rovnoRovno[1].Contains("."))
            {
                datovejTypPr2 = "double";
            }
            else
            {
                datovejTypPr2 = "int";
            }
        }

        rovnoPr1 = OdeberMezery(list, rovnoPr1);
        rovnoPr2 = OdeberMezery(list, rovnoPr2);


        if (ZnamPromennou(rovnoPr1[0]))
        {
            var datovejTyp = DejPromennou(rovnoPr1[0])?.DatovejTyp;
            if (datovejTyp != null) datovejTypPr1 = datovejTyp;
        }

        if (ZnamPromennou(rovnoPr2[0]))
        {
            var datovejTyp = DejPromennou(rovnoPr2[0])?.DatovejTyp;
            if (datovejTyp != null) datovejTypPr2 = datovejTyp;
        }

        Promenna pr1 = new Promenna("", datovejTypPr1, "");
        Promenna pr2 = new Promenna("", datovejTypPr2, "");

        ZjistiCoTamje(rovnoPr1, pr1, 0);
        ZjistiCoTamje(rovnoPr2, pr2, 0);

        List<string> listIf = new List<string>();
        int pozice = j + 1;
        string dejNoveSlovo = "";
        int test = 0;


        if (znak == "==")
        {
            if (double.Parse(pr1.Hodnota) == double.Parse(pr2.Hodnota))
            {
                return VneVNeIf(radkySplit, pozice, test, dejNoveSlovo, listIf, listy, j, elseUroven);
            }
            else
            {
                while (!radkySplit[pozice].Equals("\t"+elseUroven))
                {
                    pozice++;
                }

                if (radkySplit[pozice].Contains("else"))
                {
                    listIf.Clear();
                    pozice++;
                    try
                    {
                        while (radkySplit[pozice].Contains('\t') && !radkySplit[pozice].Contains("else"))
                        {
                            if (!radkySplit[pozice].Contains("if"))
                            {
                                listIf.Add(radkySplit[pozice]);
                                pozice++;
                            }
                            else
                            {
                                dejNoveSlovo += radkySplit[pozice] + "\n";
                                pozice++;
                                while (radkySplit[pozice].Contains("      ") ||
                                       radkySplit[pozice].Contains("   else"))
                                {
                                    dejNoveSlovo += radkySplit[pozice] + "\n";
                                    pozice++;
                                }

                                _lex.CtiSlovo(dejNoveSlovo, listy);
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }

                    while (test < listIf.Count)
                    {
                        _lex.CtiSlovo(listIf[test], listy);
                        test++;
                    }

                    j = pozice - 1;
                    return j;
                }
            }
        }
        else if (znak == ">")
        {
            if (double.Parse(pr1.Hodnota) > double.Parse(pr2.Hodnota))
            {
                return VneVNeIf(radkySplit, pozice, test, dejNoveSlovo, listIf, listy, j, elseUroven);
            }
            else
            {
                while (!radkySplit[pozice].Equals(elseUroven))
                {
                    pozice++;
                }

                if (radkySplit[pozice].Contains("else"))
                {
                    listIf.Clear();
                    pozice++;
                    try
                    {
                        while (radkySplit[pozice].Contains('\t') && !radkySplit[pozice].Contains("else"))
                        {
                            if (!radkySplit[pozice].Contains("if"))
                            {
                                listIf.Add(radkySplit[pozice]);
                                pozice++;
                            }
                            else
                            {
                                dejNoveSlovo += radkySplit[pozice] + "\n";
                                pozice++;
                                while (radkySplit[pozice].Contains("      ") ||
                                       radkySplit[pozice].Contains("   else"))
                                {
                                    dejNoveSlovo += radkySplit[pozice] + "\n";
                                    pozice++;
                                }

                                _lex.CtiSlovo(dejNoveSlovo, listy);
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }

                    while (test < listIf.Count)
                    {
                        _lex.CtiSlovo(listIf[test], listy);
                        test++;
                    }

                    j = pozice - 1;
                    return j;
                }
            }
        }
        else if (znak == "<")
        {
            if (double.Parse(pr1.Hodnota) < double.Parse(pr2.Hodnota))
            {
                return VneVNeIf(radkySplit, pozice, test, dejNoveSlovo, listIf, listy, j, elseUroven);
            }
            else
            {
                while (!radkySplit[pozice].Equals(elseUroven))
                {
                    pozice++;
                }

                if (radkySplit[pozice].Contains("else"))
                {
                    listIf.Clear();
                    pozice++;
                    try
                    {
                        while (radkySplit[pozice].Contains('\t') && !radkySplit[pozice].Contains("else"))
                        {
                            if (!radkySplit[pozice].Contains("if"))
                            {
                                listIf.Add(radkySplit[pozice]);
                                pozice++;
                            }
                            else
                            {
                                dejNoveSlovo += radkySplit[pozice] + "\n";
                                pozice++;
                                while (radkySplit[pozice].Contains("      ") ||
                                       radkySplit[pozice].Contains("   else"))
                                {
                                    dejNoveSlovo += radkySplit[pozice] + "\n";
                                    pozice++;
                                }

                                _lex.CtiSlovo(dejNoveSlovo, listy);
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }

                    while (test < listIf.Count)
                    {
                        _lex.CtiSlovo(listIf[test], listy);
                        test++;
                    }

                    j = pozice - 1;
                    return j;
                }
            }
        }

        return j;
    }

    private int VneVNeIf(string[] radkySplit, int pozice, int test, string dejNoveSlovo, List<string> listIf,
        List<Promenna> listy, int j, string elseUroven)
    {
        try
        {
            while (radkySplit[pozice].Contains('\t') && !radkySplit[pozice].Contains("else"))
            {
                if (!radkySplit[pozice].Contains("if"))
                {
                    listIf.Add(radkySplit[pozice]);
                    pozice++;
                }
                else
                {
                    test = 0;

                    while (test < listIf.Count)
                    {
                        _lex.CtiSlovo(listIf[test], listy);
                        test++;
                    }

                    listIf.Clear();

                    dejNoveSlovo += radkySplit[pozice] + "\n";
                    pozice++;
                    while (radkySplit[pozice].Contains("      ") ||
                           radkySplit[pozice].Contains("   else"))
                    {
                        dejNoveSlovo += radkySplit[pozice] + "\n";
                        pozice++;
                    }

                    _lex.CtiSlovo(dejNoveSlovo, listy);
                }
            }
        }
        catch (IndexOutOfRangeException)
        {
        }

        j = pozice - 1;
        test = 0;

        while (test < listIf.Count)
        {
            _lex.CtiSlovo(listIf[test], listy);
            test++;
        }

        if (radkySplit[pozice].Equals("\t"+elseUroven))
        {
            pozice++;
            try
            {
                while (radkySplit[pozice].Contains('\t'))
                {
                    pozice++;
                }
            }
            catch (IndexOutOfRangeException)
            {
            }

            j = pozice - 1;
        }

        return j;
    }

    public string[] OdeberMezery(List<String> list, string[] zCehoToBeru)
    {
        list = zCehoToBeru.ToList();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "" || list[i] == " ")
            {
                list.Remove(list[i]);
                i--;
            }
        }

        zCehoToBeru = list.ToArray();

        return zCehoToBeru;
    }
}