using System.Text.Json;
using System.Text.Json.Serialization;
using Contracts;

namespace Events.User;

/// <summary>
/// JsonConverter for deserializing AccountRepresentation from JSON string or object
/// Handles Keycloak's nested JSON representation format
/// </summary>
public class AccountRepresentationConverter : JsonConverter<AccountRepresentation>
{
    public override AccountRepresentation? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                // Representation is a JSON string - deserialize it
                var jsonString = reader.GetString();
                if (string.IsNullOrEmpty(jsonString))
                {
                    return null;
                }
                return JsonSerializer.Deserialize<AccountRepresentation>(jsonString, options);

            case JsonTokenType.StartObject:
                // Representation is already an object - deserialize directly
                return JsonSerializer.Deserialize<AccountRepresentation>(ref reader, options);

            default:
                throw new JsonException(
                    $"Unexpected token type {reader.TokenType} when deserializing AccountRepresentation");
        }
    }

    public override void Write(Utf8JsonWriter writer, AccountRepresentation value, JsonSerializerOptions options)
    {
        // Serialize as object (for sending events)
        JsonSerializer.Serialize(writer, value, options);
    }
}
