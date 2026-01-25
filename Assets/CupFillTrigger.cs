using UnityEngine;

public class CupFillTrigger : MonoBehaviour
{
    public CupFill cup; // Reference to the CupFill script
    public float receiveRate = 0.35f;      // Speed at which the cup fills (0..1 per second)
    public float ladlePourRate = 0.8f;    // Speed at which the ladle drains (0..1 per second)

    void OnTriggerStay(Collider other)
    {
        // Ensure a ladle with water is pouring into the cup
        var ladle = other.GetComponentInParent<LadleWater>();
        if (ladle == null || !ladle.HasWater || !ladle.IsPouringDown()) return;

        float dt = Time.deltaTime;

        // Move water from the ladle to the cup
        float poured = ladle.Pour(dt, ladlePourRate);
        cup.AddWater(poured * receiveRate); // Fill the cup dynamically
    }
}