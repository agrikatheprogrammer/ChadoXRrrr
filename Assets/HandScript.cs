using UnityEngine;

public class HandScript : MonoBehaviour
{
    public SpawnTatami manager;
    public bool isLeftHand;

    int floorContacts = 0;
    Vector3 lastContactPoint;

    void Reset()
    {
        // Make sure we have a kinematic rigidbody for trigger callbacks
        var rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Floor")) return;
        floorContacts++;
        lastContactPoint = other.ClosestPoint(transform.position);
        manager?.SetHandOnFloor(isLeftHand, true, lastContactPoint);
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Floor")) return;
        lastContactPoint = other.ClosestPoint(transform.position);
        manager?.SetHandOnFloor(isLeftHand, true, lastContactPoint);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Floor")) return;

        floorContacts = Mathf.Max(0, floorContacts - 1);
        if (floorContacts == 0)
            manager?.SetHandOnFloor(isLeftHand, false, lastContactPoint);
    }

}