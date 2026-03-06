namespace DotnetApi.Domains.Entities;

public class Client
{
    private Client() {}

    public static Client Create(string username, string email, string phoneNumber)
    {
        if(string.IsNullOrEmpty(username))
        {
            throw new ArgumentNullException(nameof(username));
        }
        if(string.IsNullOrEmpty(email))
        {
            throw new ArgumentNullException(nameof(email));
        }
        if(string.IsNullOrEmpty(phoneNumber))
        {
            throw new ArgumentNullException(nameof(phoneNumber));
        }

        return new Client
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PhoneNumber = phoneNumber
        };
    }
    
    public Guid Id { get; private set; }
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
}