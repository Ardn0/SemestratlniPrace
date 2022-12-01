namespace SemInterpreter;

public class Funkce
{
    public string nazev;
    public List<string> teloFce;
    public List<string> parametry;
    public string navratovejTyp;

    public Funkce()
    {
        teloFce = new List<string>();
        parametry = new List<string>();
    }
}