namespace StoplichtSimGodot.scripts;

public partial class Pedestrian : Vehicle
{
	public Pedestrian()
	{
		Speed = 6;
		RoadToUse = "PedRoad";
		IsMoving = true;
		SpawnChance = 5;
	}
}
