using DG.Tweening.Plugins;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity;

public class Note_YarnManager : MonoBehaviour
{
    public TextAsset[] yarnScripts;
    private int currentScriptIndex = 0;
    public DialogueRunner dialogueRunner;
    string scriptName;


    // Start is called before the first frame update
    void Start()
    {
        //string randomNodeName = dialogueRunner.dialogue.nodes.Keys.ElementAt(UnityEngine.Random.Range(0, dialogueRunner.dialogue.nodes.Count));
        //string[] nodeNames = dialogueRunner.dialogue.nodes.Keys.ToArray();
        scriptName = yarnScripts[0].name;
        dialogueRunner.StartDialogue(scriptName);
    }

    public void RunNextScript()
    {
        currentScriptIndex++;

        if (currentScriptIndex >= yarnScripts.Length)
        {
            return;
        }

        dialogueRunner.Stop();
        scriptName = yarnScripts[currentScriptIndex].name;
        dialogueRunner.StartDialogue(scriptName);

    }
    // Update is called once per frame
    void Update()
    {
        if (dialogueRunner.IsDialogueRunning)
        {
            //Dialogue dialogue = dialogueRunner.CurrentDialogue;
            //string[] nodeNames = dialogue.nodes.Keys.ToArray();
            //string randomNodeName = nodeNames[Random.Range(0, nodeNames.Length)];
            //dialogueRunner.NodeComplete(randomNodeName);
        }
    }
}
