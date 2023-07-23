using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_02_AIDialogue : TutorialBase
{
    public override void Enter()
    {
        UIManager.instance.GetTutorialDialogue().StartDialogue();
    }

    public override void Execute(TutorialController _controller)
    {

    }

    public override void Exit()
    {
        UIManager.instance.GetTutorialDialogue().EndDialogue();
    }
}
