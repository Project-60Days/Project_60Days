using UnityEngine;


public class CraftRawCtrl : MonoBehaviour
{
    [SerializeField] GameObject hologramBack;

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
        hologramBack.SetActive(false);
    }

    private void ChangeTarget(Sprite itemSprite)
    {
        if (itemSprite == null)
        {
            InitTarget();
            return;
        }

        hologramBack.SetActive(true);
        targetSprite.sprite = itemSprite;
    }
}