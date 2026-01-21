namespace Askly.Application.Exceptions;

public class UserNotFoundException : ApplicationExceptionBase
{
    public UserNotFoundException(string email)
        : base($"Пользователь с емэйлом {email} не найден") { }
}