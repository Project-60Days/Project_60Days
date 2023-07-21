using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity;

public class Tutorial_01_ReadDiary : TutorialBase
{
    NoteController noteController;
    DialogueRunner diaryDialogueRunner;

    public override void Init()
    {
        noteController = FindObjectOfType<NoteController>();

    }

    public override void Enter()
    {


    }

    public override void Execute(TutorialController _controller)
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }

}
