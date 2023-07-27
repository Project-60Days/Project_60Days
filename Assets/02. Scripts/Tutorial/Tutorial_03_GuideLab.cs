using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_03_GuideLab : TutorialBase
{
    public override void Enter()
    {
        UIManager.instance.GetTutorialDialogue().StartDialogue(this.name);
    }

    public override void Execute(TutorialController _controller)
    {

    }

    public override void Exit()
    {
        UIManager.instance.GetTutorialDialogue().EndDialogue();
    }
}
