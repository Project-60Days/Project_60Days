using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;

public class OptionController : MonoBehaviour
{
    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] GameObject[] optionButtons;

    string[] options;

    InMemoryVariableStorage variableStorage;

    public void ChooseEventSetOption()
    {
        variableStorage = GameObject.FindObjectOfType<InMemoryVariableStorage>();
        for (int i = 0; i < options.Length; i++)
        {
            variableStorage.TryGetValue("$option" + (i + 1), out options[i]);
            if (options[i] == "null")
                return;
            else
            {
                optionButtons[i].SetActive(true);
                optionButtons[i].GetComponent<Text>().text = options[i];
            }
        }

    }
    void Start()
    {
        options = new string[4];
        for (int i = 0; i < options.Length; i++)
            optionButtons[i].SetActive(false);
    }
}
