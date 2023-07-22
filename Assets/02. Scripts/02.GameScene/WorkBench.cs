using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WorkBench : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onClickEvent;
   
    public void OnPointerClick(PointerEventData eventData)
    {
        onClickEvent.Invoke();
    }
}
