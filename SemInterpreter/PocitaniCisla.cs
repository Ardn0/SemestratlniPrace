using System.Drawing;

namespace SemInterpreter
{
    internal class PocitaniCisla
    {
        public string vstup = "";
        private int ukazatel;
        public Stack<double> zasobnikCisel = new Stack<double>();
        private string[] splitPomocna;

        public bool NactiVyraz()
        {
            string? znamenkoOperator;

            splitPomocna = vstup.Split(" ");

            while (true)
            {
                NactiTerm();

                try
                {
                    znamenkoOperator = NactiPlusMinusNic(splitPomocna);
                }
                catch (IndexOutOfRangeException e)
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
                    ukazatel++;
                    NactiTerm();

                    h1 = zasobnikCisel.ElementAt(zasobnikCisel.Count - (zasobnikCisel.Count - 1));
                    h2 = zasobnikCisel.ElementAt(zasobnikCisel.Count - zasobnikCisel.Count);
                    vysledek = h1 + h2;
                    zasobnikCisel.Push(vysledek);
                }
                else if (znamenkoOperator == "-")
                {
                    ukazatel++;
                    NactiTerm();

                    h1 = zasobnikCisel.ElementAt(zasobnikCisel.Count - (zasobnikCisel.Count - 1));
                    h2 = zasobnikCisel.ElementAt(zasobnikCisel.Count - zasobnikCisel.Count);
                    vysledek = h1 - h2;
                    zasobnikCisel.Push(vysledek);
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
                    znamenkoOperator = NactiKratDelenoNic(splitPomocna);
                }
                catch (IndexOutOfRangeException e)
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
                    ukazatel++;
                    NactiTerm();

                    h1 = zasobnikCisel.ElementAt(zasobnikCisel.Count - (zasobnikCisel.Count - 1));
                    h2 = zasobnikCisel.ElementAt(zasobnikCisel.Count - zasobnikCisel.Count);
                    vysledek = h1 * h2;
                    zasobnikCisel.Pop();
                    zasobnikCisel.Pop();
                    zasobnikCisel.Push(vysledek);
                }
                else if (znamenkoOperator == "/")
                {
                    ukazatel++;
                    NactiTerm();

                    h1 = zasobnikCisel.ElementAt(zasobnikCisel.Count - (zasobnikCisel.Count - 1));
                    h2 = zasobnikCisel.ElementAt(zasobnikCisel.Count - zasobnikCisel.Count);
                    vysledek = h1 / h2;
                    zasobnikCisel.Pop();
                    zasobnikCisel.Pop();
                    zasobnikCisel.Push(vysledek);
                }
            }
        }

        public bool NactiFaktor()
        {
            if (NactiCislo(splitPomocna) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool NactiCislo(string[] pomocna)
        {
            try
            {
                if (pomocna[ukazatel] == " " || pomocna[ukazatel] == "" || !pomocna[ukazatel].Any(char.IsNumber))
                {
                    return false;
                }
                else
                {
                    zasobnikCisel.Push(double.Parse(pomocna[ukazatel]));
                    ukazatel++;
                    return true;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
        }

        public string? NactiPlusMinusNic(string[] vstup)
        {
            if (vstup[ukazatel] == "+" || vstup[ukazatel] == "-")
            {
                return vstup[ukazatel];
            }

            return null;
        }

        public string? NactiKratDelenoNic(string[] vstup)
        {
            if (vstup[ukazatel] == "*" || vstup[ukazatel] == "/")
            {
                return vstup[ukazatel];
            }
            else
            {
                return null;
            }
        }
    }
}