using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoxAddListener : MonoBehaviour
{
    Button button;
    GameObject backGround;
    GameObject rawImage;
    GameObject inventory;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        StartCoroutine(GetUi());
    }

    IEnumerator GetUi()
    {
        yield return new WaitForEndOfFrame();
        backGround = GameObject.FindGameObjectWithTag("UiCanvas").transform.Find("CraftingUi").GetChild(0).gameObject;
        rawImage = GameObject.FindGameObjectWithTag("UiCanvas").transform.Find("CraftingUi").GetChild(1).gameObject;
        inventory = GameObject.FindGameObjectWithTag("UiCanvas").transform.Find("InventoryUi").gameObject;

        button.onClick.AddListener(() =>
        {
            inventory.GetComponent<InventoryAnimation>().CraftingAnimation();
            OnCraftingPanel(true);
        });
    }

    public void OnCraftingPanel(bool set)
    {
        backGround.SetActive(true);
        rawImage.SetActive(true);
    }
}