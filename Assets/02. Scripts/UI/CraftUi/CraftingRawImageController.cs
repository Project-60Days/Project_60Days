using UnityEngine;


public class CraftingRawImageController : MonoBehaviour
{
    [SerializeField] GameObject targetObject;

    void Awake()
    {
        SlotBase.CraftItemClick += ChangerTarget;
        //CraftSlot.CraftItemClick += DestroyObject;

        targetObject = GameObject.FindWithTag("RenderTextureObject");
    }

    void OnDestroy()
    {
        SlotBase.CraftItemClick -= ChangerTarget;
        //CraftSlot.CraftItemClick -= DestroyObject;
    }

    public void ChangerTarget(Sprite itemSprite)
    {
        if (itemSprite == null)
        {
            DestroyObject();
            return;
        }

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