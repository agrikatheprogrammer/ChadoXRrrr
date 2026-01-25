using UnityEngine;

public class SpawnTatami : MonoBehaviour
{
    [Header("XR Rig References")]
    [Tooltip("Leave empty to auto-find")]
    public Transform xrOrigin;
    
    [Tooltip("Leave empty to auto-find Main Camera")]
    public Transform playerCamera;

    [Header("Spawn Settings")]
    [Tooltip("Drag your mat prefab here")]
    public GameObject tatamiPrefab;
    
    [Tooltip("Drag your character prefab here")]
    public GameObject characterPrefab;
    
    public float floorYPosition = 0f;
    public float liftAboveFloor = 0.002f;
    
    [Header("Hand Collision Settings")]
    [Tooltip("If checked, requires both hands on floor. If unchecked, press SPACE to spawn")]
    public bool requireHandsTouchFloor = true;
    
    [Tooltip("How long both hands must stay on floor")]
    public float bothHandsHoldSeconds = 0.5f;
    
    [Header("Character Animation")]
    [Tooltip("Delay before playing character animation")]
    public float characterSpawnDelay = 0.5f;
    
    // Private variables for hand tracking
    bool leftHandOnFloor, rightHandOnFloor;
    Vector3 leftPoint, rightPoint;
    float holdTimer = 0f;
    
    GameObject spawned;
    GameObject spawnedCharacter;

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
            if (Input.GetKeyDown(KeyCode.Space))
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

        // Spawn character
        if (characterPrefab != null)
        {
            Vector3 charPos = spawnPosition;
            charPos.y = floorYPosition;
            
            spawnedCharacter = Instantiate(characterPrefab, charPos, rotation);
            Debug.Log($"✅ Character spawned at: {charPos}");
            
            // Play animation after delay
            StartCoroutine(PlayCharacterAnimationAfterDelay());
        }
        else
        {
            Debug.LogWarning("⚠️ No character prefab assigned!");
        }
    }

    // ⭐ THIS METHOD IS CALLED BY HandScript - REQUIRED!
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
        
        // Show current status
        Debug.Log($"📊 Current state - Left:{leftHandOnFloor}, Right:{rightHandOnFloor}");
    }

    System.Collections.IEnumerator PlayCharacterAnimationAfterDelay()
    {
        yield return new WaitForSeconds(characterSpawnDelay);
        
        if (spawnedCharacter != null)
        {
            // Try to find and call PlayOpenScene method
            Component[] components = spawnedCharacter.GetComponents<Component>();
            
            foreach (Component comp in components)
            {
                if (comp == null) continue;
                
                var method = comp.GetType().GetMethod("PlayOpenScene");
                if (method != null)
                {
                    method.Invoke(comp, null);
                    Debug.Log($"✅ Triggered PlayOpenScene on spawned character!");
                    yield break;
                }
            }
            
            Debug.LogWarning("⚠️ PlayOpenScene method not found on character!");
        }
        else
        {
            Debug.LogError("❌ Spawned character is null!");
        }
    }
}