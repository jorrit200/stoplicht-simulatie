using Godot;
using System;

public partial class Vehicle : Node2D
{
	public float speed;
	public string roadToUse;
	public bool isMoving;
	public float spawnChance;
	
	private PathFollow2D pathFollow;

	public void StartMoving(PathFollow2D followPath)
	{
		pathFollow = followPath;
		isMoving = true;
	}

	public override void _Process(double delta)
	{
		if (isMoving && pathFollow != null)
		{
			pathFollow.Progress += speed * (float)delta;
			
			if(pathFollow.ProgressRatio >= 0.99f)
			{
				QueueFree();
			}
		}
	}
}
