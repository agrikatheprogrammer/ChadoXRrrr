using UnityEngine;

public class CupFillTrigger : MonoBehaviour
{
    public CupFill cup;
    public float receiveRate = 0.35f;
    public float ladlePourRate = 0.8f;

    private LadleWater activeLadle;

    void OnTriggerEnter(Collider other)
    {
        if (activeLadle == null)
            activeLadle = other.GetComponentInParent<LadleWater>();
    }

    void OnTriggerStay(Collider other)
    {
        if (activeLadle == null || !activeLadle.HasWater || !activeLadle.IsPouringDown()) return;

        float poured = activeLadle.Pour(Time.deltaTime, ladlePourRate);
        cup.AddWater(poured * receiveRate);
    }

    void OnTriggerExit(Collider other)
    {
        if (activeLadle != null && other.GetComponentInParent<LadleWater>() == activeLadle)
            activeLadle = null;
    }
}