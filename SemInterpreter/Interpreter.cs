namespace SemInterpreter;

public class Interpreter
{
    public Interpreter(string vstup)
    {
        Parser par = new Parser();
        par.ctiSlovo(vstup);
        
    }
}