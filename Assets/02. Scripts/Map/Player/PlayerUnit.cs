using UnityEngine;
using Hexamap;

public class PlayerUnit : MapBase
{
    [SerializeField] Transform mapParentTransform;
    [SerializeField] GameObject prefab;

    public Player player { get; private set; }

    public int PlayerMoveRange => player.MoveRange;

    public Vector3 PlayerTransform => player.transform.position;

    public override void Init()
    {
        Vector3 spawnPos = App.Manager.Map.TileToTileController(App.Manager.Map.GetTileFromCoords(new Coords(0, 0))).transform.position;
        spawnPos.y += 0.7f;

        var playerObject = Instantiate(prefab, spawnPos,
            Quaternion.Euler(0, -90, 0));
        player = playerObject.GetComponent<Player>();
        player.transform.parent = mapParentTransform;
        player.InputDefaultData(data.playerMovementPoint);

        App.Manager.Map.UpdateCurrentTile(App.Manager.Map.TileToTileController(App.Manager.Map.GetTileFromCoords(new Coords(0, 0))));
    }

    public override void ReInit()
    {
        player.ChangeClockBuffDuration();

        player.ActionDecision(App.Manager.Map.targetTile);

        player.SetHealth(true);
    }
}
