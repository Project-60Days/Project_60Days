using System.Linq;
using UnityEngine;
using TMPro;

public class BlueprintCtrl : ModeCtrl
{
    [SerializeField] GameObject slotPrefab;

    public BlueprintSlot[] blueprintSlots;

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

    ItemBase GetItemByItemCode(string _itemCode)
        => itemData.Find(x => x.data.Code == _itemCode);
}
