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
        UIManager.instance.AddCurrUIName("UI_LOADING");

        yield return new WaitUntil(() => App.instance.GetMapManager().Controller != null);
        yield return new WaitUntil(() => App.instance.GetMapManager().Controller.LoadingComplete == true);

        App.instance.GetSoundManager().PlayBGM("BGM_InGameTheme");
        gameObject.SetActive(false);
        UIManager.instance.GetNextDayController().InitBlackPanel();
    }
}
