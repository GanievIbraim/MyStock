using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyStock.Json
{
    /// <summary>
    /// Сериализует/десериализует любой enum как строку в camelCase без поддержки числовых значений.
    /// </summary>
    public class JsonCamelCaseEnumConverter : JsonStringEnumConverter
    {
        public JsonCamelCaseEnumConverter()
            : base(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
        {
        }
    }
}
