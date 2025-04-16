using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using StoplichtSimGodot.scripts;

namespace StoplichtSimGodot.dto;


public class StoplichtenDto : Dictionary<string, TrafficLightState>;