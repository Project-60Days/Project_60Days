using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SoundButton : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] GameObject soundBar;
    [SerializeField] Transform parentTransform;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] SoundController soundController;
    
    public SoundType type;
    [HideInInspector] public float currentWidth;

    float initWidth;

    float startDragX;
    float startWidth;
    bool isDragging = false;

    void Awake()
    {
        initWidth = soundBar.transform.localScale.x;
        currentWidth = initWidth;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;
            startDragX = eventData.position.x;
            startWidth = soundBar.transform.localScale.x;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging == true)
        {
            float currentDragX = eventData.position.x;

            float widthChange = (currentDragX - startDragX) / Screen.width * 2;

            float newWidth = startWidth + widthChange;

            float clampedWidth = Mathf.Clamp(newWidth, 0f, initWidth);

            Vector3 newScale = soundBar.transform.localScale;
            newScale.x = clampedWidth;
            soundBar.transform.localScale = newScale;

            float widthRatio = clampedWidth * 100;
            text.text = ((int)widthRatio).ToString();

            currentWidth = clampedWidth;

            soundController.SetVolume(this);
        }
    }
}
