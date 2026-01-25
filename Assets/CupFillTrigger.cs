using UnityEngine;

public class CupFillTrigger : MonoBehaviour
{
    public CupFill cup;
    public float receiveRate = 0.35f; // how fast cup fills (0..1 per second)
    public float ladlePourRate = 0.8f; // how fast ladle drains (0..1 per second)

    void OnTriggerStay(Collider other)
    {
        // Ladle (or its child) must be inside fill zone
        var ladle = other.GetComponentInParent<LadleWater>();
        if (ladle == null) return;

        if (!ladle.HasWater) return;
        if (!ladle.IsPouringDown()) return;

        float dt = Time.deltaTime;

        // Drain ladle and fill cup
        float poured = ladle.Pour(dt, ladlePourRate);
        cup.AddWater(poured * receiveRate); // scale so it feels right
    }
}
