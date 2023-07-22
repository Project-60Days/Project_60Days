using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialDialog : TutorialBase
{
    [SerializeField]
    GameObject DialogPanel;
    [SerializeField]
    Image DialogArea;
    [SerializeField]
    Image NPC;

    [TextArea(3, 50), SerializeField]
    string[] dialog;

    int dialogIndex = -1;
    bool isStart = true;

    void SetNextDialog(TutorialController _controller = null)
    {
        if (dialogIndex >= dialog.Length - 1)
        {
            isStart = true;
            DialogArea.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.Linear);
            NPC.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _controller.SetNextTutorial();
            });
            return;
        }

        dialogIndex++;
    }

    public override void Enter()
    {
        isStart = true;

        DialogPanel.SetActive(true);
        DialogArea.color = Color.white;
        NPC.color = Color.white;

        DialogArea.GetComponent<CanvasGroup>().DOFade(0, 0.5f).From().SetEase(Ease.Linear);
        NPC.DOFade(0, 0.5f).From().SetEase(Ease.Linear).OnComplete(() =>
        {
            isStart = false;
            SetNextDialog();
        });
    }

    public override void Execute(TutorialController _controller)
    {
        if (Input.anyKeyDown && !isStart)
        {
            SetNextDialog(_controller);
        }
    }

    public override void Exit()
    {
        DialogPanel.SetActive(false);
    }

    public override void Init()
    {
        throw new System.NotImplementedException();
    }
}