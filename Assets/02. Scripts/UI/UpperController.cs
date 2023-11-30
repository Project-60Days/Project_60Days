using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpperController : MonoBehaviour
{
    [SerializeField] ItemSO itemSO;

    [Header("ItemCountText")]
    [SerializeField] TextMeshProUGUI steelText;
    [SerializeField] TextMeshProUGUI carbonText;
    [SerializeField] TextMeshProUGUI plasmaText;
    [SerializeField] TextMeshProUGUI bulletText;
    [SerializeField] TextMeshProUGUI durabilityText;
    [SerializeField] TextMeshProUGUI powderText;
    [SerializeField] TextMeshProUGUI gasText;
    [SerializeField] TextMeshProUGUI rubberText;

    ItemBase steel;
    ItemBase carbon;
    ItemBase plasma;
    ItemBase powder;
    ItemBase gas;
    ItemBase rubber;
    ItemBase bullet;

    void Start()
    {
        foreach (var item in itemSO.items)
        {
            if (item.data.Code == "ITEM_STEEL") steel = item;
            else if (item.data.Code == "ITEM_CARBON") carbon = item;
            else if (item.data.Code == "ITEM_PLASMA") plasma = item;
            else if (item.data.Code == "ITEM_POWDER") powder = item;
            else if (item.data.Code == "ITEM_GAS") gas = item;
            else if (item.data.Code == "ITEM_RUBBER") rubber = item;
            else if (item.data.Code == "ITEM_BULLET") bullet = item;
        }

        UpdateBar();
    }

    public void UpdateBar()
    {
        steelText.text = steel.itemCount.ToString("D3");
        carbonText.text = carbon.itemCount.ToString("D3");
        plasmaText.text = plasma.itemCount.ToString("D3");
        bulletText.text = powder.itemCount.ToString("D3");
        powderText.text = gas.itemCount.ToString("D3");
        gasText.text = rubber.itemCount.ToString("D3");
        rubberText.text = bullet.itemCount.ToString("D3");
    }
}
