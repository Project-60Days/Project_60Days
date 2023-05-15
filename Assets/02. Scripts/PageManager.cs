using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Yarn;
using Yarn.Unity;

public class PageManager : Singleton<PageManager>
{
    private static PageManager instance = null;

    void Awake()
    {
        if(null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    
    public static PageManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public int CountPages(string nodeName, int maxCharsPerPage)
    {
        Yarn.IVariableStorage variableStorage = new Yarn.MemoryVariableStore();
        Dialogue dialogue = new Dialogue(variableStorage);

        string text = dialogue.GetStringIDForNode(nodeName);

        int pageCount = 1;
        int totalChars = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                pageCount++;
                totalChars = 0;
            }
            else
            {
                totalChars++;

                if (totalChars > maxCharsPerPage)
                {
                    pageCount++;
                    totalChars = 0;
                }
            }
        }

        GC.Collect();
        return pageCount;
    }

    public string[] CreatePages(string nodeName, int pageCount, int maxCharsPerPage)
    {
        Yarn.IVariableStorage variableStorage = new Yarn.MemoryVariableStore();
        Dialogue dialogue = new Dialogue(variableStorage);

        string[] pages = new string[pageCount];
        string text = dialogue.GetStringIDForNode(nodeName);
        for(int i = 0; i < pageCount; i++)
        {
            int startIndex = i * maxCharsPerPage;
            int endIndex = Mathf.Min(startIndex + maxCharsPerPage, text.Length);
            pages[i] = text.Substring(startIndex, endIndex - startIndex);
        }

        GC.Collect();
        return pages;
    }
}
