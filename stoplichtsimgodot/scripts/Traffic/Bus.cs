namespace StoplichtSimGodot.scripts;

public partial class Bus : Vehicle
{
	public Bus()
	{
		Speed = 50;
		RoadToUse = "MainRoad";
		IsMoving = true;
		SpawnChance = 5f;
	}
}
