using System;
using System.Collections.Generic;
using System.Linq;
using SemInterpreter;

namespace AvaloniaApplication1.Interpreter;

public class Lexer
{
    private Parser _par;

    public Lexer()
    {
        _par = new Parser(this);
    }
    
    public void CtiSlovo(string vstup, List<Promenna> listy)
    {
        if (listy.Count != 0)
        {
            foreach (var VARIABLE in listy)
            {
                _par._promenneLocal.Add(VARIABLE);
            }
        }
        
        string[] radkySplit = vstup.Split('\n');
        var list = radkySplit.ToList();

        radkySplit = _par.OdeberMezery(list, radkySplit);
        

        for (int j = 0; j < radkySplit.Length; j++)
        {
            string[] slova = radkySplit[j].Split(' ');
            slova = _par.OdeberMezery(list, slova);

            string slovoHlavni = slova[0];
            
            if (slovoHlavni.Contains("print")) // jestli to je print
            {
                _par.Print(radkySplit,j);
            }
            else if (slovoHlavni.Contains("return"))
            {
                break;
            }
            else if (slovoHlavni.Contains("double")) // jestli to je pretypovani
            {
               _par.PretypovadinaDouble(slovoHlavni);
            }
            else if (slovoHlavni.Contains("int")) // jestli to je pretypovani
            {
                _par.PretypovaninaInt(slovoHlavni);
            }
            else if (slovoHlavni.Contains('(') && slovoHlavni.Contains(')')) // jestli to je volani fce
            {
                _par.VolaniFce(slovoHlavni);
            }
            else if (slovoHlavni.Contains("def")) // jestli to je vytvoreni fce
            {
                j = _par.VytvoreniFce(j,slova,radkySplit);
            }
            else if (slovoHlavni.Contains("while")) // jestli to je while
            {
                j = _par.While(radkySplit, j, list, listy);
            }
            else if (slovoHlavni.Contains("if")) // jestli to je if
            {
              j = _par.If(radkySplit, j, list, listy);
            }
            else if (slovoHlavni[slovoHlavni.Length - 1] == ':')  // jestli to je vytvoreni promenne
            {
                _par.VytvoreniPromenne(slovoHlavni,slova);
            }
            else if (slova[1] == "=") // jestli to je prirazeni hodnoty do promenne
            {
               _par.PridelHodnotuPromenne(slova);
            }
        }
    }
}