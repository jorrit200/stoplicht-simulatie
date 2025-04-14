namespace StoplichtSimGodot.scripts;

public partial class EmergencyVehicle : Vehicle
{
    public EmergencyVehicle()
    {
        Speed = 100;
        RoadToUse = "MainRoad";
        IsMoving = true;
        SpawnChance = 0;
    }
}