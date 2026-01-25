using UnityEngine;

public class Cube_Skip_trigger : MonoBehaviour
{
    [SerializeField] private UIScript uiScript;
    
    // Update is called once per frame
    void OnTriggerEnter(Collider other) {
            uiScript.OnSkipTriggerEntered(); // or whichever trigger
    }
    
}
