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
		pathFollow.Progress = 0.0f; // Begin bij het startpunt van het pad
		isMoving = true;
	}

	public void Process(float delta)
	{
		if (isMoving && pathFollow != null)
		{
			// Verhoog de voortgang op basis van de snelheid en tijd
			pathFollow.Progress += speed * delta / 1;

			// Herstart de voortgang als het einde van het pad is bereikt
			if (pathFollow.Progress >= 1.0f)
			{
				pathFollow.Progress = 0.0f;
			}
		}
	}
}
