using UnityEngine;

// Intended to blend between a day gradient skybox and a starry night skybox
// based on the time of day in the DayNightCycle.
// Not currently used in the final version, but kept for reference and future polish.
public class StarSkyboxController : MonoBehaviour
{
    // Gradient skybox used during the day
    public Material daySkybox;

    // Stars skybox used at night (not assigned in final version)
    public Material nightSkybox;

    // Blend factor between day (1) and night (0)
    [Range(0f, 1f)]
    public float blendValue = 0f;

    // Reference to the DayNightCycle script for tracking time
    public DayNightCycle timeController;

    void Start()
    {
        if (daySkybox == null || nightSkybox == null)
        {
            Debug.LogWarning("Skyboxes not assigned in StarSkyboxController.");
        }
    }

    void Update()
    {
        if (RenderSettings.skybox != null && daySkybox != null && nightSkybox != null && timeController != null)
        {
            // Currently using a manual blend slider (blendValue), not synced with time
            float t = blendValue;

            // Only blend stars in/out during dusk and dawn transitions
            if (t >= 0.85f || t <= 0.15f)
            {
                float nightT = t >= 0.85f ? (t - 0.85f) / 0.15f : (0.15f - t) / 0.15f;
                blendValue = 1f - nightT; // Brightest at midnight
            }
            else
            {
                blendValue = 0f; // Fully day
            }

            // Blend the skybox material from day to night
            RenderSettings.skybox.Lerp(daySkybox, nightSkybox, blendValue);
        }
    }
}
