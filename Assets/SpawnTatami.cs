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
        // Auto-find camera
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;
            
        // Auto-find XR Origin
        if (xrOrigin == null)
        {
            var xrOriginComponent = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
            if (xrOriginComponent != null)
                xrOrigin = xrOriginComponent.transform;
        }
        
        Debug.Log("🎋 TatamiManager initialized!");
    }

    void Update()
    {
        if (spawned != null) return; // Already spawned
        if (tatamiPrefab == null) return; // No prefab assigned

        // Method 1: Hand collision trigger
        if (requireHandsTouchFloor)
        {
            if (leftHandOnFloor && rightHandOnFloor)
            {
                holdTimer += Time.deltaTime;
                
                if (holdTimer >= bothHandsHoldSeconds)
                {
                    Debug.Log($"⏱️ Both hands held for {holdTimer:F2}s - SPAWNING!");
                    SpawnAtPlayerFeet();
                }
            }
            else
            {
                // Reset timer if hands not both on floor
                if (holdTimer > 0)
                {
                    holdTimer = 0f;
                    Debug.Log("⏱️ Timer reset - both hands not on floor");
                }
            }
        }
        // Method 2: Keyboard trigger (for testing)
        else
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Debug.Log("⌨️ SPACEBAR pressed - SPAWNING!");
                SpawnAtPlayerFeet();
            }
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

        // Spawn tatami mat
        spawned = Instantiate(tatamiPrefab, spawnPosition, rotation);
        Debug.Log($"✅ Tatami spawned at: {spawnPosition}");
    }

    // THIS METHOD IS CALLED BY HandScript
    public void SetHandOnFloor(bool isLeft, bool onFloor, Vector3 contactPoint)
    {
        if (isLeft)
        {
            leftHandOnFloor = onFloor;
            leftPoint = contactPoint;
            Debug.Log($"🟢 LEFT hand on floor: {onFloor} at {contactPoint}");
        }
        else
        {
            rightHandOnFloor = onFloor;
            rightPoint = contactPoint;
            Debug.Log($"🔵 RIGHT hand on floor: {onFloor} at {contactPoint}");
        }
        
        Debug.Log($"📊 Current state - Left:{leftHandOnFloor}, Right:{rightHandOnFloor}");
    }
}