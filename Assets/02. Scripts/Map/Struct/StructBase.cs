using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public abstract class StructBase: MonoBehaviour
{
    [SerializeField] Material cloakingMaterial;

    public StructData Data { get; private set; }
    protected List<Tile> colleagues;
    protected List<TileBase> colleagueBases;
    protected PagePanel page;

    private Renderer render;

    protected abstract string GetCode();

    public void SetData()
    {
        Data = App.Data.Game.structData[GetCode()];
    }

    public virtual void Init(List<Tile> _colleagueList)
    {
        render = transform.GetChild(0).GetComponent<Renderer>();
        page = App.Manager.UI.GetPanel<PagePanel>();

        colleagues = _colleagueList;
        colleagueBases = colleagues.Select(x => x.Ctrl).ToList();
    }

    public virtual void DetectStruct()
    {
        page.SetNextPage(PageType.Select, "STR_SELECT_STRUCT", App.Data.Game.GetString(Data.Name));
    }

    public virtual void YesFunc()
    {
        page.SetNextPage(PageType.Result, "STR_RESULT_STRUCT_YES", App.Data.Game.GetString(Data.Name));
    }

    public virtual void NoFunc()
    {
        page.SetNextPage(PageType.Result, "STR_RESULT_STRUCT_NO", App.Data.Game.GetString(Data.Name));
    }

    protected void SetCanAccess()
    {
        foreach (var tileBase in colleagueBases)
        {
            tileBase.isAccessable = true;
        }

        App.Manager.Map.GetUnit<EnemyUnit>().SpawnStructureZombies(colleagues);

        colleagueBases.ForEach(tile => tile.UpdateResource());

        int randomInt = Random.Range(0, colleagueBases.Count);
        colleagueBases[randomInt].SetSpecialResource(new Resource(Data.Item, Data.Count));

        FadeIn();
    }

    protected void FadeIn()
    {
        render.material = cloakingMaterial;
    }
}