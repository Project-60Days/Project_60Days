using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    List<TutorialBase> tutorialList;

    public TutorialBase currentTutorial;
    int TutorialIndex = -1;

    public void Init()
    {
        tutorialList = new List<TutorialBase>(this.GetComponentsInChildren<TutorialBase>());
        SetNextTutorial();
    }

    public void SetNextTutorial()
    {
        TutorialIndex++;

        if (TutorialIndex > tutorialList.Count - 1)
        {
            CompleteTutorial();
            return;
        }

        currentTutorial = tutorialList[TutorialIndex];

        currentTutorial?.Enter();
    }

    void CompleteTutorial()
    {
        currentTutorial = null;
        TutorialManager.instance.EndTutorial();
    }
}
