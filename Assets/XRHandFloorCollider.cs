using UnityEngine;
using UnityEngine.XR.Hands;

public class XRHandFloorCollider : MonoBehaviour
{
    [Header("Setup")]
    public SpawnTatami manager;
    public bool isLeftHand;
    
    [Header("Which joint to track (recommend Palm or IndexTip)")]
    public XRHandJointID jointToTrack = XRHandJointID.Palm;
    
    private GameObject colliderObject;
    private HandScript handScript;
    
    void Start()
    {
        // Create the trigger collider as a visible child object
        colliderObject = new GameObject(
            isLeftHand ? "LeftHandCollider" : "RightHandCollider");
        colliderObject.transform.parent = this.transform;

        // Add collider & Rigidbody
        var sphereCol = colliderObject.AddComponent<SphereCollider>();
        sphereCol.radius = 0.05f;
        sphereCol.isTrigger = true;
        var rb = colliderObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // Add and setup HandScript
        handScript = colliderObject.AddComponent<HandScript>();
        handScript.manager = manager;
        handScript.isLeftHand = isLeftHand;
        
        Debug.Log($"[{name}] Collider created for {(isLeftHand ? "left" : "right")} hand.");
    }
    
    void Update()
    {
        // Get the hand subsystem
        var subsystem = GetHandSubsystem();
        if (subsystem == null)
        {
            Debug.LogWarning("No XRHandSubsystem found");
            return;
        }

        var hand = isLeftHand ? subsystem.leftHand : subsystem.rightHand;
        if (!hand.GetJoint(jointToTrack).TryGetPose(out var pose))
        {
            Debug.LogWarning($"Pose not tracked for {jointToTrack}");
            return;
        }

        colliderObject.transform.position = pose.position;
        colliderObject.transform.rotation = pose.rotation;
    }
    
    private XRHandSubsystem GetHandSubsystem()
    {
        var subsystems = new System.Collections.Generic.List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);
        return subsystems.Count > 0 ? subsystems[0] : null;
    }
}