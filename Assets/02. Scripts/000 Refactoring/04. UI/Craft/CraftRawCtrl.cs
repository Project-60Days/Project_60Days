using UnityEngine;


public class CraftRawCtrl : MonoBehaviour
{
    private GameObject targetObject;
    private SpriteRenderer targetSprite;

    private void Start()
    {
        ItemSlot.CraftItemClick += ChangeTarget;
        BlueprintSlot.CraftItemClick += ChangeTarget;
        CraftSlot.CraftItemClick += InitTarget;

        targetObject = GameObject.FindWithTag("RenderTextureObject");
        targetSprite = targetObject.GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        ItemSlot.CraftItemClick -= ChangeTarget;
        BlueprintSlot.CraftItemClick -= ChangeTarget;
        CraftSlot.CraftItemClick -= InitTarget;
    }

    public void InitTarget()
    {
        App.Manager.UI.GetPanel<CraftPanel>().TurnHologram(false);
        targetObject.SetActive(false);
    }

    public void ChangeTarget(Sprite itemSprite)
    {
        if (itemSprite == null)
        {
            InitTarget();
            return;
        }

        App.Manager.UI.GetPanel<CraftPanel>().TurnHologram(true);
        targetObject.SetActive(true);
        targetSprite.sprite = itemSprite;
    }
}