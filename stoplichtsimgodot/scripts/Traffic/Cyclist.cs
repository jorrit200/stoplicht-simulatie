namespace StoplichtSimGodot.scripts;

public partial class Cyclist : Vehicle
{
	public Cyclist()
	{
		Speed = 20;
		RoadToUse = "BikeRoad";
		IsMoving = true;
		SpawnChance = 10;
	}
}
