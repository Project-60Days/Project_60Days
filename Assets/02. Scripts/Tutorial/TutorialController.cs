using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    List<TutorialBase> tutorialList;

    TutorialBase currentTutorial;
    int TutorialIndex = -1;

    public void Init()
    {
        tutorialList = new List<TutorialBase>(this.GetComponentsInChildren<TutorialBase>());

        SetNextTutorial();
    }

    private void Update()
    {
        currentTutorial?.Execute(this);
    }

    public void SetNextTutorial()
    {
        currentTutorial?.Exit();

        if (TutorialIndex >= tutorialList.Count - 1)
        {
            CompleteTutorial();
            return;
        }

        TutorialIndex++;
        currentTutorial = tutorialList[TutorialIndex];

        currentTutorial?.Enter();
    }

    void CompleteTutorial()
    {
        currentTutorial = null;

        SceneManager.LoadScene("Scene_01_Lobby");
    }
}
