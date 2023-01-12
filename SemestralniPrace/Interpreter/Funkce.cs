using System;
using System.Collections.Generic;

namespace AvaloniaApplication1.Interpreter;

public class Funkce
{
    private readonly string _nazev;
    private readonly List<string> _teloFce;
    private readonly List<string> _paramerty;
    private readonly string _navratovejTyp;
    private readonly List<Promenna> _promenneDef;
    
    public string Nazev => _nazev;
    public List<string> TeloFce => _teloFce;
    public List<string> Parametry => _paramerty;
    public string NavratovejTyp => _navratovejTyp;
    public List<Promenna> PromenneDef => _promenneDef;

    public Funkce(string nazev, string navratovejTyp, List<string> teloFce, List<string> parametry, List<Promenna> promenneDef)
    {
        _nazev = nazev ?? throw new ArgumentNullException(nameof(nazev));
        _navratovejTyp = navratovejTyp ?? throw new ArgumentNullException(nameof(navratovejTyp));
        _teloFce = teloFce ?? throw new ArgumentNullException(nameof(teloFce));
        _paramerty = parametry ?? throw new ArgumentNullException(nameof(parametry));
        _promenneDef = promenneDef ?? throw new ArgumentNullException(nameof(promenneDef));
    }
}

