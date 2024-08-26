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
    /// ���� index�� todayNodeNames ����Ʈ ���� ���Ͽ� ����Ʈ�� �� ��° �������� ��ȯ��
    /// -1 : ����Ʈ�� 0��° ����
    /// 0 : ����Ʈ�� �߰� ����
    /// 1 : ����Ʈ�� ������ ����
    /// 2 : ����Ʈ�� ���Ұ� �ϳ�����
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
