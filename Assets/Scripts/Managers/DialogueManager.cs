using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Image DialogueImage;
    [SerializeField] private TextMeshProUGUI CharacterName;
    [SerializeField] private TextMeshProUGUI Dialogue;
    [SerializeField] private Image Background;

    // make it an instance singleton
    public static DialogueManager Instance { get; private set; }

    private bool IsDialogueActive = false;
    private Dialogue[] Dialogues;
    private Sprite[] Sprites;
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
    }

    // Everything is on on Awake, turned off on start.
    private void Start()
    {
        backgroundAlpha = Background.color.a;
        //dialogueAlpha = Dialogue.color.a;
        dialogueImageAlpha = DialogueImage.color.a;
        //charNameAlpha = characterName.color.a;

        BackgroundOff();
        DialogueOff();
        DialogueImageOff();
        CharacterNameOff();
    }

    public bool StartDialogue(Dialogue[] dialogues, Sprite[] sprites)
    {
        if (IsDialogueActive) return false;
        IsDialogueActive = true;

        Dialogues = dialogues;
        Sprites = sprites;

        BackgroundOn();
        SetDialogue();

        return true;
    }
    public bool NextDialogue()
    {
        if (!IsDialogueActive) return false;

        DialoguesIndex++;
        if (DialoguesIndex == Dialogues.Length)
        {
            EndDialogue();
        }

        SetDialogue();

        return true;
    }
    public void EndDialogue()
    {
        DialoguesIndex = 0;
        Dialogues = new Dialogue[0];
        Sprites = new Sprite[0];

        DialogueOff();
        BackgroundOff();
        CharacterNameOff();
        DialogueImageOff();

        IsDialogueActive = false;
    }

    private void SetDialogue()
    {
        SetDialogueText(Dialogues[DialoguesIndex].text);
        SetDialogueImage(Sprites[Dialogues[DialoguesIndex].charSpriteIndex]);
        SetCharacterName(Dialogues[DialoguesIndex].charName);
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

    public bool IsDialogueOngoing()
    {
        return IsDialogueActive;
    }
}

public struct Dialogue
{
    public string text;
    public string charName;
    public int charSpriteIndex;
}
