using UnityEngine;


public class CraftingRawImageController : MonoBehaviour
{
    [SerializeField] GameObject targetObject;

    void Awake()
    {
        ItemSlot.CraftItemClick += ChangerTarget;
        BlueprintSlot.CraftItemClick += ChangerTarget;
        CraftSlot.CraftItemClick += DestroyObject;

        targetObject = GameObject.FindWithTag("RenderTextureObject");
    }

    void OnDestroy()
    {
        ItemSlot.CraftItemClick -= ChangerTarget;
        BlueprintSlot.CraftItemClick -= ChangerTarget;
        CraftSlot.CraftItemClick -= DestroyObject;
    }

    public void ChangerTarget(Sprite itemSprite)
    {
        if (itemSprite == null)
        {
            DestroyObject();
            return;
        }

        App.Manager.UI.GetCraftingUiController().TurnOnHologram();
        targetObject.SetActive(true);
        targetObject.GetComponent<SpriteRenderer>().sprite = itemSprite;
    }

    public void DestroyObject()
    {
        App.Manager.UI.GetCraftingUiController().TurnOffHologram();
        targetObject.SetActive(false);
    }
}