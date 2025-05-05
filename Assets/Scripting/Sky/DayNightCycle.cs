using UnityEngine;

// This script controls the day-night cycle in the game by rotating the sun and moon,
// adjusting their intensity, and tracking the time of day as a normalized value (0–1).
public class DayNightCycle : MonoBehaviour
{
    // Length of a full in-game day (0 to 1) in real-world seconds
    public float dayLengthInSeconds = 60f;

    // The pivot transform that rotates the sun visual and light
    public Transform sunPivot;

    // The directional light representing sunlight
    public Light sunLight;

    // The pivot transform that rotates the moon visual and light
    public Transform moonPivot;

    // The directional light representing moonlight
    public Light moonLight;

    // Current normalized time (0 = midnight, 0.5 = noon, 1 = midnight again)
    [Range(0, 1)]
    public float currentTime = 0f;

    // Public getter for other scripts to access the current time of day
    public float TimeOfDay => currentTime;

    void Update()
    {
        // Advance time
        currentTime += Time.deltaTime / dayLengthInSeconds;
        if (currentTime > 1f) currentTime -= 1f;

        // Calculate rotation angles (sun and moon both follow arcs from -90° to 270°)
        // The sun is offset by 12 hours (0.5) to ensure it peaks at noon
        float sunAngle = Mathf.Lerp(-90f, 270f, (currentTime + 0.5f) % 1f);
        float moonAngle = Mathf.Lerp(-90f, 270f, (currentTime + 0.5f) % 1f);

        // Apply rotation to the sun's pivot (X-axis arc)
        if (sunPivot != null)
            sunPivot.localRotation = Quaternion.Euler(sunAngle, 0f, 0f);

        // Apply rotation to the moon's pivot (X-axis arc)
        if (moonPivot != null)
            moonPivot.localRotation = Quaternion.Euler(moonAngle, 0f, 0f);

        // Adjust sun light intensity based on time of day (peaks at noon, zero at night)
        if (sunLight != null)
        {
            float sunIntensity = Mathf.Clamp01(Mathf.Cos((currentTime - 0.5f) * Mathf.PI * 2));
            sunLight.intensity = sunIntensity * 1.5f;
        }

        // Adjust moon light intensity (peaks at midnight, zero during the day)
        if (moonLight != null)
        {
            float moonIntensity = Mathf.Clamp01(Mathf.Cos((currentTime + 0.5f) * 2f * Mathf.PI));
            moonLight.intensity = moonIntensity * 0.5f;
        }
    }
}
