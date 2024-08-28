using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class BlueprintCtrl : ModeCtrl
{
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Transform slotParent;

    public override BenchType GetModeType() => BenchType.Blueprint;

    public BlueprintSlot[] blueprintSlots;
    private List<ItemCombineData> itemCombineData;

    public override void Init()
    {
        base.Init();

        itemCombineData = App.Data.Game.itemCombineData.Values.ToList();

        blueprintSlots = GetComponentsInChildren<BlueprintSlot>(includeInactive: true);
    }

    public override void InitSlots()
    {
        for (int i = 0; i < slotParent.childCount; i++)
            Destroy(slotParent.GetChild(i).gameObject);
    }

    public override void Exit()
    {
        base.Exit();

        InitSlots();
    }

    void OnEnable()
    {
        UpdateBlueprint();
    }

    public void UpdateBlueprint()
    {
        foreach (var slot in blueprintSlots)
        {
            slot.CheckShowCondition();
        }
    }

    public void ShowItemBlueprint(ItemBase _item)
    {
        InitSlots();

        string[] blueprintCodes = GetItemCombineCodes(_item);
        if (blueprintCodes == null) return;

        bool isFirst = true;

        for (int i = 0; i < blueprintCodes.Length - 1; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);

            if (blueprintCodes[i] == "-1")
            {
                obj.GetComponentInChildren<CraftSlot>().item = GetItemByItemCode(blueprintCodes[blueprintCodes.Length - 1]);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = "=";
            }
            else
                obj.GetComponentInChildren<CraftSlot>().item = GetItemByItemCode(blueprintCodes[i]);

            if (isFirst == true)
            {
                obj.transform.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
                isFirst = false;
            }

            obj.GetComponentInChildren<CraftSlot>().enabled = false;
        }
    }

    public string[] GetItemCombineCodes(ItemBase _item)
    {
        foreach (ItemCombineData combineData in itemCombineData)
        {
            if (combineData.Result == _item.data.Code)
                return GetCombinationCodes(combineData);
        }

        return null;
    }

    private string[] GetCombinationCodes(ItemCombineData _combineData)
    {
        string[] codes = new string[4];

        codes[0] = _combineData.Material_1;
        codes[1] = _combineData.Material_2;
        codes[2] = _combineData.Material_3;
        codes[3] = _combineData.Result;

        return codes;
    }

    ItemBase GetItemByItemCode(string _itemCode)
        => itemData.Find(x => x.data.Code == _itemCode);
}
