using SemInterpreter;

StreamReader reader = new StreamReader("/home/ondra/RiderProjects/SemInterpreter/SemInterpreter/vstup.txt");

Interpreter inter = new Interpreter(reader.ReadToEnd());



