using UnityEngine;

/// <summary>
/// Plays a water-pouring sound while the ladle is actively pouring (has water AND tilted past
/// its pour angle), and stops when you stop pouring or it runs dry.
/// Attach to the ladle (or wherever your AudioSource is) and assign the LadleWater + AudioSource.
/// </summary>
public class PourSound : MonoBehaviour
{
    [Tooltip("The ladle's water state. Auto-found on this object if left empty.")]
    public LadleWater ladle;
    [Tooltip("The water-pouring AudioSource. Auto-found on this object if left empty.")]
    public AudioSource audioSource;
    [Tooltip("Loop the sound continuously while pouring (recommended for a stream sound).")]
    public bool loopWhilePouring = true;

    void Awake()
    {
        if (ladle == null) ladle = GetComponent<LadleWater>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.loop = loopWhilePouring;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (ladle == null || audioSource == null) return;

        bool pouring = ladle.HasWater && ladle.IsPouringDown();

        if (pouring && !audioSource.isPlaying)
            audioSource.Play();
        else if (!pouring && audioSource.isPlaying)
            audioSource.Stop();
    }
}
