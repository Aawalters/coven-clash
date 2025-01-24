using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour {
    public static DialogueManager Instance;

    public TextAsset inkJSONAsset; // Reference to the Ink JSON
    private Story story;

    public GameObject dialoguePanel;
    public Text npcNameText;
    public Image npcPortrait;
    public Text dialogueText;
    public GameObject choicesPanel;
    public Button choiceButtonPrefab;

    private bool isDialogueActive;

    // Dictionary for NPC portraits
    public Dictionary<string, Sprite> npcPortraits = new Dictionary<string, Sprite>();

    void Awake() 
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() 
    {
        story = new Story(inkJSONAsset.text);

        // Example NPC portraits setup
        npcPortraits["NPC1"] = Resources.Load<Sprite>("Portraits/NPC1");
        npcPortraits["NPC2"] = Resources.Load<Sprite>("Portraits/NPC2");
    }

    public void StartDialogueAt(string knotName) 
    {
        if (story != null) 
        {
            story.ChoosePathString(knotName);
            StartDialogue();
        } 
        else 
        {
            Debug.LogError($"Knot {knotName} not found in the Ink story!");
        }
    }


    public void StartDialogue() 
    {
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        DisplayNextLine();
    }

    public void DisplayNextLine() 
    {
        if (story.canContinue) 
        {
            string text = story.Continue();
            dialogueText.text = text;
            // handle the tags 
            foreach (string tag in story.currentTags) 
            {
                // set up npc name/portrait
                if (tag.StartsWith("NPC:")) 
                {
                    string npcName = tag.Substring(4);
                    npcNameText.text = npcName;

                    if (npcPortraits.ContainsKey(npcName)) 
                    {
                        npcPortrait.sprite = npcPortraits[npcName];
                    }
                }
            }
        } 
        else 
        {
            EndDialogue();
        }
    }

    private void DisplayChoices() {
        foreach (Transform child in choicesPanel.transform) 
        {
            Destroy(child.gameObject);
        }

        List<Choice> choices = story.currentChoices;

        if (choices.Count > 0) 
        {
            // Display player choices
            choicesPanel.SetActive(true);
            foreach (var choice in choices) 
            {
                Button button = Instantiate(choiceButtonPrefab, choicesPanel.transform);
                button.GetComponentInChildren<Text>().text = choice.text;
                int choiceIndex = choice.index;
                button.onClick.AddListener(() => MakeChoice(choiceIndex));
            }
        } 
        else 
        {
            // No choices, just "click to continue"
            choicesPanel.SetActive(false);
        }
    }

    public void MakeChoice(int choiceIndex) 
    {
        story.ChooseChoiceIndex(choiceIndex);
        DisplayNextLine();
    }

    private void EndDialogue() 
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false); // Corrected here
        //FindObjectOfType<Spawner>()?.DialogueFinished();
    }
}
