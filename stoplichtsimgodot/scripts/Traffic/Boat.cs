namespace StoplichtSimGodot.scripts;

public partial class Boat : Vehicle
{
	public Boat()
	{
		Speed = 10;
		RoadToUse = "Water";
		IsMoving = true;
		SpawnChance = 0.5f;
	}
}
