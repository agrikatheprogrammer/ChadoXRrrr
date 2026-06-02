using UnityEngine;

public class CupFillTrigger : MonoBehaviour
{
    public CupFill cup;
    public float receiveRate = 0.35f;
    public float ladlePourRate = 0.8f;

    [Tooltip("Plays ONLY while the cup is actively filling. Stops when not pouring into the cup or when the cup is full.")]
    public AudioSource pourAudio;

    private LadleWater activeLadle;
    private bool fillingThisFrame;

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

        if (fillingThisFrame && !pourAudio.isPlaying)
            pourAudio.Play();
        else if (!fillingThisFrame && pourAudio.isPlaying)
            pourAudio.Stop();

        fillingThisFrame = false;
    }
}
