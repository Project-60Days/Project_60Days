using Cinemachine;
using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MapInteraction : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] CanvasGroup shelterUi;
    CinemachineVirtualCamera mapCamera;
    CinemachineFramingTransposer transposer;
    public UnityEvent onClickEvent;

    void Start() //임시..
    {
        mapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<CinemachineVirtualCamera>();
        transposer = mapCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    /// <summary>
    /// 지도 클릭 시 이벤트 발생 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        FadeOutUiObjects();
    }

    void FadeOutUiObjects()
    {
        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(shelterUi.DOFade(0f, 0.5f))
            .OnComplete(() => ZoomInMap());
        sequence.Play();
    }

    void ZoomInMap()
    {
        App.instance.GetMapManager().SetMapCameraPriority(true);
        //DOTween.To(() => transposer.m_CameraDistance, x => transposer.m_CameraDistance = x, targetDistance, duration);
        //StartCoroutine("OrthoAnim");
    }

    IEnumerator OrthoAnim()
    {
        for(int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            transposer.m_CameraDistance -= 0.05f;
        }
    }
}