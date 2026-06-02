using UnityEngine;

public class CupFillTrigger : MonoBehaviour
{
    public CupFill cup;
    public float receiveRate = 0.35f;
    public float ladlePourRate = 0.8f;

    [Tooltip("A seamless looping water-stream clip. Plays while the cup is filling.")]
    public AudioSource pourAudio;
    [Tooltip("Once the pour sound starts, keep playing at least this many seconds — so a fast fill still sounds like a real pour, not a short blip.")]
    public float minPourSoundSeconds = 2.5f;

    private LadleWater activeLadle;
    private bool fillingThisFrame;
    private float soundStartTime;

    void Awake()
    {
        if (pourAudio != null)
        {
            pourAudio.loop = true;         // seamless loop while pouring
            pourAudio.playOnAwake = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (activeLadle == null)
            activeLadle = other.GetComponentInParent<LadleWater>();
    }

    void OnTriggerStay(Collider other)
    {
        if (activeLadle == null || !activeLadle.HasWater || !activeLadle.IsPouringDown()) return;
        if (cup != null && cup.fill >= 0.999f) return; // cup full -> stop filling (and sound)

        float poured = activeLadle.Pour(Time.deltaTime, ladlePourRate);
        if (poured > 0f)
        {
            cup.AddWater(poured * receiveRate);
            fillingThisFrame = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (activeLadle != null && other.GetComponentInParent<LadleWater>() == activeLadle)
            activeLadle = null;
    }

    // Drive the pour sound from whether the cup actually took water this frame.
    // (OnTriggerStay stops firing when the ladle leaves the cup, so this naturally stops.)
    void LateUpdate()
    {
        if (pourAudio == null) return;

        if (fillingThisFrame)
        {
            if (!pourAudio.isPlaying)
            {
                pourAudio.Play();
                soundStartTime = Time.time;
            }
        }
        else if (pourAudio.isPlaying && Time.time - soundStartTime >= minPourSoundSeconds)
        {
            // Only stop once it's been playing the minimum time, so a quick fill isn't a blip.
            pourAudio.Stop();
        }

        fillingThisFrame = false;
    }
}
