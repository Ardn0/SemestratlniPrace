namespace SemInterpreter;

public class PocitaniString
{
    private int ukazatel;
    private string[] splitPomocna;

    public string nactiVyraz(string vstup)
    {
        string novejString = "";
        string[] splitZbavitSeUvozovek = vstup.Split('"');
        vstup = "";
        foreach (var VARIABLE in splitZbavitSeUvozovek)
        {
            if (VARIABLE != "")
            {
                vstup += VARIABLE;
            }
        }
        splitPomocna = vstup.Split(" ");

        for (int i = 0; i < splitPomocna.Length; i++)
        {
            if(i != splitPomocna.Length -1)
            {
                novejString += splitPomocna[i] + " ";
            }
            else
            {
                novejString += splitPomocna[i];
            }
        }

        return novejString;
    }
}