using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager> {
    private Queue<string> sentences;

    [Header("UI Elements")]
    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI dialogueText;
    public TMPro.TextMeshProUGUI continueButton;
    public Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private int frequencyLevel = 3;
    [Range(-3.0f, 3.0f)]
    [SerializeField] private float minPitch = 0.85f;
    [Range(-3.0f, 3.0f)]
    [SerializeField] private float maxPitch = 1.10f;
    [SerializeField] private bool isRandomSFX = false;
    [SerializeField] private bool isRandomNotPred = true;
    [SerializeField] private bool isPredictableSFX = true; // Make this opposite of isRandomSFX
    [SerializeField] private float volume = 0.65f;

    private void Start() {
        sentences = new Queue<string>();

        if (audioManager == null) {
            audioManager = FindObjectOfType<AudioManager>();
        }
    }

    private void Update() {
        if (audioManager == null) {
            audioManager = FindObjectOfType<AudioManager>();
        }

        SetContinueButton();
    }

    public void StartDialogue(Dialogue dialogue) {
        animator.SetBool("IsOpen", true); // Open the dialogue box
        
        nameText.text = dialogue.name;

        sentences.Clear(); // Clear the queue from previous sentences

        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence); // Add the sentences to the queue
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines(); // Stop the previous sentence from being typed
        StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence) {
        dialogueText.text = "";
        yield return new WaitForSeconds(0.35f);
        int counter = 0;
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;

            if ((counter % frequencyLevel == 0) && audioManager != null) {
                if (!isPredictableSFX){
                    float randomPitch = Random.Range(minPitch, maxPitch);

                    if (isRandomNotPred) {
                        audioManager.MakeAndPlaySFXVariable("TextSound", randomPitch, volume);
                    } else {
                        int randomIndex = Random.Range(0, 2);

                        if (randomIndex == 0) {
                            audioManager.MakeAndPlaySFXVariable("TextSound", randomPitch, volume);
                        } else {
                            audioManager.MakeAndPlaySFXVariable("TextSound1", randomPitch, volume);
                        }
                    }
                } else {
                    int currentCharHash = letter.GetHashCode();
                    int predIndex = currentCharHash % 2;

                    int minPitchInt = (int)(minPitch * 100);
                    int maxPitchInt = (int)(maxPitch * 100);
                    int pitchRange = maxPitchInt - minPitchInt;

                    int randomRange = Random.Range(0, 3);

                    if (pitchRange != 0) {
                        int predPitchInt = minPitchInt + (currentCharHash % pitchRange);
                        float predPitch = predPitchInt / 100.0f;

                        if (randomRange == 0) {
                            audioManager.MakeAndPlaySFXVariable("TextSound", predPitch, volume);
                        } else if (randomRange == 1) {
                            audioManager.MakeAndPlaySFXVariable("TextSound3", predPitch, volume);
                        } else {
                            audioManager.MakeAndPlaySFXVariable("TextSound4", predPitch, volume);
                        }
                    } else {
                        audioManager.MakeAndPlaySFXVariable("TextSound", minPitch, volume);
                    }
                }
            }

            counter++;

            yield return new WaitForSeconds(0.03f);
            // yield return null;
        }
    }

    private void SetContinueButton() {
        if (sentences.Count == 0) {
            continueButton.text = "End >>>";
        } else {
            continueButton.text = "Continue >>>";
        }
    }

    public void EndDialogue() {
        animator.SetBool("IsOpen", false); // Close the dialogue box
    }
}
