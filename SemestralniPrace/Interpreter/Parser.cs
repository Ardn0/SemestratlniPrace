using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SemInterpreter;

namespace AvaloniaApplication1.Interpreter;

public class Parser
{
    private Lexer _lex;
    private static List<Promenna>? _promenneGlobal;
    private static List<Promenna>? _promenneLocal;
    private static List<Funkce>? _funkce;
    private Promenna _pr;
    private PocitaniCisla _pocc;
    private PocitaniString _pocs;
    private bool _jsemVIf;
    public static string Vystup = "";
    public static string VystpuChyba = "";
    public static string Input = "";
    public static bool Pokracuj = false;

    public Parser(Lexer lex)
    {
        _promenneGlobal = new List<Promenna>();
        _promenneLocal = new List<Promenna>();
        _funkce = new List<Funkce>();
        _lex = lex;
    }

    public void Print(string[] radkySplit, int j)
    {
        _pr = new Promenna();
        FunkcePrint(radkySplit[j], _pr);
        Vystup += _pr.hodnota + "\n";
    }

    public void PretypovadinaDouble(string slovoHlavni)
    {
        string[] test = slovoHlavni.Split('(');
        string[] test1 = test[1].Split(')');

        if (ZnamPromennou(test1[0]))
        {
            Promenna pr = DejPromennou(test1[0]);
            pr.datovejTyp = "double";
            pr.hodnota += ".0";
        }
    }

    public void PretypovaninaInt(string slovoHlavni)
    {
        string[] test = slovoHlavni.Split('(');
        string[] test1 = test[1].Split(')');

        if (ZnamPromennou(test1[0]))
        {
            Promenna pr = DejPromennou(test1[0]);
            pr.datovejTyp = "int";
            int convert = (int)double.Parse(pr.hodnota);
            pr.hodnota = convert.ToString();
        }
    }

    public void VolaniFce(string slovoHlavni)
    {
        string[] test = slovoHlavni.Split('(');
        string[] test1 = test[1].Split(')');
        string[] test2 = test1[0].Split(',');

        if (ZnamFunkci(test[0]))
        {
            Funkce def = DejFunkci(test[0]);

            if ((def.parametry.Count != 0 && test1[0] != "") ||
                (def.parametry.Count == 0 && test1[0] == ""))
            {
                string vstupDef = "";

                for (int i = 0; i < def.teloFce.Count; i++)
                {
                    vstupDef += def.teloFce[i] + "\n";
                }

                if (def.parametry.Count != 0 && test1[0] != "")
                {
                    for (int i = 0; i < def.parametry.Count; i++)
                    {
                        try
                        {
                            def.promenneDef[i].hodnota = test2[i];
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            Console.WriteLine("Chybi parametr u funkce: " + def.nazev);
                            Environment.Exit((int)ExitCode.ParameterMissing);
                        }
                    }
                }

                _lex.CtiSlovo(vstupDef, def.promenneDef);
                _promenneLocal?.Clear();
            }
        }
    }

    public void VytvoreniFce(int j, string[] slova, string[] radkySplit)
    {
        int pocet = j + 1;
        Funkce def = new Funkce();
        string[] nazevFce = slova[1].Split('(');
        def.nazev = nazevFce[0];

        if (ZnamFunkci(def.nazev))
        {
            Console.WriteLine("Funkce uz existuje: " + def.nazev);
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
                    def.navratovejTyp = test5[0];
                }

                foreach (var VARIABLE in test3)
                {
                    def.parametry.Add(VARIABLE);
                    string[] test4 = VARIABLE.Split(':');
                    Promenna pr = new Promenna();
                    pr.nazev = test4[0];
                    pr.datovejTyp = test4[1];
                    def.promenneDef.Add(pr);
                }
            }


            try
            {
                while (radkySplit[pocet].Contains("   "))
                {
                    def.teloFce.Add(radkySplit[pocet]);
                    pocet++;
                }
            }
            catch (IndexOutOfRangeException e)
            {
            }


