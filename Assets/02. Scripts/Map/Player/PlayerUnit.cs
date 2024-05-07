using UnityEngine;
using Hexamap;

public class PlayerUnit : MapBase
{
    [SerializeField] Transform mapParentTransform;
    [SerializeField] GameObject prefab;

    private Player player;

    public Transform PlayerTransform => player.transform;

    int cloakingDay = 0;

    public override void Init()
    {
        Vector3 spawnPos = App.Manager.Map.TileToTileController(App.Manager.Map.GetTileFromCoords(new Coords(0, 0))).transform.position;
        spawnPos.y += 0.7f;

        var playerObject = Instantiate(prefab, spawnPos,
            Quaternion.Euler(0, -90, 0));
        player = playerObject.GetComponent<Player>();
        player.transform.parent = mapParentTransform;

        App.Manager.Map.UpdateCurrentTile(App.Manager.Map.TileToTileController(App.Manager.Map.GetTileFromCoords(new Coords(0, 0))));
    }

    public override void ReInit()
    {
        CheckCloaking();

        player.Move(App.Manager.Map.targetTile);
    }

    public void SetCloaking(int num)
    {
        player.SetCloaking(true);
        cloakingDay = App.Manager.Game.dayCount + num;
    }

    private void CheckCloaking()
    {
        if (App.Manager.Game.dayCount == cloakingDay)
        {
            player.SetCloaking(false);
            App.Manager.Map.UnsetCloaking();
        }
    }
}
