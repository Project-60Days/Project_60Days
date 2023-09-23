//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class TutorialController1 : MonoBehaviour
//{
//    List<TutorialBase> tutorialList;

//    public TutorialBase currentTutorial;
//    int TutorialIndex = -1;

//    public void Init()
//    {
//        tutorialList = new List<TutorialBase>(this.GetComponentsInChildren<TutorialBase>());
//    }

//    private void Update()
//    {
//        //currentTutorial?.Execute(this);
//    }

//    public void SetNextTutorial()
//    {
//        currentTutorial?.Exit();

//        if (TutorialIndex >= tutorialList.Count - 1)
//        {
//            CompleteTutorial();
//            return;
//        }

//        TutorialIndex++;
//        currentTutorial = tutorialList[TutorialIndex];

//        currentTutorial?.Enter();
//    }

//    void CompleteTutorial()
//    {
//        currentTutorial = null;

//        // 모든 튜토리얼 종료
//    }
//}
