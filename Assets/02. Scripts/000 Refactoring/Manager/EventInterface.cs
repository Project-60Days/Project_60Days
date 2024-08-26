using UnityEngine;
using UnityEngine.Scripting;

public enum EventCode
{
    PlayerCreate,
    GoToMap,
    GoToShelter,
    TileUpdate,
    NextDayStart,
    NextDayMiddle,
    NextDayEnd,
    Hit,
    Die,
    NormalDay
};

[RequireImplementors]
public interface IListener
{
    void OnEvent(EventCode _code, Component _sender, object _param = null);
}