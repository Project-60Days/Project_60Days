using UnityEngine;


public class BenchRawCtrl : MonoBehaviour
{
    private SpriteRenderer targetSprite;

    private void Start()
    {
        ItemSlot.CraftItemClick += ChangeTarget;
        BlueprintSlot.CraftItemClick += ChangeTarget;
        CraftSlot.CraftItemClick += InitTarget;

        targetSprite = GameObject.FindWithTag("RenderTextureObject").GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        ItemSlot.CraftItemClick -= ChangeTarget;
        BlueprintSlot.CraftItemClick -= ChangeTarget;
        CraftSlot.CraftItemClick -= InitTarget;
    }

    public void InitTarget()
    {
        gameObject.SetActive(false);
    }

    private void ChangeTarget(Sprite itemSprite)
    {
        if (itemSprite == null)
        {
            InitTarget();
            return;
        }

        gameObject.SetActive(true);
        targetSprite.sprite = itemSprite;
    }
}