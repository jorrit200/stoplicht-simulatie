using System.Collections.Generic;
using Godot;
using StoplichtSimGodot.interfaces;
using StoplichtSimGodot.dto; // Voor evt. extra struct of DTO
using System.Text.Json;
using System.Linq;
using System;

namespace StoplichtSimGodot.scripts;

public class VoorrangsVoertuigEntry
{
	public Vehicle vehicle { get; set; }
	public string baan { get; set; }
	public ulong simulatie_tijd_ms { get; set; }
	public int prioriteit { get; set; }
}

public partial class VoorrangsQueueManager : Node2D
{
	public List<VoorrangsVoertuigEntry> _queue = new();
	private IMessagePublisher _publisher;
	private string _topic = "voorrangsvoertuig";
	public static VoorrangsQueueManager Instance { get; private set; }

	public void InjectPublisher(IMessagePublisher publisher)
	{
		_publisher = publisher;
	}

	public override void _Ready()
	{
		Instance = this;
	}

	public void VoegToe(Vehicle voertuig, string lane, int prio)
	{
		var entry = new VoorrangsVoertuigEntry
		{
			vehicle = voertuig,
			baan = ParseLaneId(lane),
			prioriteit = prio,
			simulatie_tijd_ms = Time.GetTicksMsec()
		};

		_queue.Add(entry);
		PubliceerQueue();
	}

	public void Verwijder(Vehicle voertuig)
	{
		var removed = _queue.RemoveAll(q => q.vehicle == voertuig) > 0;
		if (removed)
		{
			PubliceerQueue();
		}
	}

	public void PubliceerQueue()
	{
		var simplified = _queue.Select(q => new
		{
			baan = q.baan,
			simulatie_tijd_ms = q.simulatie_tijd_ms,
			prioriteit = q.prioriteit
		});


		var json = JsonSerializer.Serialize(new { queue = simplified });
		_publisher!.Send(_topic, json);
	}

	public static string ParseLaneId(string laneId)
	{
		if (!laneId.StartsWith("Path2D"))
			throw new ArgumentException("Ongeldig laneId-formaat: " + laneId);

		string stripped = laneId.Substring("Path2D".Length);

		if (stripped.Contains('_'))
		{
			var parts = stripped.Split('_');
			return $"{parts[0]}.{parts[1]}";
		}
		else
		{
			return $"{stripped}.1";
		}
	}
}
