namespace SemInterpreter;

public class Promenna
{
    public string hodnota;
    public string datovejTyp;
    public string nazev;

    public override string ToString()
    {
        return "Pomenna " + nazev + " = " + hodnota + " s datovym type " + datovejTyp;
    }
}