using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    List<MenuButtonBase> buttons = new List<MenuButtonBase>();

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++) 
            buttons.Add(transform.GetChild(i).GetComponent<MenuButtonBase>());
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
