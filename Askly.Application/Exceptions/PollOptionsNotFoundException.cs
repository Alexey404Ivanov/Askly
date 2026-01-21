namespace Askly.Application.Exceptions;

public class PollOptionsNotFoundException : ApplicationExceptionBase
{
    public PollOptionsNotFoundException(Guid[] ids) 
        : base($"Варианты ответа с идентификаторами {string.Join(',', ids)} не найдены") { }
}