namespace DotnetApi.Domains.Entities;

public class Business
{
    private Business() {}

    public static Business Create(string name, string industry)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (string.IsNullOrEmpty(industry))
        {
            throw new ArgumentNullException(nameof(industry));
        }

        return new Business
        {
            Id = Guid.NewGuid(),
            Name = name,
            Industry = industry,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Industry { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}