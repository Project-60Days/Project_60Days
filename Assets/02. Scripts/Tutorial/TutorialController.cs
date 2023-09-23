using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    void CompleteTutorial()
    {
        TutorialManager.instance.EndTutorial();
    }
}
