public class Explorer : DroneBase
{
    public override DroneType GetDroneType() => DroneType.Explorer;

    private void OnDestroy()
    {
        App.Manager.Map.fog.Remove();
    }
}
