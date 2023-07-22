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
        noteController = FindObjectOfType<NoteController>();
        noteController.SetTutorialDiary();
    }

    public override void Execute(TutorialController _controller)
    {

    }

    public override void Exit()
    {

    }

}
