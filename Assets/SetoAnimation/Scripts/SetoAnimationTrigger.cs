using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using NaughtyAttributes;

public class SetoAnimationTrigger : MonoBehaviour

{
    private Animator anim;
    
    private int layerIndexWalk;
    
    [Header("Voice-over")]
    [SerializeField] AudioSource voiceSource;           // drag the AudioSource here
    [SerializeField] AudioClip voice_Greeting;
    
    // ===== Public APIs =====
    
    
    [Button("Play Open Scene")]
    public void PlayOpenScene()
    {
        anim.SetTrigger("Animation_01");
        //PlayVoice(voice_Greeting);
    }
    
    [Button("Play Joyful Jump")]
    public void PlayJoyfulJump()
    {
        anim.SetTrigger("Animation_02");
        //PlayVoice(voice_Greeting);
    }
    
    [Button("Play Disappointed")]
    public void PlayDisappointed()
    {
        anim.SetTrigger("Animation_03");
        //PlayVoice(voice_Greeting);
    }
    
    [Button("Play Walking")]
    public void PlayWalking()
    {
        anim.SetLayerWeight(layerIndexWalk, 1);
    }
    
    [Button("Stop Walking")]
    public void StopWalking()
    {
        anim.SetLayerWeight(layerIndexWalk, 0);
    }
    
    [Header("Expressions")]
    [SerializeField] private GameObject expressionNormal;
    [SerializeField] private GameObject expressionHappy;
    [SerializeField] private GameObject expressionAngry;
    
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        HideAllExpression();
        ShowNormal();
        
        layerIndexWalk = anim.GetLayerIndex("Walking Layer");
        anim.SetLayerWeight(layerIndexWalk, 0);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (anim == null) return;

        // Drive the facial expression from the current animation clip.
        // Guard against an empty layer (Animator has no clip this frame, e.g. Timeline-driven)
        // so we never index an empty array — that was throwing every frame and tanking FPS.
        var clipInfo = anim.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length == 0 || clipInfo[0].clip == null) return;

        string m_ClipName = clipInfo[0].clip.name;
        if (m_ClipName.Contains("StandCoverToLook") || m_ClipName.Contains("LookAround") || m_ClipName.Contains("Joyful Jump"))
        {
            ShowHappy();
        }
        else if (m_ClipName.Contains("Angry") || m_ClipName.Contains("Disappointed"))
        {
            ShowAngry();
        }
        else
        {
            ShowNormal();
        }
    }
    
    private void HideAllExpression()
    {
        expressionAngry.SetActive(false);
        expressionHappy.SetActive(false);
        expressionNormal.SetActive(false);
    }




    /// <summary>
    /// Expressions
    /// </summary>
    private void ShowNormal()
    {
        HideAllExpression();
        expressionNormal.SetActive(true);
    }
    private void ShowHappy()
    {
        HideAllExpression();
        expressionHappy.SetActive(true);
    }
    private void ShowAngry()
    {
        HideAllExpression();
        expressionAngry.SetActive(true);
    }
    
    // ===== Helpers =====
    void PlayVoice(AudioClip clip)
    {
        if (voiceSource && clip) voiceSource.PlayOneShot(clip);
    }
    
    
}
