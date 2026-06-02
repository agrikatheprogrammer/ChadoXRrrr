using UnityEngine;
using System.Collections;

public class AutoPlayOnSpawn : MonoBehaviour
{
    [Tooltip("Delay before playing animation")]
    public float delayBeforePlay = 0.5f;

    void Start()
    {
        if (delayBeforePlay > 0f)
            StartCoroutine(PlayAfterDelay());
        else
            SendMessage("PlayOpenScene", SendMessageOptions.DontRequireReceiver);
    }

    IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforePlay);
        SendMessage("PlayOpenScene", SendMessageOptions.DontRequireReceiver);
    }
}