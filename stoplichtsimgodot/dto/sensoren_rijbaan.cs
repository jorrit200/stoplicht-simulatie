using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Sensoren_Rijbaan
{
	[JsonPropertyName("voor")]
	public bool Voor { get; set; }

	[JsonPropertyName("achter")]
	public bool Achter { get; set; }
}

public class SensorStatusData : Dictionary<string, Sensoren_Rijbaan>
{
}
