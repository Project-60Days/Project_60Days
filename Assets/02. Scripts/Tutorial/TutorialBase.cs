using UnityEngine;

public abstract class TutorialBase : MonoBehaviour
{
    public abstract void Init();
    public abstract void Enter();

    public abstract void Execute(TutorialController _controller);

    public abstract void Exit();
}
