using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuController : MonoBehaviour
{
    [SerializeField] Transform text;
    [SerializeField] Transform saveDetails;
    [SerializeField] Transform loadDetails;
    [SerializeField] Transform settingDetails;
    MenuButtonBase[] buttons;

    float textInitialY;
    float saveInitialY;
    float loadInitialY;
    float settingInitialY;

    void Start()
    {
        buttons = GetComponentsInChildren<MenuButtonBase>();
       
        textInitialY = text.transform.localPosition.y;
        saveInitialY = saveDetails.transform.localPosition.y;
        loadInitialY = loadDetails.transform.localPosition.y;
        settingInitialY = settingDetails.transform.localPosition.y;

        gameObject.SetActive(false);
    }

    public void EnterMenu()
    {
        UIManager.instance.AddCurrUIName("UI_MENU");
        gameObject.SetActive(true);
        foreach (var button in buttons)
            button.Init();
    }

    public void QuitMenu()
    {
        if (UIManager.instance.isUIStatus("UI_MENU"))
            UIManager.instance.PopCurrUI();
        else return;

        foreach (var button in buttons) 
        {
            if (button.isClicked == true)
                button.CloseEvent();
        }

        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void InitSettingButtonsLocation()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject.name == "Setting_Btn") continue;
            buttons[i].gameObject.GetComponent<Transform>().DOLocalMoveY(buttons[i].initialY, 0f);
        }

        text.localPosition = new Vector3(text.localPosition.x, textInitialY, text.localPosition.z);
        saveDetails.localPosition = new Vector3(saveDetails.localPosition.x, saveInitialY, saveDetails.localPosition.z);
        loadDetails.localPosition = new Vector3(loadDetails.localPosition.x, loadInitialY, loadDetails.localPosition.z);
        settingDetails.localPosition = new Vector3(settingDetails.localPosition.x, settingInitialY, settingDetails.localPosition.z);
    }
}
