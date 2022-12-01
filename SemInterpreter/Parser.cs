using System.Text.RegularExpressions;

namespace SemInterpreter;

public class Parser
{
    public static List<Promenna> promenne;
    public static List<Funkce> funkce;
    private Promenna pr;
    private PocitaniCisla pocc;
    private PocitaniString pocs;

    public Parser()
    {
        promenne = new List<Promenna>();
        funkce = new List<Funkce>();
    }

    public void ctiSlovo(string vstup) //jmeno
    {
        string[] radkySplit = vstup.Split('\n');
        var list = radkySplit.ToList();

        radkySplit = odeberMezery(list, radkySplit, 0);

        //Console.WriteLine(radkySplit.Length + " pocet radku");

        for (int j = 0; j < radkySplit.Length; j++)
        {
            string[] slova = radkySplit[j].Split(' ');
            slova = odeberMezery(list, slova, 0);

            string slovoHlavni = slova[0];

            //jestli to je print
            if (slovoHlavni.Contains("print"))
            {
                pr = new Promenna();
                funkcePrint(radkySplit[j], pr);
                Console.WriteLine(pr.hodnota);
            }
            else if (slovoHlavni.Contains("()")) // jestli to je volani fce
            {
                if (znamFunkci(slovoHlavni))
                {
                    Funkce def = dejFunkci(slovoHlavni);
                    string vstupDef = "";

                    for (int i = 0; i < def.teloFce.Count; i++)
                    {
                        vstupDef += def.teloFce[i] + "\n";
                    }
                    
                    ctiSlovo(vstupDef);
                }
            }
            else if (slovoHlavni.Contains("def")) // jestli to je vytvoreni fce
            {
                int pocet = j+1;
                Funkce def = new Funkce();
                string[] nazevFce = slova[1].Split("()");

                if (slova.Length > 2)
                {
                    string spoj = "";
                    for (int i = 1; i < slova.Length; i++)
                    {
                        spoj += slova[i];
                    }

                    string[] test = spoj.Split("(");
                    string[] test2 = test[1].Split(")");
                }

                def.nazev = nazevFce[0];
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
                funkce.Add(def);
            }
            else if (slovoHlavni.Contains("while"))
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

                    rovnoPr1 = odeberMezery(list, rovnoPr1, 0);
                    rovnoPr2 = odeberMezery(list, rovnoPr2, 0);


                    if (znamPromennou(rovnoPr1[0]))
                    {
                        pr1.datovejTyp = dejPromennou(rovnoPr1[0]).datovejTyp;
                    }

                    if (znamPromennou(rovnoPr2[0]))
                    {
                        pr2.datovejTyp = dejPromennou(rovnoPr2[0]).datovejTyp;
                    }

                    zjistiCoTamje(rovnoPr1, pr1, 0);
                    zjistiCoTamje(rovnoPr2, pr2, 0);

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

                    j = pozice-1;
                    int test = 0;

                    while (double.Parse(pr1.hodnota) < double.Parse(pr2.hodnota))
                    {
                        while ( test < listWhile.Count)
                        {
                            ctiSlovo(listWhile[test]);
                            test++;
                        }

                        test = 0;
                        zjistiCoTamje(rovnoPr1, pr1, 0);
                        zjistiCoTamje(rovnoPr2, pr2, 0);
                    }
                }else if (whileSplit2[1].Contains(">"))
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

                    rovnoPr1 = odeberMezery(list, rovnoPr1, 0);
                    rovnoPr2 = odeberMezery(list, rovnoPr2, 0);


                    if (znamPromennou(rovnoPr1[0]))
                    {
                        pr1.datovejTyp = dejPromennou(rovnoPr1[0]).datovejTyp;
                    }

                    if (znamPromennou(rovnoPr2[0]))
                    {
                        pr2.datovejTyp = dejPromennou(rovnoPr2[0]).datovejTyp;
                    }

                    zjistiCoTamje(rovnoPr1, pr1, 0);
                    zjistiCoTamje(rovnoPr2, pr2, 0);

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

                    j = pozice-1;
                    int test = 0;

                    while (double.Parse(pr1.hodnota) > double.Parse(pr2.hodnota))
                    {
                        while (test < listWhile.Count)
                        {
                            ctiSlovo(listWhile[test]);
                            test++;
                        }

                        test = 0;
                        zjistiCoTamje(rovnoPr1, pr1, 0);
                        zjistiCoTamje(rovnoPr2, pr2, 0);
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

                    rovnoPr1 = odeberMezery(list, rovnoPr1, 0);
                    rovnoPr2 = odeberMezery(list, rovnoPr2, 0);


                    if (znamPromennou(rovnoPr1[0]))
                    {
                        pr1.datovejTyp = dejPromennou(rovnoPr1[0]).datovejTyp;
                    }

                    if (znamPromennou(rovnoPr2[0]))
                    {
                        pr2.datovejTyp = dejPromennou(rovnoPr2[0]).datovejTyp;
                    }

                    zjistiCoTamje(rovnoPr1, pr1, 0);
                    zjistiCoTamje(rovnoPr2, pr2, 0);

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

                    j = pozice -1;
                    int test = 0;

                    while (double.Parse(pr1.hodnota) == double.Parse(pr2.hodnota))
                    {
                        while (test < listWhile.Count)
                        {
                            ctiSlovo(listWhile[test]);
                            test++;
                        }

                        test = 0;
                        zjistiCoTamje(rovnoPr1, pr1, 0);
                        zjistiCoTamje(rovnoPr2, pr2, 0);
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

                    rovnoPr1 = odeberMezery(list, rovnoPr1, 0);
                    rovnoPr2 = odeberMezery(list, rovnoPr2, 0);


                    if (znamPromennou(rovnoPr1[0]))
                    {
                        pr1.datovejTyp = dejPromennou(rovnoPr1[0]).datovejTyp;
                    }

                    if (znamPromennou(rovnoPr2[0]))
                    {
                        pr2.datovejTyp = dejPromennou(rovnoPr2[0]).datovejTyp;
                    }

                    zjistiCoTamje(rovnoPr1, pr1, 0);
                    zjistiCoTamje(rovnoPr2, pr2, 0);

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

                    j = pozice-1;
                    int test = 0;

                    while (double.Parse(pr1.hodnota) != double.Parse(pr2.hodnota))
                    {
                        while (test < listWhile.Count)
                        {
                            ctiSlovo(listWhile[test]);
                            test++;
                        }

                        test = 0;
                        zjistiCoTamje(rovnoPr1, pr1, 0);
                        zjistiCoTamje(rovnoPr2, pr2, 0);
                    }
                }
            }
            // jestli to je if
            else if (slovoHlavni.Contains("if"))
            {
                string[] ifSplit1 = radkySplit[j].Split(':');
                string[] ifSplit2 = ifSplit1[0].Split("if");
                int uroven;
                string elseUroven;
                string tab;

                if (ifSplit1[0].Contains("      if"))
                {
                    uroven = 3;
                    elseUroven = "      else:";
                    tab = "     ";
                }else if(ifSplit1[0].Contains("   if"))
                {
                    uroven = 2;
                    elseUroven = "    else:";
                    tab = " ";
                }
                else
                {
                    uroven = 1;
                    elseUroven = "else:";
                    tab = "";
                }

                if (ifSplit2[1].Contains("=="))
                {
                    Promenna pr1 = new Promenna();
                    Promenna pr2 = new Promenna();

                    string[] rovnoRovno = ifSplit2[1].Split("==");
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

                    rovnoPr1 = odeberMezery(list, rovnoPr1, 0);
                    rovnoPr2 = odeberMezery(list, rovnoPr2, 0);


                    if (znamPromennou(rovnoPr1[0]))
                    {
                        pr1.datovejTyp = dejPromennou(rovnoPr1[0]).datovejTyp;
                    }

                    if (znamPromennou(rovnoPr2[0]))
                    {
                        pr2.datovejTyp = dejPromennou(rovnoPr2[0]).datovejTyp;
                    }

                    zjistiCoTamje(rovnoPr1, pr1, 0);
                    zjistiCoTamje(rovnoPr2, pr2, 0);

                    List<string> listIf = new List<string>();
                    int pozice = j + 1;
                    string dejNoveSlovo = "";
                    int test = 0;

                    if (double.Parse(pr1.hodnota) == double.Parse(pr2.hodnota))
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
                                    dejNoveSlovo += radkySplit[pozice] + "\n";
                                    pozice++;
                                    while (radkySplit[pozice].Contains("      ") || radkySplit[pozice].Contains("   else"))
                                    {
                                        dejNoveSlovo += radkySplit[pozice] + "\n";
                                        pozice++;
                                    }
                                
                                    ctiSlovo(dejNoveSlovo);
                                }

                            }
                        }
                        catch (IndexOutOfRangeException e)
                        {
                        
                        }

