namespace AvaloniaApplication1.Interpreter;

public class PocitaniString
{
    private string[]? _splitPomocna;

    public string NactiVyraz(string vstup)
    {
        string novejString = "";
        string[] splitZbavitSeUvozovek = vstup.Split('"');
        vstup = "";
        foreach (var variable in splitZbavitSeUvozovek)
        {
            if (variable != "")
            {
                vstup += variable;
            }
        }
        _splitPomocna = vstup.Split(" ");

        for (int i = 0; i < _splitPomocna.Length; i++)
        {
            if(i != _splitPomocna.Length -1)
            {
                novejString += _splitPomocna[i] + " ";
            }
            else
            {
                novejString += _splitPomocna[i];
            }
        }

        return novejString;
    }
}