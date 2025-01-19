using System.Text.Json.Serialization;

namespace Web.Common;

public abstract record BaseEntity
{
    [JsonIgnore]
    public DateTime CreatedAt { get; init; } = DateTime.Now;

    [JsonIgnore]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}