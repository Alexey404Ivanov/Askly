namespace Askly.Application.Exceptions;

public class UserAlreadyExistsException : ApplicationExceptionBase
{
    public UserAlreadyExistsException() 
        : base("Пользователь с таким емэйлом уже существует") { }
}