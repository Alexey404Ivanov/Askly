namespace Askly.Application.Exceptions;

public class InvalidPasswordException : ApplicationExceptionBase
{
    public InvalidPasswordException() 
        : base("Неправильный пароль") { }
}