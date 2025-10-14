using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public string ID;
    public string speakerName;
    public string eventTrigger;
    public DialogueChoice[] choices;

    [TextArea(3, 5)]
    public string dialogueText;
}

[Serializable]
public class DialogueChoice
{
    public string choiceText;
    public string nextDialogueID;
}
