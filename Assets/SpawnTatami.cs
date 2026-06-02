using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnTatami : MonoBehaviour
{
    [Header("XR Rig References")]
    public Transform xrOrigin;
    public Transform playerCamera;

    [Header("Spawn Settings")]
    public GameObject tatamiPrefab;
    public float floorYPosition = 0f;
    public float liftAboveFloor = 0.002f;
    
    [Header("Hand Collision Settings")]
    public bool requireHandsTouchFloor = true;
    public float bothHandsHoldSeconds = 0.5f;
    
    // Private variables for hand tracking
    bool leftHandOnFloor, rightHandOnFloor;
    Vector3 leftPoint, rightPoint;
    float holdTimer = 0f;
    
    GameObject spawned;

    void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        if (xrOrigin == null)
        {
            var xrOriginComponent = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
            if (xrOriginComponent != null)
                xrOrigin = xrOriginComponent.transform;
        }
    }

    void Update()
    {
        if (spawned != null || tatamiPrefab == null) return;

        if (requireHandsTouchFloor)
        {
            if (leftHandOnFloor && rightHandOnFloor)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= bothHandsHoldSeconds)
                    SpawnAtPlayerFeet();
            }
            else
            {
                holdTimer = 0f;
            }
        }
        else
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                SpawnAtPlayerFeet();
        }
    }

    void SpawnAtPlayerFeet()
    {
        Vector3 spawnPosition = xrOrigin != null ? xrOrigin.position : Vector3.zero;
        spawnPosition.y = floorYPosition + liftAboveFloor;

        Quaternion rotation = Quaternion.identity;
        if (playerCamera != null)
        {
            Vector3 forward = playerCamera.forward;
            forward.y = 0;
            if (forward != Vector3.zero)
                rotation = Quaternion.LookRotation(forward);
        }

        spawned = Instantiate(tatamiPrefab, spawnPosition, rotation);
    }

    public void SetHandOnFloor(bool isLeft, bool onFloor, Vector3 contactPoint)
    {
        if (isLeft) { leftHandOnFloor = onFloor; leftPoint = contactPoint; }
        else        { rightHandOnFloor = onFloor; rightPoint = contactPoint; }
    }
}