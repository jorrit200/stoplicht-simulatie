using System.Collections.Generic;
using Godot;

namespace StoplichtSimGodot.scripts;

public partial class TrafficLightMaster : ZMQSubscriber
{
    private Dictionary<string, TrafficLight> subscribedLights = new();

    public void BindTrafficLight(TrafficLight light)
    {
        string lightname = light.Name;

        lightname = lightname.Replace("_", ".");

        subscribedLights.Add(lightname, light);
    }

    private void OnMessage((string topic, string message) message)
    {
        GD.Print("Master ontvangt dingen");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        DoOnMessage(OnMessage);
    }
}