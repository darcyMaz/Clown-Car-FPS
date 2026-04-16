using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    // This class has all of the progress of the game.
    // 1) Starting Dialogue
    // 2) First three enemies spawn in
    // 3) Dialogue: let's jump down
    // 4) 6 enemies
    // 5) Let's get up on the seat
    // 6) Boost up onto seat
    // 7) Seat reclines
    // 8) 1 big enemy spawns in
    // 9) Dialogue: We did it ig 

    private int ProgressStep = 0;
    private Dictionary<string, Sprite> Portraits = new Dictionary<string, Sprite>();

    private void Awake()
    {
        Sprite[] portraits_str = Resources.LoadAll<Sprite>("Portraits");
        foreach (Sprite sprite in portraits_str) Portraits.Add(sprite.name, sprite);
    }

    private void Start()
    {
        OpeningDialogue();
    }

    private void Update()
    {
        if (!DialogueManager.Instance.IsDialogueOngoing() && ProgressStep == 0)
        {
            ProgressStep++;
            EnemyManager.Instance.NextWave();
        }
    }

    private void OpeningDialogue()
    {
        string[] dialogues_str = GetDialogueText("Dialogue\\Opening Dialogue");

        string[] portrait_keys_in_order = new string[dialogues_str.Length];
        portrait_keys_in_order[0] = "Smiley_0";
        portrait_keys_in_order[1] = "Smiley_0";
        portrait_keys_in_order[2] = "Smiley_0";

        Dialogue[] dialogues = GetDialogues(dialogues_str, portrait_keys_in_order);

        DialogueManager.Instance.StartDialogue(dialogues);
    }



    private Dialogue[] GetDialogues(string[] dialogues_str, string[] portrait_keys)
    {
        Dialogue[] dialogues = new Dialogue[dialogues_str.Length];

        for (int index = 0; index < dialogues_str.Length; index++)
        {
            Sprite tryGetSprite;
            Portraits.TryGetValue(portrait_keys[index], out tryGetSprite);
            //Debug.Log(dialogues_str[index]);
            dialogues[index] = new Dialogue(dialogues_str[index], portrait_keys[index], tryGetSprite);
        }

        return dialogues;
    }

    private string[] GetDialogueText(string folder)
    {
        TextAsset[] files = Resources.LoadAll<TextAsset>(folder);

        string[] dialogues = new string[files.Length];

        for (int index = 0; index < dialogues.Length; index++)
        {
            dialogues[index] = files[index].text;
        }

        return dialogues;
    }
}
