using UnityEngine;

public class Cube_Start_trigger : MonoBehaviour
{
    [SerializeField] private UIScript uiScript;
    
    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        uiScript.OnStartTriggerEntered();
    }
    
}
