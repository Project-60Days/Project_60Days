using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;
using FischlWorks_FogWar;

public class PlayerUnit : MapBase
{
    [Header("ÄÄÆ÷³ÍÆ®")]
    [Space(5f)]
    [SerializeField]
    HexamapController hexaMap;

    [SerializeField] Transform mapParentTransform;
    [SerializeField] GameObject prefab;
    public csFogWar fog;
    public Player player { get; private set; }

    public int PlayerMoveRange => player.MoveRange;
    public bool IsMovePathSaved() => player.MovePath != null;

    public Vector3 PlayerTransform => player.transform.position;

    public override void Init()
    {
        Vector3 spawnPos = App.Manager.Map.TileToTileController(hexaMap.Map.GetTileFromCoords(new Coords(0, 0))).transform.position;
        spawnPos.y += 0.7f;

        var playerObject = Instantiate(prefab, spawnPos,
            Quaternion.Euler(0, -90, 0));
        player = playerObject.GetComponent<Player>();
        player.transform.parent = mapParentTransform;
        player.InputDefaultData(data.playerMovementPoint, data.durability);

        App.Manager.Map.UpdateCurrentTile(App.Manager.Map.TileToTileController(hexaMap.Map.GetTileFromCoords(new Coords(0, 0))));

        SpawnFog();
        //player.TileEffectCheck();
    }

    private void SpawnFog()
    {
        fog.InitializeMapControllerObjects(player.gameObject, data.fogSightRange);
    }

    public override void ReInit()
    {
        player.ChangeClockBuffDuration();

        if (IsMovePathSaved())
        {
            player.ActionDecision(App.Manager.Map.targetTile);
        }

        player.SetHealth(true);
        player.TileEffectCheck();
    }
}
