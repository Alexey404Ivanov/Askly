namespace Askly.Application.Exceptions;

public class VoteNotFoundException : ApplicationExceptionBase
{
    public VoteNotFoundException()
        : base("Вы не голосовали в этом опросе ранее") { }
}