using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] GameObject soundBar;
    [SerializeField] Transform parentTransform;

    private float startDragX;
    private float startWidth;
    private bool isDragging = false;

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

            float maxParentWidth = parentTransform.localScale.x;
            float clampedWidth = Mathf.Clamp(newWidth, 0f, maxParentWidth);

            Vector3 newScale = soundBar.transform.localScale;
            newScale.x = clampedWidth;

            soundBar.transform.localScale = newScale;
        }
    }
}
