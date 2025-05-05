using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Controls the color tint of the scene using post-processing based on the time of day.
// Intended to create smooth transitions between night and day color tones.
public class PostProcessingController : MonoBehaviour
{
    // Reference to the Global Volume in the scene
    public Volume volume;

    // Holds a reference to the Color Adjustments component inside the volume profile
    private ColorAdjustments colorAdjustments;

    // Color tint for nighttime (cool tones)
    public Color nightColor = new Color(0.2f, 0.3f, 0.6f);

    // Color tint for daytime (warm tones)
    public Color dayColor = new Color(1f, 0.95f, 0.8f);

    // Blend factor between night and day (0 = night, 1 = day)
    [Range(0, 1)]
    public float dayNightBlend = 0f;

    void Start()
    {
        // Try to extract the ColorAdjustments component from the volume
        if (volume != null && volume.profile.TryGet(out colorAdjustments))
        {
            Debug.Log("ColorAdjustments loaded!");
        }
        else
        {
            Debug.LogWarning("Couldn’t find ColorAdjustments in Volume Profile. Did you forget to add it?");
        }
    }

    void Update()
    {
        // Lerp the scene's color tint between night and day based on the blend value
        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = Color.Lerp(nightColor, dayColor, dayNightBlend);
        }
    }
}