                        j = pozice-1; 
                        test = 0;
                        
                        while (test < listIf.Count)
                        {
                            ctiSlovo(listIf[test]);
                            test++;
                        }
                        
                        if (radkySplit[pozice].Equals(elseUroven))
                        {
                            pozice++;
                            try
                            {
                                while (radkySplit[pozice].Contains("   "))
                                {
                                    //listIf.Add(radkySplit[pozice]);
                                    pozice++;
                                }
                            }
                            catch (IndexOutOfRangeException e)
                            {
                        
                            }

                            j = pozice-1;
                        }
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
                                        while (radkySplit[pozice].Contains("      ") || radkySplit[pozice].Contains("   else"))
                                        {
                                            dejNoveSlovo += radkySplit[pozice] + "\n";
                                            pozice++;
                                        }
                                
                                        ctiSlovo(dejNoveSlovo);
                                    }
                            
                                }
                            }
                            catch (IndexOutOfRangeException e)
                            {
                        
                            }

                            while (test < listIf.Count)
                            {
                                ctiSlovo(listIf[test]);
                                test++;
                            }

                            j = pozice - 1;
                        }
                    }
                }
                else if (ifSplit2[1].Contains(">"))
                {
                }
                else if (ifSplit2[1].Contains("<"))
                {
                }
            }
            //jestli to je promenna s type
            else if (slovoHlavni[slovoHlavni.Length - 1] == ':')
            {
                pr = new Promenna();
                string[] splitSlova = slovoHlavni.Split(':');
                pr.nazev = splitSlova[0];

                if (znamPromennou(splitSlova[0]) == false)
                {
                    zjistiCoTamje(slova, pr, 1);
                    promenne.Add(pr);
                }
                else
                {
                    Console.WriteLine("Promenna " + slova[0] + " existuje ");
                    Environment.Exit((int)ExitCode.VariableExist);
                }
            }
            //jestli to je promenna bez typu
            else if (slova[1] == "=")
            {
                if (znamPromennou(slova[0]))
                {
                    foreach (var VARIABLE in promenne)
                    {
                        if (VARIABLE.nazev == slova[0])
                        {
                            zjistiCoTamje(slova, VARIABLE, 1);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Zadana promnenna neexistuje: " + slova[0]);
                    Environment.Exit((int)ExitCode.VariableDoNotExist);
                }
            }
        }
    }

    private void zjistiCoTamje(string[] slova, Promenna promenna, int zacatek)
    {
        bool mohuPocitat = false;
        string pocitaniVstup = "";
        pocc = new PocitaniCisla();
        pocs = new PocitaniString();
        string skladaneSlovo = "";

        for (int i = zacatek; i < slova.Length; i++)
        {
            string slovo = slova[i];

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
            else if (slovo.Any(char.IsNumber))
            {
                if (slovo.Contains("."))
                {
                    if (promenna.datovejTyp == "double")
                    {
                        pocitaniVstup += slovo + " ";

                        if (i == slova.Length - 1)
                        {
                            pocc.vstup = pocitaniVstup;
                            pocc.NactiVyraz();
                            promenna.hodnota = pocc.zasobnikCisel.ElementAt(0).ToString();
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
                    if (promenna.datovejTyp == "int")
                    {
                        if (i == slova.Length - 1)
                        {
                            pocitaniVstup += slovo;
                            pocc.vstup = pocitaniVstup;
                            pocc.NactiVyraz();
                            promenna.hodnota = pocc.zasobnikCisel.ElementAt(0).ToString();
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
                            string noveSlovo = pocs.nactiVyraz(pocitaniVstup);
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
                            string noveSlovo = pocs.nactiVyraz(pocitaniVstup);
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
                if (znamPromennou(slovo))
                {
                    Promenna prNova = dejPromennou(slovo);
                    if (prNova.datovejTyp == promenna.datovejTyp && promenna.datovejTyp == "string")
                    {
                        pocitaniVstup += '"' + prNova.hodnota + '"';

                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            string noveSlovo = pocs.nactiVyraz(pocitaniVstup);
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
                            pocc.vstup = pocitaniVstup;
                            pocc.NactiVyraz();
                            promenna.hodnota = pocc.zasobnikCisel.ElementAt(0).ToString();
                        }
                    }
                    else if (prNova.datovejTyp == promenna.datovejTyp && promenna.datovejTyp == "double")
                    {
                        pocitaniVstup += double.Parse(prNova.hodnota);

                        if (i == slova.Length || i == slova.Length - 1)
                        {
                            pocc.vstup = pocitaniVstup;
                            pocc.NactiVyraz();
                            promenna.hodnota = pocc.zasobnikCisel.ElementAt(0).ToString();
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
    }

    private void funkcePrint(string slovoPrint, Promenna promenna)
    {
        string[] uvozovka1 = slovoPrint.Split("(");
        string[] uvozovka2 = uvozovka1[uvozovka1.Length - 1].Split(")");
        string[] slova = uvozovka2[0].Split(',');
        string pocitaniVstup = "";
        pocc = new PocitaniCisla();
        pocs = new PocitaniString();
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
                    string noveSlovo = pocs.nactiVyraz(pocitaniVstup);
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
                        string noveSlovo = pocs.nactiVyraz(pocitaniVstup);
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
                        string noveSlovo = pocs.nactiVyraz(pocitaniVstup);
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
                if (znamPromennou(slovo))
                {
                    Promenna prNova = dejPromennou(slovo);
                    pocitaniVstup += '"' + prNova.hodnota + '"';

                    if (i == slova.Length || i == slova.Length - 1)
                    {
                        string noveSlovo = pocs.nactiVyraz(pocitaniVstup);
                        promenna.hodnota = noveSlovo;
                    }
                }
                else
                {
                    Console.WriteLine("Toto v mem jazyku neexistuje: " + slovo);
                    Environment.Exit((int)ExitCode.UnknownError);
                }
            }
        }
    }

    private bool znamPromennou(string slovo)
    {
        foreach (var VARIABLE in promenne)
        {
            if (VARIABLE.nazev == slovo)
            {
                return true;
            }
        }

        return false;
    }
    
    private bool znamFunkci(string slovo)
    {
        foreach (var VARIABLE in funkce)
        {
            if (VARIABLE.nazev + "()" == slovo)
            {
                return true;
            }
        }

        return false;
    }

    private Promenna dejPromennou(string slovo)
    {
        foreach (var VARIABLE in promenne)
        {
            if (VARIABLE.nazev == slovo)
            {
                return VARIABLE;
            }
        }

        return null;
    }
    
    private Funkce dejFunkci(string slovo)
    {
        foreach (var VARIABLE in funkce)
        {
            if (VARIABLE.nazev + "()" == slovo)
            {
                return VARIABLE;
            }
        }

        return null;
    }

    private string[] odeberMezery(List<String> list, string[] zCehoToBeru, int pocetRadku)
    {
        // mozna pocitani mezer u radku
        int pocetMezer = 0;
        list = zCehoToBeru.ToList();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "" || list[i] == " ")
            {
                pocetMezer++;
                list.Remove(list[i]);
                i--;
            }
        }

        zCehoToBeru = list.ToArray();

        if (pocetMezer == 4)
        {
            pocetRadku++;
        }

        return zCehoToBeru;
    }
}