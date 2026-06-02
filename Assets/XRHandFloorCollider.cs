using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class XRHandFloorCollider : MonoBehaviour
{
    [Header("Setup")]
    public SpawnTatami manager;
    public bool isLeftHand;

    [Header("Which joint to track (recommend Palm or IndexTip)")]
    public XRHandJointID jointToTrack = XRHandJointID.Palm;

    private GameObject colliderObject;
    private XRHandSubsystem handSubsystem;

    // Reused across CacheHandSubsystem calls to avoid per-call allocation
    private static readonly List<XRHandSubsystem> s_SubsystemScratch = new List<XRHandSubsystem>();

    void Start()
    {
        colliderObject = new GameObject(isLeftHand ? "LeftHandCollider" : "RightHandCollider");
        colliderObject.transform.parent = transform;

        var sphereCol = colliderObject.AddComponent<SphereCollider>();
        sphereCol.radius = 0.05f;
        sphereCol.isTrigger = true;

        var rb = colliderObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var handScript = colliderObject.AddComponent<HandScript>();
        handScript.manager = manager;
        handScript.isLeftHand = isLeftHand;

        CacheHandSubsystem();
    }

    void OnEnable() => CacheHandSubsystem();

    void CacheHandSubsystem()
    {
        s_SubsystemScratch.Clear();
        SubsystemManager.GetSubsystems(s_SubsystemScratch);
        handSubsystem = s_SubsystemScratch.Count > 0 ? s_SubsystemScratch[0] : null;
    }

    void Update()
    {
        if (handSubsystem == null)
        {
            CacheHandSubsystem();
            return;
        }

        var hand = isLeftHand ? handSubsystem.leftHand : handSubsystem.rightHand;
        if (!hand.GetJoint(jointToTrack).TryGetPose(out var pose)) return;

        colliderObject.transform.SetPositionAndRotation(pose.position, pose.rotation);
    }
}