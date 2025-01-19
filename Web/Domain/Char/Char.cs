using Web.Common;

namespace Web.Domain.Char;

public record Char : BaseEntity
{
    public long AccountUid { get; set; }

    public long CharUid { get; set; }

    public string CharId { get; init; } = string.Empty;

    public string CharPassword { get; init; } = string.Empty;

    public string CharName { get; init; } = string.Empty;

    public int CharLevel { get; init; } = 1;

    public int CharExp { get; init; }
}