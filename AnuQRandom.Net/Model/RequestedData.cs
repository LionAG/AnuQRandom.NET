using Newtonsoft.Json;

namespace AnuQRandom.Model
{
    public class RequestedData
    {
        [JsonConstructor]
        public RequestedData([JsonProperty("type")] string type,
                             [JsonProperty("length")] int length,
                             [JsonProperty("size")] int size,
                             [JsonProperty("data")] List<string> data,
                             [JsonProperty("success")] bool success)
        {
            Type = type;
            Length = length;
            Size = size;
            Data = data;
            Success = success;
        }

        [JsonProperty("type")]
        public string Type { get; }

        [JsonProperty("length")]
        public int Length { get; }

        [JsonProperty("size")]
        public int Size { get; }

        [JsonProperty("data")]
        public IReadOnlyList<string> Data { get; }

        [JsonProperty("success")]
        public bool Success { get; }

        public IReadOnlyList<int> DataNumbers => Data.Select(s => int.TryParse(s, out int number) ? number : (int?)null)
                                                     .Where(n => n.HasValue)
                                                     .Select(n => n.Value)
                                                     .ToList();
    }
}