            j = pocet - 1;
            _funkce?.Add(def);
        }
    }

    public void While(string[] radkySplit, int j, List<string> list, List<Promenna> listy)
    {
        string[] whileSplit1 = radkySplit[j].Split(':');
        string[] whileSplit2 = whileSplit1[0].Split("while");

        if (whileSplit2[1].Contains("<"))
        {
            Promenna pr1 = new Promenna();
            Promenna pr2 = new Promenna();

            string[] rovnoRovno = whileSplit2[1].Split("<");
            string[] rovnoPr1 = rovnoRovno[0].Split(" ");
            string[] rovnoPr2 = rovnoRovno[1].Split(" ");

            if (rovnoRovno[0].Any(char.IsNumber))
            {
                if (rovnoRovno[0].Contains("."))
                {
                    pr1.datovejTyp = "double";
                }
                else
                {
                    pr1.datovejTyp = "int";
                }
            }

            if (rovnoRovno[1].Any(char.IsNumber))
            {
                if (rovnoRovno[1].Contains("."))
                {
                    pr2.datovejTyp = "double";
                }
                else
                {
                    pr2.datovejTyp = "int";
                }
            }

            rovnoPr1 = OdeberMezery(list, rovnoPr1);
            rovnoPr2 = OdeberMezery(list, rovnoPr2);


            if (ZnamPromennou(rovnoPr1[0]))
            {
                pr1.datovejTyp = DejPromennou(rovnoPr1[0]).datovejTyp;
            }

            if (ZnamPromennou(rovnoPr2[0]))
            {
                pr2.datovejTyp = DejPromennou(rovnoPr2[0]).datovejTyp;
            }

            ZjistiCoTamje(rovnoPr1, pr1, 0);
            ZjistiCoTamje(rovnoPr2, pr2, 0);

            List<string> listWhile = new List<string>();
            int pozice = j + 1;
            try
            {
                while (radkySplit[pozice].Contains("   "))
                {
                    listWhile.Add(radkySplit[pozice]);
                    pozice++;
                }
            }
            catch (IndexOutOfRangeException e)
            {
            }

            j = pozice - 1;
            int test = 0;

            while (double.Parse(pr1.hodnota) < double.Parse(pr2.hodnota))
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
        }
        else if (whileSplit2[1].Contains(">"))
        {
            Promenna pr1 = new Promenna();
            Promenna pr2 = new Promenna();

            string[] rovnoRovno = whileSplit2[1].Split(">");
            string[] rovnoPr1 = rovnoRovno[0].Split(" ");
            string[] rovnoPr2 = rovnoRovno[1].Split(" ");

            if (rovnoRovno[0].Any(char.IsNumber))
            {
                if (rovnoRovno[0].Contains("."))
                {
                    pr1.datovejTyp = "double";
                }
                else
                {
                    pr1.datovejTyp = "int";
                }
            }

            if (rovnoRovno[1].Any(char.IsNumber))
            {
                if (rovnoRovno[1].Contains("."))
                {
                    pr2.datovejTyp = "double";
                }
                else
                {
                    pr2.datovejTyp = "int";
                }
            }

            rovnoPr1 = OdeberMezery(list, rovnoPr1);
            rovnoPr2 = OdeberMezery(list, rovnoPr2);


            if (ZnamPromennou(rovnoPr1[0]))
            {
                pr1.datovejTyp = DejPromennou(rovnoPr1[0]).datovejTyp;
            }

            if (ZnamPromennou(rovnoPr2[0]))
            {
                pr2.datovejTyp = DejPromennou(rovnoPr2[0]).datovejTyp;
            }

            ZjistiCoTamje(rovnoPr1, pr1, 0);
            ZjistiCoTamje(rovnoPr2, pr2, 0);

            List<string> listWhile = new List<string>();
            int pozice = j + 1;
            try
            {
                while (radkySplit[pozice].Contains("   "))
                {
                    listWhile.Add(radkySplit[pozice]);
                    pozice++;
                }
            }
            catch (IndexOutOfRangeException e)
            {
            }

            j = pozice - 1;
            int test = 0;

            while (double.Parse(pr1.hodnota) > double.Parse(pr2.hodnota))
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
        }

        if (whileSplit2[1].Contains("=="))
        {
            Promenna pr1 = new Promenna();
            Promenna pr2 = new Promenna();

            string[] rovnoRovno = whileSplit2[1].Split("==");
            string[] rovnoPr1 = rovnoRovno[0].Split(" ");
            string[] rovnoPr2 = rovnoRovno[1].Split(" ");

            if (rovnoRovno[0].Any(char.IsNumber))
            {
                if (rovnoRovno[0].Contains("."))
                {
                    pr1.datovejTyp = "double";
                }
                else
                {
                    pr1.datovejTyp = "int";
                }
            }

            if (rovnoRovno[1].Any(char.IsNumber))
            {
                if (rovnoRovno[1].Contains("."))
                {
                    pr2.datovejTyp = "double";
                }
                else
                {
                    pr2.datovejTyp = "int";
                }
            }

            rovnoPr1 = OdeberMezery(list, rovnoPr1);
            rovnoPr2 = OdeberMezery(list, rovnoPr2);


            if (ZnamPromennou(rovnoPr1[0]))
            {
                pr1.datovejTyp = DejPromennou(rovnoPr1[0]).datovejTyp;
            }

            if (ZnamPromennou(rovnoPr2[0]))
            {
                pr2.datovejTyp = DejPromennou(rovnoPr2[0]).datovejTyp;
            }

            ZjistiCoTamje(rovnoPr1, pr1, 0);
            ZjistiCoTamje(rovnoPr2, pr2, 0);

            List<string> listWhile = new List<string>();
            int pozice = j + 1;
            try
            {
                while (radkySplit[pozice].Contains("   "))
                {
                    listWhile.Add(radkySplit[pozice]);
                    pozice++;
                }
            }
            catch (IndexOutOfRangeException e)
            {
            }

            j = pozice - 1;
            int test = 0;

            while (double.Parse(pr1.hodnota) == double.Parse(pr2.hodnota))
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
        }

        if (whileSplit2[1].Contains("!="))
        {
            Promenna pr1 = new Promenna();
            Promenna pr2 = new Promenna();

            string[] rovnoRovno = whileSplit2[1].Split("!=");
            string[] rovnoPr1 = rovnoRovno[0].Split(" ");
            string[] rovnoPr2 = rovnoRovno[1].Split(" ");

            if (rovnoRovno[0].Any(char.IsNumber))
            {
                if (rovnoRovno[0].Contains("."))
                {
                    pr1.datovejTyp = "double";
                }
                else
                {
                    pr1.datovejTyp = "int";
                }
            }

            if (rovnoRovno[1].Any(char.IsNumber))
            {
                if (rovnoRovno[1].Contains("."))
                {
                    pr2.datovejTyp = "double";
                }
                else
                {
                    pr2.datovejTyp = "int";
                }
            }

            rovnoPr1 = OdeberMezery(list, rovnoPr1);
            rovnoPr2 = OdeberMezery(list, rovnoPr2);


            if (ZnamPromennou(rovnoPr1[0]))
            {
                pr1.datovejTyp = DejPromennou(rovnoPr1[0]).datovejTyp;
            }

            if (ZnamPromennou(rovnoPr2[0]))
            {
                pr2.datovejTyp = DejPromennou(rovnoPr2[0]).datovejTyp;
            }

            ZjistiCoTamje(rovnoPr1, pr1, 0);
            ZjistiCoTamje(rovnoPr2, pr2, 0);

            List<string> listWhile = new List<string>();
            int pozice = j + 1;
            try
            {
                while (radkySplit[pozice].Contains("   "))
                {
                    listWhile.Add(radkySplit[pozice]);
                    pozice++;
                }
            }
            catch (IndexOutOfRangeException e)
            {
            }

            j = pozice - 1;
            int test = 0;

            while (double.Parse(pr1.hodnota) != double.Parse(pr2.hodnota))
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
        }
    }

    public int If(string[] radkySplit, int j, List<string> list, List<Promenna> listy)
    {
        _jsemVIf = true;
        string[] ifSplit1 = radkySplit[j].Split(':');
        string[] ifSplit2 = ifSplit1[0].Split("if");
        string elseUroven;
        int pozice = 0;

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
        _promenneLocal?.Clear();

        return 0;
    }

    public void VytvoreniPromenne(string slovoHlavni, string[] slova)
    {
        _pr = new Promenna();
        string[] splitSlova = slovoHlavni.Split(':');
        _pr.nazev = splitSlova[0];

        if (ZnamPromennou(splitSlova[0]) == false)
        {
            ZjistiCoTamje(slova, _pr, 1);
            if (_jsemVIf == false)
            {
                _promenneGlobal?.Add(_pr);
            }
            else
            {
                _promenneLocal?.Add(_pr);
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
        if (ZnamPromennou(slova[0]))
        {
            if (_promenneGlobal != null)
                foreach (var VARIABLE in _promenneGlobal)
                {
                    if (VARIABLE.nazev == slova[0])
                    {
                        ZjistiCoTamje(slova, VARIABLE, 1);
                    }
                }

            if (_promenneLocal != null)
                foreach (var VARIABLE in _promenneLocal)
                {
                    if (VARIABLE.nazev == slova[0])
                    {
                        ZjistiCoTamje(slova, VARIABLE, 1);
                    }
                }
        }
        else
        {
            VystpuChyba = "Zadana promnenna neexistuje: " + slova[0];
            Environment.Exit((int)ExitCode.VariableDoNotExist);
        }
    }

    private Task ZjistiCoTamje(string[] slova, Promenna promenna, int zacatek)
    {
        string pocitaniVstup = "";
        _pocc = new PocitaniCisla();
        _pocs = new PocitaniString();
        string skladaneSlovo;

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
                    Funkce def = DejFunkci(test[0]);

                    bool maReturn = false;
                    foreach (var VARIABLE in def.teloFce)
                    {
                        if (VARIABLE.Contains("return"))
                        {
                            maReturn = true;
                        }
                    }

                    if (def.navratovejTyp != null && maReturn)
                    {
                        if ((def.parametry.Count != 0 && test1[0] != "") ||
                            (def.parametry.Count == 0 && test1[0] == ""))
                        {
                            string vstupDef = "";

                            for (int j = 0; j < def.teloFce.Count; j++)
                            {
                                vstupDef += def.teloFce[j] + "\n";
                            }

                            if (def.parametry.Count != 0 && test1[0] != "")
                            {
                                for (int j = 0; j < def.parametry.Count; j++)
                                {
                                    try
                                    {
                                        def.promenneDef[j].hodnota = test2[j];
                                    }
                                    catch (IndexOutOfRangeException e)
                                    {
                                        Console.WriteLine("Chybi parametr u funkce: " + def.nazev);
                                        Environment.Exit((int)ExitCode.ParameterMissing);
                                    }
                                }
                            }

                            _lex.CtiSlovo(vstupDef, def.promenneDef);
                            slovo = _promenneLocal?[_promenneLocal.Count - 1].hodnota;
                            _promenneLocal?.Clear();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Funkce je void nebo nema return: " + def.nazev);
                        Environment.Exit((int)ExitCode.DefType);
                    }
                }

                if (slovo != null && slovo.Contains("input"))
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
                    promenna.datovejTyp = "int";
                }
                else if (slovo == "double")
                {
                    promenna.datovejTyp = "double";
                }
                else if (slovo == "string")
                {
                    promenna.datovejTyp = "string";
                }
                else if (slovo == "bool")
                {
                    promenna.datovejTyp = "bool";
                }
            }
            // jestli to je number
            else if (slovo != null && slovo.Any(char.IsNumber))
            {
                if (slovo.Contains("."))
                {
                    if (promenna.datovejTyp == "double")
                    {
                        pocitaniVstup += slovo + " ";

                        if (i == slova.Length - 1)
                        {
                            _pocc.vstup = pocitaniVstup;
                            _pocc.NactiVyraz();
                            promenna.hodnota = _pocc.zasobnikCisel.ElementAt(0).ToString();
                        }
                    }
                    else
                    {
                        VystpuChyba = "Spatnej datovej typ u promenne " + promenna.nazev;
                        //Environment.Exit((int)ExitCode.InvalidDataType);
                    }
                }
                else
                {
                    if (promenna.datovejTyp == "int")
                    {
                        if (i == slova.Length - 1)
                        {
                            pocitaniVstup += slovo;
                            _pocc.vstup = pocitaniVstup;
                            _pocc.NactiVyraz();
                            promenna.hodnota = _pocc.zasobnikCisel.ElementAt(0).ToString();
                        }
                        else
                        {
                            pocitaniVstup += slovo + " ";
                        }
                    }
                    else
                    {
                        Console.WriteLine("Spatnej datovej typ u promenne " + promenna.nazev);
                        Environment.Exit((int)ExitCode.InvalidDataType);
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

                    if (promenna.datovejTyp == "string")
                    {
                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            string noveSlovo = _pocs.nactiVyraz(pocitaniVstup);
                            promenna.hodnota = noveSlovo;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Spatnej datovej typ u promenne " + promenna.nazev);
                        Environment.Exit((int)ExitCode.InvalidDataType);
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
                    catch (IndexOutOfRangeException e)
                    {
                    }

                    pocitaniVstup += skladaneSlovo;

                    i = j;

                    if (promenna.datovejTyp == "string")
                    {
                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            string noveSlovo = _pocs.nactiVyraz(pocitaniVstup);
                            promenna.hodnota = noveSlovo;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Spatnej datovej typ u promenne " + promenna.nazev);
                        Environment.Exit((int)ExitCode.InvalidDataType);
                    }
                }
            }
            else if (slovo == "true" || slovo == "false")
            {
                if (promenna.datovejTyp == "bool")
                {
                    if (slovo == "true")
                    {
                        promenna.hodnota = "true";
                    }
                    else
                    {
                        promenna.hodnota = "false";
                    }
                }
            }
            else if (slovo is "+" or "-" or "*" or "/" or "==" or ">" or "<" or "and" or "or")
            {
                if (slovo == "+" && promenna.datovejTyp is "int" or "double")
                {
                    pocitaniVstup += "+ ";
                }
                else if (slovo == "-" && promenna.datovejTyp is "int" or "double")
                {
                    pocitaniVstup += "- ";
                }
                else if (slovo == "*" && promenna.datovejTyp is "int" or "double")
                {
                    pocitaniVstup += "* ";
                }
                else if (slovo == "/" && promenna.datovejTyp is "int" or "double")
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
                    Promenna prNova = DejPromennou(slovo);
                    if (prNova.datovejTyp == promenna.datovejTyp && promenna.datovejTyp == "string")
                    {
                        pocitaniVstup += '"' + prNova.hodnota + '"';

                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            string noveSlovo = _pocs.nactiVyraz(pocitaniVstup);
                            promenna.hodnota = noveSlovo;
                        }
                    }
                    else if (prNova.datovejTyp == promenna.datovejTyp && promenna.datovejTyp == "int")
                    {
                        if (pocitaniVstup == "")
                        {
                            pocitaniVstup += int.Parse(prNova.hodnota) + " ";
                        }
                        else
                        {
                            pocitaniVstup += int.Parse(prNova.hodnota);
                        }


                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            _pocc.vstup = pocitaniVstup;
                            _pocc.NactiVyraz();
                            promenna.hodnota = _pocc.zasobnikCisel.ElementAt(0).ToString();
                        }
                    }
                    else if (prNova.datovejTyp == promenna.datovejTyp && promenna.datovejTyp == "double")
                    {
                        pocitaniVstup += double.Parse(prNova.hodnota);

                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            _pocc.vstup = pocitaniVstup;
                            _pocc.NactiVyraz();
                            promenna.hodnota = _pocc.zasobnikCisel.ElementAt(0).ToString();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Spatnej datovej typ u promenne " + promenna.nazev);
                        Environment.Exit((int)ExitCode.InvalidDataType);
                    }
                }
                else
                {
                    Console.WriteLine("Toto neexistuje: " + slovo);
                    Environment.Exit((int)ExitCode.UnknownError);
                }
            }
        }

        return Task.CompletedTask;
    }

    private void FunkcePrint(string slovoPrint, Promenna promenna)
    {
        string[] uvozovka1 = slovoPrint.Split("(");
        string[] uvozovka2 = uvozovka1[uvozovka1.Length - 1].Split(")");
        string[] slova = uvozovka2[0].Split(',');
        string pocitaniVstup = "";
        _pocc = new PocitaniCisla();
        _pocs = new PocitaniString();
        string skladaneSlovo = "";

        for (int i = 0; i < slova.Length; i++)
        {
            string slovo = slova[i];

            // jestli to je number
            if (slovo.Any(char.IsNumber))
            {
                pocitaniVstup += slovo + " ";

                if (i == slova.Length - 1)
                {
                    string noveSlovo = _pocs.nactiVyraz(pocitaniVstup);
                    promenna.hodnota = noveSlovo;
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
                        string noveSlovo = _pocs.nactiVyraz(pocitaniVstup);
                        promenna.hodnota = noveSlovo;
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
                    catch (IndexOutOfRangeException e)
                    {
                    }

                    pocitaniVstup += skladaneSlovo;

                    i = j;

                    if (i == slova.Length || i == slova.Length - 1)
                    {
                        string noveSlovo = _pocs.nactiVyraz(pocitaniVstup);
                        promenna.hodnota = noveSlovo;
                    }
                }
            }
            else if (slovo == "true" || slovo == "false")
            {
                if (promenna.datovejTyp == "bool")
                {
                    if (slovo == "true")
                    {
                        promenna.hodnota = "true";
                    }
                    else
                    {
                        promenna.hodnota = "false";
                    }
                }
            }
            else if (slovo is not "=" or "+" or "-" or "*" or "/")
            {
                if (ZnamPromennou(slovo))
                {
                    Promenna prNova = DejPromennou(slovo);
                    pocitaniVstup += '"' + prNova.hodnota + '"';

                    if (i == slova.Length || i == slova.Length - 1)
                    {
                        string noveSlovo = _pocs.nactiVyraz(pocitaniVstup);
                        promenna.hodnota = noveSlovo;
                    }
                }
                else
                {
                    VystpuChyba = "Toto v mem jazyku neexistuje: " + slovo;
                    //Console.WriteLine("Toto v mem jazyku neexistuje: " + slovo);
                    //Environment.Exit((int)ExitCode.UnknownError);
                }
            }
        }
    }

    private bool ZnamPromennou(string slovo)
    {
        foreach (var VARIABLE in _promenneGlobal)
        {
            if (VARIABLE.nazev == slovo)
            {
                return true;
            }
        }

        foreach (var VARIABLE in _promenneLocal)
        {
            if (VARIABLE.nazev == slovo)
            {
                return true;
            }
        }

        return false;
    }

    private bool ZnamFunkci(string slovo)
    {
        foreach (var VARIABLE in _funkce)
        {
            if (VARIABLE.nazev == slovo)
            {
                return true;
            }
        }

        return false;
    }

    private Promenna DejPromennou(string slovo)
    {
        foreach (var VARIABLE in _promenneGlobal)
        {
            if (VARIABLE.nazev == slovo)
            {
                return VARIABLE;
            }
        }

        foreach (var VARIABLE in _promenneLocal)
        {
            if (VARIABLE.nazev == slovo)
            {
                return VARIABLE;
            }
        }

        return null;
    }

    private Funkce DejFunkci(string slovo)
    {
        foreach (var VARIABLE in _funkce)
        {
            if (VARIABLE.nazev == slovo)
            {
                return VARIABLE;
            }
        }

        return null;
    }

    private int VneIf(string znak, string[] ifSplit2, List<string> list, int j, string[] radkySplit,
        List<Promenna> listy, string elseUroven)
    {
        Promenna pr1 = new Promenna();
        Promenna pr2 = new Promenna();

        string[] rovnoRovno = ifSplit2[1].Split(znak);
        string[] rovnoPr1 = rovnoRovno[0].Split(" ");
        string[] rovnoPr2 = rovnoRovno[1].Split(" ");

        if (rovnoRovno[0].Any(char.IsNumber))
        {
            if (rovnoRovno[0].Contains("."))
            {
                pr1.datovejTyp = "double";
            }
            else
            {
                pr1.datovejTyp = "int";
            }
        }

        if (rovnoRovno[1].Any(char.IsNumber))
        {
            if (rovnoRovno[1].Contains("."))
            {
                pr2.datovejTyp = "double";
            }
            else
            {
                pr2.datovejTyp = "int";
            }
        }

        rovnoPr1 = OdeberMezery(list, rovnoPr1);
        rovnoPr2 = OdeberMezery(list, rovnoPr2);


        if (ZnamPromennou(rovnoPr1[0]))
        {
            pr1.datovejTyp = DejPromennou(rovnoPr1[0]).datovejTyp;
        }

        if (ZnamPromennou(rovnoPr2[0]))
        {
            pr2.datovejTyp = DejPromennou(rovnoPr2[0]).datovejTyp;
        }

        ZjistiCoTamje(rovnoPr1, pr1, 0);
        ZjistiCoTamje(rovnoPr2, pr2, 0);

        List<string> listIf = new List<string>();
        int pozice = j + 1;
        string dejNoveSlovo = "";
        int test = 0;


        if (znak == "==")
        {
            if (double.Parse(pr1.hodnota) == double.Parse(pr2.hodnota))
            {
                return VneVNeIf(radkySplit, pozice, test, dejNoveSlovo, listIf, listy, j, elseUroven);
            }else
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
                        while (radkySplit[pozice].Contains("   ") && !radkySplit[pozice].Contains("else"))
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
                    catch (IndexOutOfRangeException e)
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
            if (double.Parse(pr1.hodnota) > double.Parse(pr2.hodnota))
            {
                return VneVNeIf(radkySplit, pozice, test, dejNoveSlovo, listIf, listy, j, elseUroven);
            }else
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
                        while (radkySplit[pozice].Contains("   ") && !radkySplit[pozice].Contains("else"))
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
                    catch (IndexOutOfRangeException e)
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
            if (double.Parse(pr1.hodnota) < double.Parse(pr2.hodnota))
            {
                return VneVNeIf(radkySplit, pozice, test, dejNoveSlovo, listIf, listy, j, elseUroven);
            }else
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
                        while (radkySplit[pozice].Contains("   ") && !radkySplit[pozice].Contains("else"))
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
                    catch (IndexOutOfRangeException e)
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
            while (radkySplit[pozice].Contains("   ") && !radkySplit[pozice].Contains("else"))
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
        catch (IndexOutOfRangeException e)
        {
        }

        j = pozice - 1;
        test = 0;

        while (test < listIf.Count)
        {
            _lex.CtiSlovo(listIf[test], listy);
            test++;
        }

        if (radkySplit[pozice].Equals(elseUroven))
        {
            pozice++;
            try
            {
                while (radkySplit[pozice].Contains("   "))
                {
                    pozice++;
                }
            }
            catch (IndexOutOfRangeException e)
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