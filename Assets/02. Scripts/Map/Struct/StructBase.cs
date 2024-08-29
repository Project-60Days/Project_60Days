using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Hexamap;

public abstract class StructBase: MonoBehaviour
{
    [SerializeField] Material cloakingMaterial;

    protected StructData data;
    protected List<Tile> colleagues;
    protected List<TileBase> colleagueBases;
    protected PagePanel page;

    private Renderer render;

    protected abstract string GetCode();

    public virtual void Init(List<Tile> _colleagueList)
    {
        data = App.Data.Game.structData[GetCode()];

        render = transform.GetChild(0).GetComponent<Renderer>();
        page = App.Manager.UI.GetPanel<PagePanel>();

        colleagues = _colleagueList;
        colleagueBases = colleagues.Select(x => x.Ctrl).ToList();
    }

    public virtual void DetectStruct()
    {
        page.SetNextPage(PageType.Select, "STR_SELECT_STRUCT", App.Data.Game.GetString(data.Name));
    }

    public virtual void YesFunc()
    {
        page.SetNextPage(PageType.Result, "STR_RESULT_STRUCT_YES", App.Data.Game.GetString(data.Name));
    }

    public virtual void NoFunc()
    {
        page.SetNextPage(PageType.Result, "STR_RESULT_STRUCT_NO", App.Data.Game.GetString(data.Name));
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
        colleagueBases[randomInt].SetSpecialResource(new Resource(data.Item, data.Count));

        FadeIn();
    }

    protected void FadeIn()
    {
        render.material = cloakingMaterial;
    }
}