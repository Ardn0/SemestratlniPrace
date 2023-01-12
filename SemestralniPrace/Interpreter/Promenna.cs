using System;

namespace AvaloniaApplication1.Interpreter;

public class Promenna
{
    private string _hodnota;
    private string _datovejTyp;
    private readonly string _nazev;

    public string Hodnota
    {
        get => _hodnota;
        set => _hodnota = value;
    }

    public string DatovejTyp
    {
        get => _datovejTyp;
        set => _datovejTyp = value;
    }

    public string Nazev
    {
        get => _nazev;
    }

    public Promenna(string hodnota, string datovejTyp, string nazev)
    {
        _hodnota = hodnota ?? throw new ArgumentNullException(nameof(hodnota));
        _datovejTyp = datovejTyp ?? throw new ArgumentNullException(nameof(datovejTyp));
        _nazev = nazev ?? throw new ArgumentNullException(nameof(nazev));
    }

    public override string ToString()
    {
        return "Pomenna " + Nazev + " = " + Hodnota + " s datovym type " + DatovejTyp;
    }
}