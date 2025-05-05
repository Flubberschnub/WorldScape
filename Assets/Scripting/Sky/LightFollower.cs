/*using UnityEngine;

public class LightFollower : MonoBehaviour
{
    public DayNightCycle timeController;
    public bool isMoon = false; // Toggle for Moon or Sun

    void Update()
    {
        if (timeController == null) return;

        float t = timeController.TimeOfDay;

        if (isMoon)
            t = (t + 0.5f) % 1f; // Offset moon by 12 hours (opposite arc)

        float angle = Mathf.Lerp(-90f, 270f, t); // Rotates from below horizon up and over
        transform.rotation = Quaternion.Euler(angle, 0f, 0f);
    }
}
*/