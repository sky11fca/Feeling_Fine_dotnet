namespace DotnetApi.Domains.Entities;

public class Reply
{
    private Reply()
    {
        
    }

    public static Reply Create(Guid reviewId, Guid toClientId, string rawText)
    {
        if (reviewId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(reviewId));
        }

        if (toClientId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(toClientId));
        }
        
        if (string.IsNullOrEmpty(rawText))
        {
            throw new ArgumentNullException(nameof(rawText));
        }

        return new Reply
        {
            Id = Guid.NewGuid(),
            ReviewId = reviewId,
            ToClientId = toClientId,
            RawText = rawText
        };
    }
    
    public Guid Id { get; private set; }
    public Guid ReviewId { get; private set; }
    public Guid ToClientId { get; private set; }
    public string RawText { get; private set; } = string.Empty;
}