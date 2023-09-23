using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_04 : TutorialBase
{
    public override void Enter()
    {
        UIManager.instance.GetTutorialDialogue().StartDialogue(this.name);
    }

    public override void Exit()
    {
        UIManager.instance.GetTutorialDialogue().EndDialogue();
    }
}
