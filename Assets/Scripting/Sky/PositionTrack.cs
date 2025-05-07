using Unity.VisualScripting;
using UnityEngine;

public class PositionTrack : MonoBehaviour
{
    // This script will parent the position of an object to another object in the scene, but not the rotation.

    [SerializeField] private Transform target; // The target object to follow
    [SerializeField] private Vector3 offset; // Offset from the target's position

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Check if the target is assigned
        if (target != null)
        {
            // Update the position of this object to follow the target's position with an offset
            transform.position = target.position + offset;
        }
        else
        {
            Debug.LogWarning("Target not assigned in PositionTrack script.");
        }
        
    }
}
