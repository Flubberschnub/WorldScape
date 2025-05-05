using UnityEngine;

// Makes the attached object always face the main camera.
// This is used for 2D effects like the sun or moon visuals,
// so they appear to rotate toward the player regardless of camera movement.
public class BillboardToCamera : MonoBehaviour
{
    void LateUpdate()
    {
        // Rotate the object to face the camera each frame
        if (Camera.main != null)
            transform.LookAt(Camera.main.transform);
    }
}
