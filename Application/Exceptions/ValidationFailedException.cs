namespace AdotaAI.Application.Exceptions;

public class ValidationFailedException(
    IReadOnlyList<string> errors,
    string message = "Dados do usuário são inválidos.") : Exception(message)
{
    public IReadOnlyList<string> Errors { get; } = errors;
}