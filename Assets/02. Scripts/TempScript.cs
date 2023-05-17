using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class TempScript : MonoBehaviour
{
    [SerializeField] GameObject pageContainer;
    [SerializeField] GameObject dialogueBox;
    [SerializeField] GameObject nextDay;
    [SerializeField] Sprite[] btnImages;
    [SerializeField] Transform[] notePages;

    [SerializeField] GameObject prefab;
    [SerializeField] Transform parent;

    public DialogueRunner dialogueRunner;

    [SerializeField] Button nextPageBtn;
    [SerializeField] Button prevPageBtn;
    [SerializeField] Button nextDayBtn;

    InMemoryVariableStorage variableStorage;
    private int pageNum = 0;
    private int pages = 0;
    int selectedNumber;
    List<int> numbers = new List<int>() { 1, 2, 3, 4, 5 };
    private int dayCount = 1;
    bool isContinued = false;
    string nextNode;
    string nodeName;
    bool isEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] allChildren = pageContainer.GetComponentsInChildren<Transform>();
        List<Transform> targets = new List<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("NotePage"))
            {
                targets.Add(child);
            }
        }
        notePages = targets.ToArray();
        for (int i = 0; i < notePages.Length; i++)
        {
            notePages[i].gameObject.SetActive(false);
        }

        dialogueBox.SetActive(false);

        int randomIndex = Random.Range(0, numbers.Count);
        selectedNumber = numbers[randomIndex];
        numbers.RemoveAt(randomIndex);

        InstantiateNewNameCard();

        nextPageBtn.onClick.AddListener(NextPageEvent);
        prevPageBtn.onClick.AddListener(PrevPageEvent);
        nextDayBtn.onClick.AddListener(NextDayEvent);

        dialogueRunner.StartDialogue("Day1");
        variableStorage = GameObject.FindObjectOfType<InMemoryVariableStorage>();
    }

    public void OpenBox()
    {
        notePages[pageNum].gameObject.SetActive(true);
        ChangePageButton();
        PageOn(0);
    }
    public void CloseBox()
    {
        notePages[pageNum].gameObject.SetActive(false);
        dialogueBox.SetActive(false);
        nextPageBtn.image.sprite = btnImages[0];
        prevPageBtn.image.sprite = btnImages[0];
    }

    /// <summary>
    /// 다음 페이지 버튼
    /// </summary>
    private void NextPageEvent()
    {
        if (pageNum + 1 > notePages.Length - 1)
            return;

        if(isEnd || pageNum == 1 || pageNum == 2 || pageNum == 5)
        {
            dialogueBox.SetActive(false);
            ChangePage(pageNum + 1);
        }
  
        PageOn(pageNum);
    }
    /// <summary>
    /// 이전 페이지 버튼
    /// </summary>
    private void PrevPageEvent()
    {
        if (pageNum - 1 < 0)
            return;
        if (isEnd || pageNum == 1 || pageNum == 2 || pageNum == 5)
        {
            dialogueBox.SetActive(false);
            ChangePage(pageNum - 1);
        }
        
        PageOn(pageNum);
    }
    
    void ChangePage(int index)
    {
        notePages[pageNum].gameObject.SetActive(false);
        notePages[index].gameObject.SetActive(true);

        pageNum = index;
        isEnd = false;
        ChangePageButton();
    }

    private void PageOn(int index)
    {
        if (pageNum == 1 || pageNum == 2 || pageNum == 5)
            return;

        switch (index)
        {
            case 0:
                nodeName = "Day" + dayCount; break;
            case 3:
                nodeName = "Day" + dayCount + "ChooseEvent"; break;
            case 4:
                nodeName = "specialEvent" + selectedNumber; break;
            default:
                dialogueBox.SetActive(false);
                ChangePage(index + 1);
                break;
        }
        if (!isContinued)
        {
            dialogueBox.SetActive(true);

            dialogueRunner.Stop();
            dialogueRunner.StartDialogue(nodeName);
            variableStorage = GameObject.FindObjectOfType<InMemoryVariableStorage>();
            variableStorage.TryGetValue("$isContinued", out isContinued);
            Debug.Log("isContinued" + isContinued);
            variableStorage.TryGetValue("$nextNode", out nextNode);
            variableStorage.TryGetValue("$isEnd", out isEnd);
        }
        else
        {
            nodeName = nextNode;
            dialogueRunner.Stop();
            dialogueRunner.StartDialogue(nodeName);
            variableStorage = GameObject.FindObjectOfType<InMemoryVariableStorage>();
            variableStorage.TryGetValue("$isContinued", out isContinued);
            variableStorage.TryGetValue("$nextNode", out nextNode);
            variableStorage.TryGetValue("$isEnd", out isEnd);
        }
    }
    void NextDayEvent()
    {
        pageNum = 0;
        int randomIndex = Random.Range(0, numbers.Count);
        selectedNumber = numbers[randomIndex];
        numbers.RemoveAt(randomIndex);
        for (int i = 0; i < notePages.Length; i++)
        {
            notePages[i].gameObject.SetActive(false);
        }
        dayCount++;
        RemoveExistingNameCard();
        InstantiateNewNameCard();
    }
    /// <summary>
    /// 페이지 버튼 이미지 변경
    /// </summary>
    void ChangePageButton()
    {
        if (pageNum == 0)
        {
            nextPageBtn.image.sprite = btnImages[1];
            prevPageBtn.image.sprite = btnImages[0];
        }
        else if (pageNum == notePages.Length - 1)
        {
            nextPageBtn.image.sprite = btnImages[0];
            prevPageBtn.image.sprite = btnImages[1];
            nextDay.SetActive(true);
        }
        else
        {
            nextPageBtn.image.sprite = btnImages[1];
            prevPageBtn.image.sprite = btnImages[1];
            nextDay.SetActive(false);
        }
    }

    void RemoveExistingNameCard()
    {
        notePages[1].gameObject.SetActive(true);
        GameObject[] existingPrefabs = GameObject.FindGameObjectsWithTag("NameCardPrefab");
        foreach (GameObject prefab in existingPrefabs)
        {
            Destroy(prefab);
        }
        notePages[1].gameObject.SetActive(false);
    }
    void InstantiateNewNameCard()
    {
        for (int i = 0; i < selectedNumber; i++)
        {
            GameObject nameCard = Instantiate(prefab, parent);
            nameCard.tag = "NameCardPrefab";
        }
    }
}
