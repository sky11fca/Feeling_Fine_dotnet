using DotnetApi.Domains.Enums;

namespace DotnetApi.Domains.Entities;

public class Review
{

    private Review() { }

    public static Review Create(Guid businessId, Guid clientId, decimal rating, RatingType ratingType, string rawText, string submittedOn)
    {
        if(businessId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(businessId));
        }

        if (string.IsNullOrEmpty(rawText))
        {
            throw new ArgumentNullException(nameof(rawText));
        }

        if (string.IsNullOrEmpty(submittedOn))
        {
            throw new ArgumentNullException(nameof(submittedOn));
        }

        return new Review
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            Rating = rating,
            RatingType = ratingType,
            BusinessId = businessId,
            RawText = rawText,
            SubmittedOn = submittedOn,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public Guid Id { get; private set; }
    public Guid BusinessId { get; private set; }
    public Guid ClientId { get; private set; }
    public decimal Rating { get; private set; }
    public RatingType RatingType { get; private set; }
    public string RawText { get; private set; } = string.Empty;
    public string SubmittedOn { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}