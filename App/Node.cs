using Newtonsoft.Json;

namespace ServiceRegistry.ConnectedNodes
{
    public enum NodeType
    {
        Repository,
        Miner,
    }
    public class Node
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? HostName { get; set; }   // Only here for when we send as a list
        public string Label { get; set; }       // Some name like "The cool miner"
        public NodeType Type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Access { get; set; }      // Not sure if required
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Param>? Parameters { get; set; }
    }

    public class Param
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Value { get; set; }
    }
}
