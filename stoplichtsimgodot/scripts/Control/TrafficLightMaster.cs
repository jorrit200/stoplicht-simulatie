using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using StoplichtSimGodot.dto;

namespace StoplichtSimGodot.scripts.Control;

public partial class TrafficLightMaster : Node
{
    [Export] private ZmqSubscriber _subscriber;
    private readonly Dictionary<string, TrafficLight> _subscribedLights = new();

    public void BindTrafficLight(TrafficLight light)
    {
        GD.Print($"Binding {light.Name} to traffic light master");
        string lightName = light.Name;

        lightName = lightName.Replace("_", ".");

        _subscribedLights.Add(lightName, light);
    }

    private void OnMessage((string topic, string message) message)
    {
        if (message.topic != "stoplichten") return;

        try
        {
            var stoplichtData = JsonConvert.DeserializeObject<StoplichtenDto>(message.message);
            foreach (var (key, newState) in stoplichtData)
            {
                GD.Print(key, newState);
                var subbedLight = _subscribedLights.GetValueOrDefault(key);
                if (subbedLight is not null)
                {
                    subbedLight.State = newState;
                }
            }
        }
        catch (Exception e)
        {
            GD.Print("PARSING ERROR: ", e.Message);
        }
    }

    public override void _Ready()
    {
        _subscriber.DoOnMessage(OnMessage);
    }
}