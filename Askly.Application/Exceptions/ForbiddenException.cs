namespace Askly.Application.Exceptions;

public class ForbiddenException : ApplicationExceptionBase
{
    public ForbiddenException(Guid pollId) 
        : base($"Вы не являетесь создателем опроса с идентификатором {pollId}") { }
}