using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StoplichtSimGodot.dto;

public class SensorenRijbaan
{
    [JsonPropertyName("voor")] 
    public bool Voor { get; set; }

    [JsonPropertyName("achter")] 
    public bool Achter { get; set; }
}

public class SensorStatusData: Dictionary<string, SensorenRijbaan>;