using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApiHttpExample;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString() ?? string.Empty;

        return DateOnly.ParseExact(dateString, DateFormat, System.Globalization.CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, System.Globalization.CultureInfo.InvariantCulture));
    }
}
