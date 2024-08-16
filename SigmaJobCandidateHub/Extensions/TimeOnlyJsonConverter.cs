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

            var timeString = reader.GetString();
            if (string.IsNullOrEmpty(timeString))
            {
                return null;
            }

            return TimeOnly.Parse(timeString);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString("HH:mm"));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
