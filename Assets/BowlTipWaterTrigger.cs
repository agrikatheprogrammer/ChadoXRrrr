using UnityEngine;

/// <summary>
/// Fills the ladle while its bowl is dipped in a WaterSource. If a KettleWater is found,
/// water is transferred from the kettle (draining it); otherwise the kettle is treated as
/// infinite (fallback). Gradual fill while dipped, so you scoop rather than fill instantly.
/// </summary>
public class BowlTipWaterTrigger : MonoBehaviour
{
    public LadleWater ladle;
    [Tooltip("The kettle being scooped from. If empty, found automatically from the WaterSource the ladle dips into.")]
    public KettleWater kettle;
    [Tooltip("How fast the ladle fills while dipped (units per second).")]
    public float scoopRate = 1.5f;

    void OnTriggerStay(Collider other)
    {
        if (ladle == null || !other.CompareTag("WaterSource")) return;

        float room = 1f - ladle.amount;
        if (room <= 0.0001f) return; // ladle already full

        float want = Mathf.Min(room, scoopRate * Time.deltaTime);

        var k = kettle != null ? kettle : other.GetComponentInParent<KettleWater>();
        if (k != null)
        {
            float taken = k.Take(want);  // drains the kettle
            ladle.Add(taken);
        }
        else
        {
            ladle.Add(want);             // no kettle assigned -> infinite source
        }
    }
}
