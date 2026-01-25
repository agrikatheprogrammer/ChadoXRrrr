using UnityEngine;

public class BowlTipWaterTrigger : MonoBehaviour
{
    public LadleWater ladle;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("WaterSource")) return;
        ladle.FillInstant(); // unlimited kettle
    }
}
