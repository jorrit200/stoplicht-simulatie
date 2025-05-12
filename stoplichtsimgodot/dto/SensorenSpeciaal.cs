using System.Collections.Generic;

namespace StoplichtSimGodot.dto
{
    public class SensorenSpeciaal
    {
        public bool Status { get; set; }
    }

    public class SensorenSpeciaalData : Dictionary<string, SensorenSpeciaal> { }
}