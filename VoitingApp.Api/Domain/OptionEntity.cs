namespace VoitingApp.Domain;

public class OptionEntity
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public Guid PoleId { get; set; }
    public int VotesCount { get; set; }
    
    public OptionEntity()
    {
        Id = Guid.Empty;
    }
    
    public OptionEntity(Guid id, string text, Guid poleId, int votesCount)
    {
        Id = id;
        Text = text;
        PoleId = poleId;
        VotesCount = votesCount;
    }
}