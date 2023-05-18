using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapButtonClick : MonoBehaviour, IPointerClickHandler
{
    public delegate void MyEventDelegate();
    public static event MyEventDelegate MyEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (MyEvent != null)
        {
            MyEvent();
        }
    }
    

}
