using System.Collections.Generic;
using SemInterpreter;

namespace AvaloniaApplication1.Interpreter;

public class Interpreter
{
    public Interpreter(string vstup)
    {
        Lexer lex = new Lexer();
        lex.CtiSlovo(vstup,new List<Promenna>());
    }
}