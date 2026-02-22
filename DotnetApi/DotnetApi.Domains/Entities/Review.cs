namespace DotnetApi.Domains.Entities;

public class Review
{

    private Review() { }

    public static Review Create(Guid businessId, string rawText, string submitedOn)
    {
        if(businessId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        if (string.IsNullOrEmpty(rawText))
        {
            throw new ArgumentNullException(nameof(rawText));
        }

        if (string.IsNullOrEmpty(submitedOn))
        {
            throw new ArgumentNullException(nameof(submitedOn));
        }

        return new Review
        {
            Id = new Guid(),
            BusinessId = businessId,
            RawText = rawText,
            SubmitedOn = submitedOn,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public string RawText { get; set; } = string.Empty;
    public string SubmitedOn { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}