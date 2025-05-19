namespace StoplichtSimGodot.scripts;

public partial class Boat : Vehicle
{
	public Boat()
	{
		Speed = 20;
		RoadToUse = "Water";
		IsMoving = true;
		SpawnChance = 1f;
	}
}
