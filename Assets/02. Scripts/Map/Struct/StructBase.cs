using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public abstract class StructBase: MonoBehaviour
{
    [SerializeField] Renderer rend;
    [SerializeField] Material cloakingMaterial;

    protected StructData data;

    protected List<Tile> colleagues;

    protected List<TileBase> colleagueBases;

    protected abstract string GetCode();

    public virtual void Init(List<Tile> _colleagueList)
    {
        data = App.Data.Game.structData[GetCode()];

        colleagues = _colleagueList;
        colleagueBases = colleagues
        .Select(x => ((GameObject)x.GameEntity).GetComponent<TileBase>()).ToList();
    }

    public virtual void DetectStruct()
    {
        App.Manager.UI.GetPanel<PagePanel>().SetSelectPage("structureSelect", this);
    }

    public virtual void YesFunc()
    {
        foreach (var tileBase in colleagueBases)
        {
            tileBase.isAccessable = true;
        }

        App.Manager.Map.MovePathDelete();
        App.Manager.Map.GetUnit<EnemyUnit>().SpawnStructureZombies(colleagues);

        colleagueBases.ForEach(tile => tile.UpdateResource());

        int randomInt = UnityEngine.Random.Range(0, colleagueBases.Count);
        colleagueBases[randomInt].SetSpecialResource(new Resource(data.Item, data.Count));

        FadeIn();

        App.Manager.UI.GetPanel<PagePanel>().CreateSelectDialogueRunner("sequence");
    }

    public abstract void NoFunc();

    protected void FadeIn()
    {
        rend.material = cloakingMaterial;
    }
}