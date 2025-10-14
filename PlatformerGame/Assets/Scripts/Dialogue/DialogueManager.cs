using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Settings")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Transform choiceButtonParent;
    [SerializeField] private Button choiceButtonPrefab;

    private Dictionary<string, DialogueLine> dialogueMap;
    public bool IsDialogueActive { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            dialoguePanel.SetActive(false);
        }
    }

    public void StartDialogue(DialogueLine[] dialogueLines)
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            Debug.LogError("Attempted to start dialogue with empty or null dialogue.");
            return;
        }
        dialogueMap = new Dictionary<string, DialogueLine>();
        foreach (var line in dialogueLines)
        {
            dialogueMap[line.ID] = line;
        }

        if (string.IsNullOrEmpty(dialogueLines[0].ID))
        {
            Debug.LogError("The first dialogue line is missing an ID.");
            return;
        }
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        ProcessLine(dialogueLines[0]);
    }

    private void ProcessLine(DialogueLine dialogueLine)
    {
        speakerText.text = dialogueLine.speakerName;
        dialogueText.text = dialogueLine.dialogueText;

        if (!string.IsNullOrEmpty(dialogueLine.eventTrigger))
        {
            Debug.Log($"Event Triggered: {dialogueLine.eventTrigger}");
        }

        foreach (Transform child in choiceButtonParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var choice in dialogueLine.choices)
        {
            Button newBtn = Instantiate(choiceButtonPrefab, choiceButtonParent);
            newBtn.GetComponentInChildren<TMP_Text>().text = choice.choiceText;
            newBtn.onClick.AddListener(() => OnChoiceSelected(choice.nextDialogueID));
        }
    }

    private void OnChoiceSelected(string nextDialogueID)
    {
        if (dialogueMap.ContainsKey(nextDialogueID))
        {
            ProcessLine(dialogueMap[nextDialogueID]);
        }
        else
        {
            HideDialogue();
        }
    }

    public void HideDialogue()
    {
        if (!IsDialogueActive) return;

        IsDialogueActive = false;
        dialoguePanel.SetActive(false);
        dialogueMap.Clear();
    }
}
