using System.Text.Json.Serialization;
using System.Text.Json;

namespace SigmaJobCandidateHub.Extensions
{
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly?>
    {
        public override TimeOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                int hour = 0, minute = 0;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        string propertyName = reader.GetString();
                        reader.Read(); // Move to the value
                        if (propertyName == "hour")
                        {
                            hour = reader.GetInt32();
                        }
                        else if (propertyName == "minute")
                        {
                            minute = reader.GetInt32();
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return new TimeOnly(hour, minute);
                    }
                }
            }

            throw new JsonException("Invalid JSON format for TimeOnly.");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStartObject();
                writer.WriteNumber("hour", value.Value.Hour);
                writer.WriteNumber("minute", value.Value.Minute);
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }

}
