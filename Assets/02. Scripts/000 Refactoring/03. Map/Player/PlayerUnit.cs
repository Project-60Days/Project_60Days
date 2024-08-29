using UnityEngine;
using Hexamap;

public class PlayerUnit : MapBase
{
    [SerializeField] GameObject prefab;

    public Transform PlayerTransform => player.transform;

    private Player player;

    public override void Init()
    {
        base.Init();

        Vector3 spawnPos = tile.transform.position;
        spawnPos.y += 0.7f;

        var playerObject = Instantiate(prefab, spawnPos, Quaternion.Euler(0, -90, 0));
        player = playerObject.GetComponent<Player>();
        player.transform.parent = transform;
    }

    public override void ReInit()
    {
        player.Move(tile);
    }

    public void SetCloaking(bool _isActive)
    {
        player.SetCloaking(_isActive);
    }
}
