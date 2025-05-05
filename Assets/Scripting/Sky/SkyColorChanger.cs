using UnityEngine;

// Dynamically changes the skybox color throughout the day using gradients.
// This script updates the sky tint and ground color based on the current time of day.
public class SkyColorChanger : MonoBehaviour
{
    [Header("Skybox Material")]
    public Material skyboxMaterial; // The skybox material to modify

    [Header("Gradient Colors")]
    public Gradient skyGradient;    // Controls the top of the sky color over time
    public Gradient groundGradient; // Controls the horizon/ground color over time

    [Header("Time Controller")]
    public DayNightCycle timeController; // Reference to the day/night cycle script

    void Start()
    {
        // Set the global skybox at the start (optional if already assigned elsewhere)
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }
    }

    void Update()
    {
        // Only proceed if required references are assigned
        if (skyboxMaterial == null || timeController == null) return;

        float t = timeController.TimeOfDay;

        // Evaluate gradient colors based on current time
        Color skyColor = skyGradient.Evaluate(t);
        Color groundColor = groundGradient.Evaluate(t);

        // Apply colors to the skybox
        skyboxMaterial.SetColor("_SkyTint", skyColor);
        skyboxMaterial.SetColor("_GroundColor", groundColor);
    }
}
