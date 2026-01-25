using UnityEngine;
using System.Collections;

public class AutoPlayOnSpawn : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Delay before playing animation")]
    public float delayBeforePlay = 0.5f;

    void Start()
    {
        StartCoroutine(PlayAnimationAfterDelay());
    }

    IEnumerator PlayAnimationAfterDelay()
    {
        // Wait for everything to initialize
        yield return new WaitForSeconds(delayBeforePlay);
        
        Debug.Log($"🎬 Attempting to play animation on {gameObject.name}");
        
        // Try to find and call PlayOpenScene method
        Component[] components = GetComponents<Component>();
        
        foreach (Component comp in components)
        {
            if (comp == null) continue;
            
            var method = comp.GetType().GetMethod("PlayOpenScene");
            if (method != null)
            {
                method.Invoke(comp, null);
                Debug.Log($"✅ Called PlayOpenScene on {comp.GetType().Name}!");
                yield break;
            }
        }
        
        Debug.LogWarning($"⚠️ PlayOpenScene method not found on {gameObject.name}");
    }
}