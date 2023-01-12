using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaApplication1.Interpreter
{
    internal class PocitaniCisla
    {
        public Stack<double> ZasobnikCisel { get;}
        
        private int _ukazatel;
        private string[]? _splitPomocna;
        private readonly string _vstup;

        public PocitaniCisla(string vstup)
        {
            _vstup = vstup;
            ZasobnikCisel = new Stack<double>();
        }

        public bool NactiVyraz()
        {
            string? znamenkoOperator;

            _splitPomocna = _vstup.Split(" ");

            while (true)
            {
                NactiTerm();

                try
                {
                    znamenkoOperator = NactiPlusMinusNic(_splitPomocna);
                }
                catch (IndexOutOfRangeException)
                {
                    znamenkoOperator = null;
                }

                if (znamenkoOperator == null)
                {
                    return true;
                }

                double h1;
                double h2;
                double vysledek;


                if (znamenkoOperator == "+")
                {
                    _ukazatel++;
                    NactiTerm();

                    h1 = ZasobnikCisel.ElementAt(ZasobnikCisel.Count - (ZasobnikCisel.Count - 1));
                    h2 = ZasobnikCisel.ElementAt(ZasobnikCisel.Count - ZasobnikCisel.Count);
                    vysledek = h1 + h2;
                    ZasobnikCisel.Push(vysledek);
                }
                else if (znamenkoOperator == "-")
                {
                    _ukazatel++;
                    NactiTerm();

                    h1 = ZasobnikCisel.ElementAt(ZasobnikCisel.Count - (ZasobnikCisel.Count - 1));
                    h2 = ZasobnikCisel.ElementAt(ZasobnikCisel.Count - ZasobnikCisel.Count);
                    vysledek = h1 - h2;
                    ZasobnikCisel.Push(vysledek);
                }
            }
        }

        public bool NactiTerm()
        {
            NactiFaktor();

            string? znamenkoOperator;

            while (true)
            {
                try
                {
                    znamenkoOperator = NactiKratDelenoNic(_splitPomocna);
                }
                catch (IndexOutOfRangeException)
                {
                    znamenkoOperator = null;
                }

                if (znamenkoOperator == null)
                {
                    return true;
                }

                double h1;
                double h2;
                double vysledek;

                if (znamenkoOperator == "*")
                {
                    _ukazatel++;
                    NactiTerm();

                    h1 = ZasobnikCisel.ElementAt(ZasobnikCisel.Count - (ZasobnikCisel.Count - 1));
                    h2 = ZasobnikCisel.ElementAt(ZasobnikCisel.Count - ZasobnikCisel.Count);
                    vysledek = h1 * h2;
                    ZasobnikCisel.Pop();
                    ZasobnikCisel.Pop();
                    ZasobnikCisel.Push(vysledek);
                }
                else if (znamenkoOperator == "/")
                {
                    _ukazatel++;
                    NactiTerm();

                    h1 = ZasobnikCisel.ElementAt(ZasobnikCisel.Count - (ZasobnikCisel.Count - 1));
                    h2 = ZasobnikCisel.ElementAt(ZasobnikCisel.Count - ZasobnikCisel.Count);
                    vysledek = h1 / h2;
                    ZasobnikCisel.Pop();
                    ZasobnikCisel.Pop();
                    ZasobnikCisel.Push(vysledek);
                }
            }
        }

        public bool NactiFaktor()
        {
            if (NactiCislo(_splitPomocna))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool NactiCislo(string[]? pomocna)
        {
            try
            {
                if (pomocna != null && (pomocna[_ukazatel] == " " || pomocna[_ukazatel] == "" || !pomocna[_ukazatel].Any(char.IsNumber)))
                {
                    return false;
                }
                else
                {
                    if (pomocna != null) ZasobnikCisel.Push(double.Parse(pomocna[_ukazatel]));
                    _ukazatel++;
                    return true;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public string? NactiPlusMinusNic(string[] vstupNacti)
        {
            if (vstupNacti[_ukazatel] == "+" || vstupNacti[_ukazatel] == "-")
            {
                return vstupNacti[_ukazatel];
            }

            return null;
        }

        public string? NactiKratDelenoNic(string[]? vstupNacti)
        {
            if (vstupNacti != null && (vstupNacti[_ukazatel] == "*" || vstupNacti[_ukazatel] == "/"))
            {
                return vstupNacti[_ukazatel];
            }
            else
            {
                return null;
            }
        }
    }
}