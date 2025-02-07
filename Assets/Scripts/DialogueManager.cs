using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour {
    public static DialogueManager Instance;

    public TextAsset inkJSONAsset; // Reference to the Ink JSON
    private Story story;

    public GameObject dialoguePanel;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private Image npcPortrait;
    [SerializeField] private TMP_Text dialogueText;
    public GameObject choicesPanel;
    [SerializeField] private Button choiceButtonPrefab;

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

        isDialogueActive = false;
        dialoguePanel.SetActive(false);
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
            
            List<Choice> choices = story.currentChoices;
            if (choices.Count > 0) DisplayChoices();
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
                button.GetComponentInChildren<TMP_Text>().text = choice.text;
                int choiceIndex = choice.index;
                button.onClick.AddListener(() => MakeChoice(choiceIndex));
                button.gameObject.SetActive(true);
                EnableAllChildren(button.gameObject);
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

        // nuke the buttons once we're done with them 
        foreach (Transform child in choicesPanel.transform) 
        {
            Destroy(child.gameObject);
        }
    }

    private void EndDialogue() 
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        //FindObjectOfType<Spawner>()?.DialogueFinished();
    }

    //helper method to help with the nightmare that is unity UI??
    private void EnableAllChildren(GameObject obj) {
        foreach (Transform child in obj.transform) {
            if (!child.gameObject.activeSelf) {
                Debug.Log($"Enabling child: {child.name}");
                child.gameObject.SetActive(true);
            }
            EnableAllChildren(child.gameObject);
        }
    }


}
