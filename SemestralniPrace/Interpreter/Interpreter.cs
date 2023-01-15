using System.Collections.Generic;

namespace AvaloniaApplication1.Interpreter;

public class Interpreter
{
    private readonly Lexer _lex = new ();
    public string Vystup => _lex.Vystup;
    public string VystupChyba => _lex.VystupChyba;
    public string Input
    {
        get => _lex.Input;
        set => _lex.Input = value;
    }
    
    public bool Pokracuj
    {
        get => _lex.Pokracuj;
        set => _lex.Pokracuj = value;
    }

    public void Run(string vstup)
    {
        _lex.CtiSlovo(vstup,new List<Promenna>());
    }

    public void VymazVystup()
    {
        _lex.VymazVystup();
    }
}