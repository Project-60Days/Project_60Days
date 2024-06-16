public class Explorer : DroneBase
{
    public override DroneType GetDroneType() => DroneType.Explorer;

    private void OnDestroy()
    {
        App.Manager.Asset.Fog.Remove();
    }
}
