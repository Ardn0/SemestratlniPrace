using SemInterpreter;

StreamReader reader = new StreamReader("/home/ondra/RiderProjects/SemestratlniPrace/SemInterpreter/vstup.txt");

Interpreter inter = new Interpreter(reader.ReadToEnd());



