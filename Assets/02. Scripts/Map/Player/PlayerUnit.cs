using UnityEngine;
using Hexamap;

public class PlayerUnit : MapBase
{
    [SerializeField] Transform mapParentTransform;
    [SerializeField] GameObject prefab;

    private Player player;

    public Transform PlayerTransform => player.transform;

    int cloakingDay = 0;
    bool isCloaking = false;

    public override void Init()
    {
        base.Init();

        Vector3 spawnPos = tile.gameObject.transform.position;
        spawnPos.y += 0.7f;

        var playerObject = Instantiate(prefab, spawnPos,
            Quaternion.Euler(0, -90, 0));
        player = playerObject.GetComponent<Player>();
        player.transform.parent = mapParentTransform;
    }

    public override void ReInit()
    {
        if (isCloaking) 
            CheckCloaking();

        player.Move(tile);
    }

    public void SetCloaking(int num)
    {
        player.SetCloaking(true);
        cloakingDay = App.Manager.Game.dayCount + num;
        isCloaking = true;
    }

    private void CheckCloaking()
    {
        if (App.Manager.Game.dayCount == cloakingDay)
        {
            player.SetCloaking(false);
            App.Manager.Test.UnsetCloaking();
        }
    }
}
