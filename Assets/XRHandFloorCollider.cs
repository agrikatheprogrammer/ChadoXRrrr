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
        // Create a GameObject with collider for this hand joint
        colliderObject = new GameObject(isLeftHand ? "LeftHandCollider" : "RightHandCollider");
        
        // Add sphere collider (trigger)
        var sphereCol = colliderObject.AddComponent<SphereCollider>();
        sphereCol.radius = 0.05f; // 5cm radius
        sphereCol.isTrigger = true;
        
        // Add kinematic rigidbody
        var rb = colliderObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        
        // Add HandScript
        handScript = colliderObject.AddComponent<HandScript>();
        handScript.manager = manager;
        handScript.isLeftHand = isLeftHand;
    }
    
    void Update()
    {
        // Get the hand subsystem
        var subsystem = GetHandSubsystem();
        if (subsystem == null) return;
        
        // Get the correct hand
        var hand = isLeftHand ? subsystem.leftHand : subsystem.rightHand;
        
        // Get the joint we're tracking
        if (hand.GetJoint(jointToTrack).TryGetPose(out var pose))
        {
            // Move our collider to match the tracked joint position
            colliderObject.transform.position = pose.position;
            colliderObject.transform.rotation = pose.rotation;
        }
    }
    
    private XRHandSubsystem GetHandSubsystem()
    {
        var subsystems = new System.Collections.Generic.List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);
        return subsystems.Count > 0 ? subsystems[0] : null;
    }
}