using System.Collections.Generic;

namespace SemInterpreter;

public class Funkce
{
    public string nazev;
    public List<string> teloFce;
    public List<string> parametry;
    public string navratovejTyp;
    public List<Promenna> promenneDef;

    public Funkce()
    {
        teloFce = new List<string>();
        parametry = new List<string>();
        promenneDef = new List<Promenna>();
    }
}