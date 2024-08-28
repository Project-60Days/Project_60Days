using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintCtrl : ModeCtrl
{
    public override BenchType GetModeType() => BenchType.Blueprint;

    [SerializeField] CraftSlot[] craftSlots;
    [SerializeField] CraftSlot resultSlot;

    private Dictionary<string, ItemCombineData> itemCombineDic;
    private BlueprintSlot[] blueprintSlots;

    public override void Init()
    {
        base.Init();

        itemCombineDic = App.Data.Game.itemCombineData.Values.ToDictionary(x => x.Result);

        blueprintSlots = GetComponentsInChildren<BlueprintSlot>(includeInactive: true);
    }

    public override void Exit()
    {
        base.Exit();

        ResetSlots();
    }

    private void OnEnable()
    {
        UpdateBlueprint();
    }

    public void UpdateBlueprint()
    {
        if (blueprintSlots == null) return;

        foreach (var slot in blueprintSlots)
        {
            slot.CheckShowCondition();
        }
    }

    public override void ResetSlots()
    {
        foreach (var slot in craftSlots)
        {
            slot.ResetItem();
        }

        resultSlot.ResetItem();
    }

    public void UpdateSlots(ItemBase _item)
    {
        ResetSlots();

        if (!itemCombineDic.TryGetValue(_item.Code, out var combineData)) return;

        string[] blueprintCodes = { combineData.Material_1, combineData.Material_2, combineData.Material_3, combineData.Result };

        for (int i = 0; i < blueprintCodes.Length - 1; i++)
        {
            if (blueprintCodes[i] == "-1") continue;

            var item = itemData[blueprintCodes[i]];
            craftSlots[i].SetItem(item);
        }

        resultSlot.SetItem(itemData[combineData.Result]);
    }
}
