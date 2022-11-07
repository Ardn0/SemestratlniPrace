namespace SemInterpreter;

public enum ExitCode
{
    Success = 0,
    InvalidDataType = 1,
    InvalidVarialbleName = 2,
    VariableExist = 3,
    VariableDoNotExist = 4,
    UnknownError = 10
}