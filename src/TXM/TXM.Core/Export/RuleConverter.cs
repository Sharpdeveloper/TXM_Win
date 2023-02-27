using System.Text.Json;
using System.Text.Json.Serialization;

namespace TXM.Core.Export;

public class RuleConverter : JsonConverter<AbstractRules>
{
    public override AbstractRules Read(
        ref Utf8JsonReader reader
        , Type typeToConvert
        , JsonSerializerOptions options) =>
        AbstractRules.GetRule(reader.GetString());

    public override void Write(
        Utf8JsonWriter writer
        , AbstractRules rule
        , JsonSerializerOptions options) =>
        writer.WriteStringValue(rule.GetName());
}