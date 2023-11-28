using UnityEngine;


public class CraftingRawImageController : MonoBehaviour
{
    [SerializeField] GameObject targetObject;

    void Awake()
    {
        ItemSlot.CraftItemClick += ChangerTarget;
        CraftSlot.CraftItemClick += DestroyObject;

        targetObject = GameObject.FindWithTag("RenderTextureObject");
    }

    void OnDestroy()
    {
        ItemSlot.CraftItemClick -= ChangerTarget;
        CraftSlot.CraftItemClick -= DestroyObject;
    }

    public void ChangerTarget(Sprite itemSprite)
    {
        if (itemSprite == null) return;
        UIManager.instance.GetCraftingUiController().TurnOnHologram();
        targetObject.SetActive(true);
        targetObject.GetComponent<SpriteRenderer>().sprite = itemSprite;
    }

    public void DestroyObject()
    {
        UIManager.instance.GetCraftingUiController().TurnOffHologram();
        targetObject.SetActive(false);
    }
}