using UnityEngine;

public class PlayAnimOnStart : MonoBehaviour
{
    void Start()
    {
        // Call PlayOpenScene method on THIS GameObject
        SendMessage("PlayOpenScene", SendMessageOptions.DontRequireReceiver);
    }
}