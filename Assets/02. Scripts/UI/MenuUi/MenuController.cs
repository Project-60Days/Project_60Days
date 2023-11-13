using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public MenuButtonBase[] buttons;

    void Awake()
    {
        buttons = GetComponentsInChildren<MenuButtonBase>();
        gameObject.SetActive(false);
    }

    public void EnterMenu()
    {
        gameObject.SetActive(true);
        foreach (var button in buttons)
            button.Init();
    }

    public void QuitMenu()
    {
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }
}
