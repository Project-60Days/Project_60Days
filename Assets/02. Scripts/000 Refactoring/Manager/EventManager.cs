using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Manager
{
    private Dictionary<EventCode, List<IListener>> Listeners = new();

    public void AddListener(EventCode _code, IListener _listener)
    {
        List<IListener> ListenList = null;

        if (Listeners.TryGetValue(_code, out ListenList))
        {
            ListenList.Add(_listener);
            return;
        }

        ListenList = new List<IListener>();
        ListenList.Add(_listener);
        Listeners.Add(_code, ListenList);
    }

    public void PostEvent(EventCode _code, Component _sender, Object _param = null)
    {
        List<IListener> ListenList = null;

        if (!Listeners.TryGetValue(_code, out ListenList)) return;

        for (int i = 0; i < ListenList.Count; i++)
        {
            ListenList?[i].OnEvent(_code, _sender, _param);
        }
    }

    public void RemoveEvent(EventCode _code) => Listeners.Remove(_code);

    public void RemoveRedundacies()
    {
        Dictionary<EventCode, List<IListener>> newListeners = new();

        foreach (KeyValuePair<EventCode, List<IListener>> item in Listeners)
        {
            for (int i = item.Value.Count - 1; i >= 0; i--)
            {
                if (item.Value[i].Equals(null))
                {
                    item.Value.RemoveAt(i);
                }
            }

            if (item.Value.Count > 0)
            {
                newListeners.Add(item.Key, item.Value);
            }
        }

        Listeners = newListeners;
    }

    public void OnLevelWasLoaded(int level)
    {
        RemoveRedundacies();
    }
}
