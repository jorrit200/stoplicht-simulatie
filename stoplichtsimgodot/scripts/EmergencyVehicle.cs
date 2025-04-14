namespace StoplichtSimGodot.scripts;

public partial class EmergencyVehicle : Vehicle
{
	public EmergencyVehicle()
	{
		speed = 100;
		roadToUse = "MainRoad";
		isMoving = true;
		spawnChance = 1;
	}
}