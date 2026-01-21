namespace Askly.Application.Exceptions;

public class PollNotFoundException : ApplicationExceptionBase
{
    public PollNotFoundException(Guid pollId)
        : base($"Опрос с идентификатором {pollId} не найден") { }
}