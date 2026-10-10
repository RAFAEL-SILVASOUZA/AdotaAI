namespace AdotaAI.Application.Exceptions;

public class ValidationFailedException : Exception
{
    public ValidationFailedException(IReadOnlyList<string> errors)
        : base("Dados do usuário são inválidos.")
    {
        Errors = errors;
    }

    public IReadOnlyList<string> Errors { get; }
}
