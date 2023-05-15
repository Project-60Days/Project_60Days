using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class TempScript : MonoBehaviour
{
    public GameObject pageContainer;
    public GameObject dialogueBox;
    public Sprite[] btnImages;
    public GameObject nextDay;
    public Transform[] notePages;

    public DialogueRunner dialogueRunner;

    public Button nextPageBtn;
    public Button prevPageBtn;
    public Button nextDayBtn;

    NoteManager noteManager;

    private int pageNum = 0;
    private int pages = 0;
    int selectedNumber;
    List<int> numbers = new List<int>() { 1, 2, 3, 4, 5 };
    private int dayCount = 1;

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


        pages = CountPages("Day" + dayCount, 30) + CountPages("Day" + dayCount + "ChooseEvent", 30) + CountPages("specialEvent" + selectedNumber, 30) + 3;
        

        dialogueBox.SetActive(false);

        int randomIndex = Random.Range(0, numbers.Count);
        selectedNumber = numbers[randomIndex];
        numbers.RemoveAt(randomIndex);

        nextPageBtn.onClick.AddListener(NextPageEvent);
        prevPageBtn.onClick.AddListener(PrevPageEvent);
        nextDayBtn.onClick.AddListener(NextDayEvent);

        noteManager=GameObject.Find("Box_Back").GetComponent<NoteManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (noteManager.isOpen)
        {
            notePages[pageNum].gameObject.SetActive(true);
            ChangePageButton();
        }
        else
        {
            notePages[pageNum].gameObject.SetActive(false);
            dialogueBox.SetActive(false);
            nextPageBtn.image.sprite = btnImages[0];
            prevPageBtn.image.sprite = btnImages[0];
        }
    }
    int CountPages(string nodeName, int maxCharsPerPage)
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

        return pageCount;
    }

    string[] CreatePages(string nodeName, int pageCount, int maxCharsPerPage)
    {
        Yarn.IVariableStorage variableStorage = new Yarn.MemoryVariableStore();
        Dialogue dialogue = new Dialogue(variableStorage);

        string[] pages = new string[pageCount];
        string text = dialogue.GetStringIDForNode(nodeName);
        for (int i = 0; i < pageCount; i++)
        {
            int startIndex = i * maxCharsPerPage;
            int endIndex = Mathf.Min(startIndex + maxCharsPerPage, text.Length);
            pages[i] = text.Substring(startIndex, endIndex - startIndex);
        }

        return pages;
    }

    /// <summary>
    /// 다음 페이지 버튼
    /// </summary>
    private void NextPageEvent()
    {
        if (pageNum + 1 > notePages.Length - 1)
            return;

        PageOn(pageNum + 1);
    }
    /// <summary>
    /// 이전 페이지 버튼
    /// </summary>
    private void PrevPageEvent()
    {
        if (pageNum - 1 < 0)
            return;

        PageOn(pageNum - 1);
    }

    private void PageOn(int index)
    {
        notePages[pageNum].gameObject.SetActive(false);
        notePages[index].gameObject.SetActive(true);

        pageNum = index;

        ChangePageButton();
    }
    void NextDayEvent()
    {
        pageNum = 0;
        int randomIndex = Random.Range(0, numbers.Count);
        selectedNumber = numbers[randomIndex];
        numbers.RemoveAt(randomIndex);
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

    void PageOne()
    {
        string nodeName;
        dialogueBox.SetActive(true);
        nodeName = "Day" + dayCount;
        dialogueRunner.Stop();
        dialogueRunner.StartDialogue(nodeName);
        int pageCount = PageManager.Instance.CountPages(nodeName, 30);
        string[] pages = PageManager.Instance.CreatePages(nodeName, pageCount, 30);
    }
    void PageTwo()
    {

    }
    void PageThree()
    {

    }
    void PageFour()
    {
        string nodeName;
        dialogueBox.SetActive(true);
        nodeName = "Day" + dayCount + "ChooseEvent";
        dialogueRunner.Stop();
        dialogueRunner.StartDialogue(nodeName);
    }
    void PageFive()
    {
        string nodeName;
        dialogueBox.SetActive(true);
        nodeName = "specialEvent" + selectedNumber;
        dialogueRunner.Stop();
        dialogueRunner.StartDialogue(nodeName);
    }
    void PageSix()
    {

    }
}
