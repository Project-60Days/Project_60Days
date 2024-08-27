using UnityEngine;
using UnityEngine.Scripting;

public enum EventCode
{
    PlayerCreate,
    GameStart,
    GoToMap,
    GoToShelter,
    TileUpdate,
    NextDayStart,
    NextDayMiddle,
    NextDayEnd,
    Hit,
    Die,
    NormalDay,
    TutorialStart,
    TutorialEnd,
};

[RequireImplementors]
public interface IListener
{
    void OnEvent(EventCode _code, Component _sender, object _param = null);
}