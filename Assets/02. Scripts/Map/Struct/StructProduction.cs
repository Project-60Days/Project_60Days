using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hexamap;

public class StructProduction : StructBase
{
    protected override string GetCode() => "STRUCT_PRODUCTION";

    public override void YesFunc()
    {
        for (var index = 0; index < colleagueBases.Count; index++)
        {
            var tile = colleagueBases[index];
            tile.structure.AllowAccess();
        }

        App.Manager.Map.MovePathDelete();
        App.Manager.Map.GetUnit<EnemyUnit>().SpawnStructureZombies(colleagues);

        FadeIn();
        colleagueBases.ForEach(tile => tile.UpdateResource());
        isAccessible = false;

        int randomInt = Random.Range(0, colleagueBases.Count);
        var randomTile = colleagueBases[randomInt];

        if (randomTile.structure == null)
            Debug.Log("비어있음");

        randomTile.SetSpecialResource();

        isAccessible = true;
        App.Manager.UI.GetPanel<PagePanel>().CreateSelectDialogueRunner("sequence");
        App.Manager.UI.GetPanel<PagePanel>().isClickYesBtnInProductionStructure = true;
    }

    public override void NoFunc()
    {

    }
}