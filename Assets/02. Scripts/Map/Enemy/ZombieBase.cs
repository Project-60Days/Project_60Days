using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hexamap;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

[SelectionBase]
public class ZombieBase : MonoBehaviour
{
    public int count { get; private set; }
    public Tile currTile { get; private set; }

    [SerializeField] GameObject[] zombieModels;

    private ValueData data;

    private Tile lastTile;

    private bool noneTileBuff;

    private int moveRange = 1;
    private bool isDebuff = false;

    private Vector3 initScale;

    public void Init(Tile tile)
    {
        data = App.Data.Game.valueData["Enemy"];

        count = (int)Random.Range(data.MinCount, data.MaxCount);
        SetModel();

        currTile = tile;
        lastTile = currTile;
        UpdateTile();

        initScale = transform.localScale;
    }

    private void SetModel()
    {
        int possibility = Mathf.FloorToInt((count - data.MinCount) / ((data.MaxCount - data.MinCount) / (zombieModels.Length - 1)));
        int modelIndex = Mathf.Clamp(possibility, 0, zombieModels.Length - 1);

        for (int i = 0; i < zombieModels.Length; i++)
        {
            zombieModels[i].SetActive(i == modelIndex);
        }
    }

    private void UpdateTile()
    {
        lastTile?.Ctrl.Base.SetEnemy(null);
        currTile?.Ctrl.Base.SetEnemy(this);
    }

    #region Move
    public void Move(Tile _playerTile)
    {
        CheckTileEffect();

        if (isDebuff) return;

        var targetTile = _playerTile;
        var targetVector = App.Manager.Map.GetUnit<PlayerUnit>().PlayerTransform.position;

        Chase(targetTile);
        transform.LookAt(new Vector3(targetVector.x, targetVector.y + 0.6f, targetVector.z));

        UpdateTile();
    }

    private void CheckTileEffect()
    {
        switch (currTile.Ctrl.Base.GetTileType())
        {
            case TileType.City:
                if (noneTileBuff == false)
                {
                    count += 5;
                    SetModel();
                    noneTileBuff = true;
                }
                break;

            case TileType.Desert:
            case TileType.Tundra:
                isDebuff = !isDebuff;
                break;
        }
    }

    private void Chase(Tile target)
    {
        var movePath = AStar.FindPath(currTile.Coords, target.Coords);

        Tile pointTile;
        Vector3 pointPos;

        if (movePath.Count == 0 && target == App.Manager.Map.tileCtrl.Model) // When the player is within 1 space, attack player
        {
            App.Manager.Game.TakeDamage(count);
        }
        else
        {
            var range = Mathf.Min(moveRange, movePath.Count);
            pointTile = App.Manager.Asset.Hexamap.Map.GetTileFromCoords(movePath[range - 1]);
            pointPos = pointTile.GameEntity.transform.position;
            pointPos.y += 0.6f;

            gameObject.transform.DOMove(pointPos, 0f);

            currTile = pointTile;
        }

        lastTile = currTile;
    }
    #endregion

    public void Sum(ZombieBase zombie)
    {
        count += zombie.count;

        SetModel();
        SizeUpCheck();
    }

    private void SizeUpCheck()
    {
        var scale = (count / 10) * 0.1f;

        if (scale > 0.7f) return;

        transform.localScale = initScale + new Vector3(scale, scale, scale);
    }
}