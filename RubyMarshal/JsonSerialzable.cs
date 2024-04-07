using System.Text.Json.Nodes;

namespace RubyMarshal
{
    public interface IJsonSerialzable
    {
        public abstract JsonNode? ToJson();
    }
}
