using Web.Common;

namespace Web.Domain.Account;

public record Account : BaseEntity
{
    public long AccountUid { get; set; }
    public string AccountId { get; set; } = string.Empty;

    public string AccountPassword { get; init; } = string.Empty;

    public string AccountName { get; init; } = string.Empty;

    public short AccountLevel { get; init; } = 1;

    public int AccountExp { get; init; }
}