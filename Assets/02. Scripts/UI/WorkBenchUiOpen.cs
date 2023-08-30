using UnityEngine;
using DG.Tweening;

public class WorkBenchUiOpen : MonoBehaviour
{
    [SerializeField] GameObject inventoryUi;
    [SerializeField] GameObject craftingUi;

    [SerializeField] Transform craftingStartPos;
    [SerializeField] Transform craftingPointPos;

    [SerializeField] Transform inventoryStartPos;
    [SerializeField] Transform inventoryPointPos;

    Sequence sequence;

    private void Start()
    {
        inventoryUi.transform.position = craftingStartPos.position;
        craftingUi.transform.position = inventoryStartPos.position;
    }


    private void OnEnable()
    {
        SetOnClickEvent(true);
    }

    private void OnDisable()
    {
        SetOnClickEvent(true);
    }

    private void SetOnClickEvent(bool enable)
    {
        WorkBenchInteraction onClickScript = FindObjectOfType<WorkBenchInteraction>();
        if (onClickScript != null)
        {
            if (enable)
            {
                onClickScript.onClickEvent.AddListener(ActivateUIObjects);
            }
            else
            {
                onClickScript.onClickEvent.RemoveListener(ActivateUIObjects);
            }
        }
    }

    private void ActivateUIObjects()
    {
        sequence?.Kill();

        sequence = DOTween.Sequence();

        sequence
            .OnStart(() =>
            {
                inventoryUi.SetActive(true);
                craftingUi.SetActive(true);
                inventoryUi.transform.position = inventoryStartPos.position;
                craftingUi.transform.position = craftingStartPos.position;
            })
            .Append(inventoryUi.transform.DOMoveX(inventoryPointPos.position.x, 1f))
            .Join(craftingUi.transform.DOMoveX(craftingPointPos.position.x, 1f));
    }
}
