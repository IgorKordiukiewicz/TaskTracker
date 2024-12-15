using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Infrastructure.Converters;

internal class JsonConverter<T> : ValueConverter<T, string>
{
    public JsonConverter()
        : base(x => JsonSerializer.Serialize(x, (JsonSerializerOptions?)null), x => JsonSerializer.Deserialize<T>(x, (JsonSerializerOptions?)null)!)
    { }
}
