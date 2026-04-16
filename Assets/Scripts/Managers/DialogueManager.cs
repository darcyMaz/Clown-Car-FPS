using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Image DialogueImage;
    [SerializeField] private TextMeshProUGUI CharacterName;
    [SerializeField] private TextMeshProUGUI Dialogue;
    [SerializeField] private Image Background;
    [SerializeField] private TextMeshProUGUI InteractPrompt;

    public static DialogueManager Instance { get; private set; }

    private bool IsDialogueActive = false;
    private Dialogue[] Dialogues;
    //private Sprite[] Sprites;
    private int DialoguesIndex = 0;

    private float backgroundAlpha = 0;
    //private float dialogueAlpha = 0;
    private float dialogueImageAlpha = 0;
    //private float charNameAlpha = 0;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        backgroundAlpha = Background.color.a;
        //dialogueAlpha = Dialogue.color.a;
        dialogueImageAlpha = DialogueImage.color.a;
        //charNameAlpha = characterName.color.a;

        BackgroundOff();
        DialogueOff();
        DialogueImageOff();
        CharacterNameOff();
        InteractPromptOff();
    }


    public bool StartDialogue(Dialogue[] dialogues)
    {
        if (IsDialogueActive) return false;
        IsDialogueActive = true;

        Dialogues = dialogues;

        //Debug.Log("Dialogues length in StartDialogue" + Dialogues.Length);

        BackgroundOn();
        SetDialogue();
        InteractPromptOn();

        return true;
    }
    public bool NextDialogue()
    {
        if (!IsDialogueActive) return false;

        DialoguesIndex++;
        if (DialoguesIndex == Dialogues.Length)
        {
            EndDialogue();
            return true;
        }

        SetDialogue();

        return true;
    }
    public void EndDialogue()
    {
        DialoguesIndex = 0;
        Dialogues = new Dialogue[0];

        DialogueOff();
        BackgroundOff();
        CharacterNameOff();
        DialogueImageOff();
        InteractPromptOff();

        IsDialogueActive = false;
    }

    private void SetDialogue()
    {
        // THIS NEEDS CHECKS FOR INDEX OUT OF RANGE

        SetDialogueText(Dialogues[DialoguesIndex].GetText());
        SetDialogueImage(Dialogues[DialoguesIndex].GetSprite());
        SetCharacterName(Dialogues[DialoguesIndex].GetCharName());
    }

    private void BackgroundOn()
    {
        Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, backgroundAlpha);
    }
    private void BackgroundOff()
    {
        Background.color = new Color(Background.color.r, Background.color.g, Background.color.b, 0);
    }

    private void SetDialogueText(string text)
    {
        Dialogue.text = text;
    }
    private void DialogueOff()
    {
        Dialogue.text = "";
    }

    private void SetDialogueImage(Sprite sprite)
    {
        DialogueImage.color = new Color(DialogueImage.color.r, DialogueImage.color.g, DialogueImage.color.b, dialogueImageAlpha);
        DialogueImage.sprite = sprite;
    }
    private void DialogueImageOff()
    {
        DialogueImage.color = new Color(DialogueImage.color.r, DialogueImage.color.g, DialogueImage.color.b, 0);
    }

    private void CharacterNameOff()
    {
        CharacterName.text = "";
    }
    private void SetCharacterName(string text)
    {
        CharacterName.text = text;
    }

    private void InteractPromptOff()
    {
        InteractPrompt.text = "";
    }
    private void InteractPromptOn()
    {
        InteractPrompt.text = "Press E to Continue";
    }

    public bool IsDialogueOngoing()
    {
        return IsDialogueActive;
    }
}

public struct Dialogue
{
    private string text;
    private string charName;
    private Sprite sprite;

    public Dialogue(string aText, string aName, Sprite aSprite)
    {
        text = aText;
        charName = aName;
        sprite = aSprite;
    }

    public string GetText() => text;
    public string GetCharName() => charName;
    public Sprite GetSprite() => sprite;
}
