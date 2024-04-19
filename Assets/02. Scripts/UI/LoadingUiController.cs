using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUiController : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(true);
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        App.Manager.UI.AddCurrUIName(UIState.Loading);

        yield return new WaitUntil(() => App.Manager.Map.mapController != null);
        yield return new WaitUntil(() => App.Manager.Map.mapController.LoadingComplete == true);

        App.Manager.Sound.PlayBGM("BGM_InGameTheme");
        gameObject.SetActive(false);
        App.Manager.UI.GetNextDayController().InitBlackPanel();
    }
}
