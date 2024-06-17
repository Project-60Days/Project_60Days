using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Yarn.Unity;

public abstract class PageBase : MonoBehaviour
{
    [SerializeField] protected List<string> todayNodeNames = new List<string>();
    [SerializeField] protected List<string> tomorrowNodeNames = new List<string>();

    public List<string> todayResourceNodeNames = new List<string>();

    protected int index = 0;

    public abstract PageType GetPageType();

    public int resourceIndex = 0;

    public virtual void SetNodeName(string _nodeName)
    {

    }

    public virtual void SetNodeName(string _nodeName, bool _isResourceNode)
    {
        
    }

    public virtual void InitNodeName()
    {
        resourceIndex = 0;

        todayNodeNames.Clear();

        InitInChildren();

        for (int i = 0; i < tomorrowNodeNames.Count; i++)
            todayNodeNames.Add(tomorrowNodeNames[i]);

        tomorrowNodeNames.Clear();
        index = 0;
    }

    public abstract void InitInChildren();

    public virtual void PlayPageAciton()
    {
        if (index > todayNodeNames.Count - 1)
            index = todayNodeNames.Count - 1;
        else if (index < 0)
            index = 0;

        PlayNode(todayNodeNames[index]);
    }

    public virtual void ChangePageAction(string _btnType)
    {
        if (_btnType == "next")
            index++;
        else
            index--;

        PlayPageAciton();
    }

    public abstract void PlayNode(string _nodeName);
    
    /// <summary>
    /// 현재 index와 todayNodeNames 리스트 길이 비교하여 리스트의 몇 번째 원소인지 반환함
    /// -1 : 리스트의 0번째 원소
    /// 0 : 리스트의 중간 원소
    /// 1 : 리스트의 마지막 원소
    /// 2 : 리스트의 원소가 하나뿐임
    /// </summary>
    /// <returns></returns>
    public virtual int CompareIndex()
    {
        if (todayNodeNames.Count == 1) return 2;
        else if (index <= 0) return -1;
        else if (index >= todayNodeNames.Count - 1) return 1;
        else return 0;
    }

    public virtual bool GetPageEnableToday()
    {
        if (todayNodeNames.Count > 0)
            return true;
        else return false;
    }
}
