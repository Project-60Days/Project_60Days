using UnityEngine;


public class BenchRawCtrl : MonoBehaviour
{
    private SpriteRenderer targetSprite;

    private void Start()
    {
        InventorySlot.CraftItemClick += ChangeTarget;
        BlueprintSlot.CraftItemClick += ChangeTarget;
        CraftSlot.CraftItemClick += InitTarget;

        targetSprite = GameObject.FindWithTag("RenderTextureObject").GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        InventorySlot.CraftItemClick -= ChangeTarget;
        BlueprintSlot.CraftItemClick -= ChangeTarget;
        CraftSlot.CraftItemClick -= InitTarget;
    }

    public void InitTarget()
    {
        gameObject.SetActive(false);
    }

    public void ChangeTarget(Sprite itemSprite)
    {
        Debug.Log("ChangeTarget");

        if (itemSprite == null)
        {
            InitTarget();
            return;
        }

        gameObject.SetActive(true);
        targetSprite.sprite = itemSprite;
    }
}