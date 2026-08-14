using MarketSphere.Domain.Entities.OrganizationSecurity;

namespace MarketSphere.Domain.Entities.Infrastructure;

public sealed class IdempotencyRequest
{
    public int IdempotencyRequestID { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public int? UserID { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public string RequestHash { get; set; } = string.Empty;
    public int? ResponseStatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public User? User { get; set; }
}
