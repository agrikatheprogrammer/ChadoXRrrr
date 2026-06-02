using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class UIScript : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image displayImage;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button startButton;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource buttonClickAudioSource;

    [Header("XR Collider Triggers")]
    [SerializeField] private GameObject startButtonTrigger;
    [SerializeField] private GameObject previousButtonTrigger;
    [SerializeField] private GameObject nextButtonTrigger;
    [SerializeField] private GameObject skipButtonTrigger;

    [Header("Sprite Sets")]
    [SerializeField] private Sprite startSprite;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] actionSprites;

    [Header("Voice Clips")]
    [SerializeField] private AudioClip startVoiceClip;
    [SerializeField] private AudioClip[] introVoiceClips;
    [SerializeField] private AudioClip[] actionVoiceClips;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Events")]
    [Tooltip("Fired when the whole narration finishes. Wire to CeremonyDirector.Finish.")]
    public UnityEvent onNarrationFinished;

    private int currentIndex = 0;
    private int currentSetIndex = 0; // 0 = sprites, 1 = actionSprites
    private Sprite[][] spriteSets;
    private AudioClip[][] voiceClipSets;
    private bool hasStarted = false;

    void Start()
    {
        // Initialize sprite sets array
        spriteSets = new Sprite[][] { sprites, actionSprites };
        voiceClipSets = new AudioClip[][] { introVoiceClips, actionVoiceClips };

        // Add listeners to buttons
        previousButton.onClick.AddListener(ShowPrevious);
        nextButton.onClick.AddListener(ShowNext);
        skipButton.onClick.AddListener(SkipToNextSet);
        startButton.onClick.AddListener(OnStartButtonPressed);

        // Hide all navigation buttons and triggers initially
        previousButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        
        if (previousButtonTrigger != null) previousButtonTrigger.SetActive(false);
        if (nextButtonTrigger != null) nextButtonTrigger.SetActive(false);
        if (skipButtonTrigger != null) skipButtonTrigger.SetActive(false);

        // Show start screen
        ShowStartScreen();
    }

    void ShowStartScreen()
    {
        hasStarted = false;
        
        // Show start sprite
        if (displayImage != null && startSprite != null)
        {
            displayImage.sprite = startSprite;
        }

        // Play start voice clip
        PlayVoiceClip(startVoiceClip);

        // Only the start button and trigger should be visible
        startButton.gameObject.SetActive(true);
        if (startButtonTrigger != null) startButtonTrigger.SetActive(true);
    }

    void OnStartButtonPressed()
    {
        PlayButtonClickSound();
        
        hasStarted = true;
        
        // Hide start button and trigger, show navigation buttons and triggers
        startButton.gameObject.SetActive(false);
        if (startButtonTrigger != null) startButtonTrigger.SetActive(false);
        
        previousButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(true);
        
        if (previousButtonTrigger != null) previousButtonTrigger.SetActive(true);
        if (nextButtonTrigger != null) nextButtonTrigger.SetActive(true);
        if (skipButtonTrigger != null) skipButtonTrigger.SetActive(true);

        // Display the first sprite of the first set
        currentSetIndex = 0;
        currentIndex = 0;
        UpdateDisplay();
    }

    void ShowPrevious()
    {
        Sprite[] currentSet = GetCurrentSpriteSet();
        if (currentSet.Length == 0) return;

        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = currentSet.Length - 1; // Wrap to last sprite
        }

        UpdateDisplay();
    }

    void ShowNext()
    {
        Sprite[] currentSet = GetCurrentSpriteSet();
        if (currentSet.Length == 0) return;

        currentIndex++;
        if (currentIndex >= currentSet.Length)
        {
            // Check if we just finished the actionSprites set
            if (currentSetIndex == 1) // 1 = actionSprites
            {
                ActionFinish();
            }
            
            // Check if we just finished the last sprite of the last set
            if (currentSetIndex == spriteSets.Length - 1)
            {
                Finish();
            }
            
            currentIndex = 0; // Wrap to first sprite
        }

        UpdateDisplay();
    }
    
    void ActionFinish()
    {
        Debug.Log("Action Finish triggered!");
        // Add your custom logic here
        // For example:
        // - Load next scene
        // - Show completion UI
        // - Trigger game events
        // - etc.
    }
    
    void TriggerAction1()
    {
        Debug.Log("TriggerAction1 - 1st ActionSprite displayed");
        // Add your custom logic for the 1st ActionSprite here
    }
    
    void TriggerAction2()
    {
        Debug.Log("TriggerAction2 - 2nd ActionSprite displayed");
        // Add your custom logic for the 2nd ActionSprite here
    }
    
    void TriggerAction3()
    {
        Debug.Log("TriggerAction3 - 3rd ActionSprite displayed");
        // Add your custom logic for the 3rd ActionSprite here
    }
    
    void TriggerAction4()
    {
        Debug.Log("TriggerAction4 - 7th ActionSprite displayed");
        // Add your custom logic for the 7th ActionSprite here
    }
    
    void Finish()
    {
        Debug.Log("All sprites finished!");
        // Add your custom logic here when all sprite sets are complete
        // For example:
        // - Show completion screen
        // - Return to main menu
        // - Save progress
        // - Unlock rewards
        // - etc.

        onNarrationFinished?.Invoke();
        gameObject.SetActive(false);
    }

    void SkipToNextSet()
    {
        currentSetIndex++;
        if (currentSetIndex >= spriteSets.Length)
        {
            currentSetIndex = 0; // Wrap to first set
        }

        currentIndex = 0; // Reset to first sprite in new set
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        Sprite[] currentSet = GetCurrentSpriteSet();
        if (currentSet.Length == 0 || displayImage == null) return;

        displayImage.sprite = currentSet[currentIndex];

        // Trigger specific actions for ActionSprites set
        if (currentSetIndex == 1) // ActionSprites set
        {
            if (currentIndex == 0) TriggerAction1();
            else if (currentIndex == 1) TriggerAction2();
            else if (currentIndex == 2) TriggerAction3();
            else if (currentIndex == 6) TriggerAction4(); // 7th sprite (0-indexed)
        }

        // Play corresponding voice clip after a small delay to let button click finish
        AudioClip[] currentVoiceSet = GetCurrentVoiceClipSet();
        if (currentVoiceSet != null && currentIndex < currentVoiceSet.Length)
        {
            // Only start coroutine if GameObject is active
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(PlayVoiceWithDelay(currentVoiceSet[currentIndex], 0.1f));
            }
        }

        // Update button interactability
        previousButton.interactable = currentSet.Length > 1;
        nextButton.interactable = currentSet.Length > 1;
        skipButton.interactable = spriteSets.Length > 1;

        // Hide skip button when in actionSprites set (index 1)
        if (currentSetIndex == 1)
        {
            skipButton.gameObject.SetActive(false);
            if (skipButtonTrigger != null) skipButtonTrigger.SetActive(false);
        }
        else
        {
            skipButton.gameObject.SetActive(true);
            if (skipButtonTrigger != null) skipButtonTrigger.SetActive(true);
        }
    }
    
    IEnumerator PlayVoiceWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayVoiceClip(clip);
    }

    Sprite[] GetCurrentSpriteSet()
    {
        if (currentSetIndex >= 0 && currentSetIndex < spriteSets.Length)
            return spriteSets[currentSetIndex];
        return System.Array.Empty<Sprite>();
    }

    AudioClip[] GetCurrentVoiceClipSet()
    {
        if (currentSetIndex >= 0 && currentSetIndex < voiceClipSets.Length)
        {
            return voiceClipSets[currentSetIndex];
        }
        return null;
    }

    void PlayVoiceClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.Stop(); // Stop any currently playing clip
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    void PlayButtonClickSound()
    {
        if (buttonClickAudioSource != null && buttonClickSound != null)
        {
            buttonClickAudioSource.PlayOneShot(buttonClickSound);
        }
    }

    // XR Trigger Methods - Call these from your XR interaction scripts
    public void OnStartTriggerEntered()
    {
        OnStartButtonPressed();
    }

    public void OnPreviousTriggerEntered()
    {
        ShowPrevious();
    }

    public void OnNextTriggerEntered()
    {
        ShowNext();
    }

    public void OnSkipTriggerEntered()
    {
        SkipToNextSet();
    }

    // Optional: Set sprite sets programmatically
    public void SetSpriteSets(Sprite[] newSprites, Sprite[] newActionSprites)
    {
        sprites = newSprites;
        actionSprites = newActionSprites;
        spriteSets = new Sprite[][] { sprites, actionSprites };
        currentSetIndex = 0;
        currentIndex = 0;
        UpdateDisplay();
    }

    void OnDestroy()
    {
        // Clean up listeners
        previousButton.onClick.RemoveListener(ShowPrevious);
        nextButton.onClick.RemoveListener(ShowNext);
        skipButton.onClick.RemoveListener(SkipToNextSet);
        startButton.onClick.RemoveListener(OnStartButtonPressed);
    }
}