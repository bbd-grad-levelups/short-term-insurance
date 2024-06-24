using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Helpers;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    /// <summary>
    /// Reads a DateTime object from a JSON reader.
    /// </summary>
    /// <param name="reader">The JSON reader to read from.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">The JSON serializer options.</param>
    /// <returns>The DateTime object read from the JSON reader.</returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateTimeFromJson = reader.GetString()!;

        return DateTime.ParseExact(dateTimeFromJson, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Writes the specified DateTime value to the provided Utf8JsonWriter.
    /// We are not catualy implemeting this as it seens like we can write a date to a string without this nonsense.
    /// </summary>
    /// <param name="writer">The Utf8JsonWriter to write the DateTime value to.</param>
    /// <param name="value">The DateTime value to write.</param>
    /// <param name="options">The JsonSerializerOptions to use for serialization.</param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}