using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
[System.Serializable]
public class LightComponent
{
    [Header("Light Setting")]
    public Light2D targetLight;

   [Header("Intensity Settings")]
    public float normalIntensity = 1f;
    public float flickerIntensityMin = 0.1f;
    public float flickerIntensityMax = 0.8f;
}
public class LightFlicker : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private LightComponent[] targetLightComponents; // For Unity 2D Light
                                                  // If using standard Light component, use: [SerializeField] private Light targetLight;

    [Header("Flicker Parameters")]
    [SerializeField] private float minHoldTime = 0.5f;
    [SerializeField] private float maxHoldTime = 3f;
    [SerializeField] private int minFlickerCount = 2;
    [SerializeField] private int maxFlickerCount = 5;
    [SerializeField] private float minFlickerInterval = 0.05f;
    [SerializeField] private float maxFlickerInterval = 0.2f;

   

    [Header("Control")]
    [SerializeField] private bool isFlickering = true;
    [SerializeField] private bool keepLightOnWhenDisabled = true; // If true, light stays at normal intensity when disabled

    private Coroutine flickerCoroutine;
    private float[] originalIntensity;
    private bool isCurrentlyFlickering = false;

    void Start()
    {
        // Get light component if not assigned
        if (targetLightComponents.Length < 0) return;
        originalIntensity = new float[targetLightComponents.Length];

        for(int i =0; i<targetLightComponents.Length;i++)
        {
            originalIntensity[i] = targetLightComponents[i].targetLight.intensity;
            targetLightComponents[i].normalIntensity = originalIntensity[i];
        }

        // Start flickering if enabled
        if (isFlickering)
        {
            StartFlickering();
        }
    }
    void OnEnable()
    {
        if (isFlickering)
        {
            StartFlickering();
        }
    }
    void OnDisable()
    {
        StopFlickering();
    }

    /// <summary>
    /// Enables or disables the flickering effect
    /// </summary>
    /// <param name="enable">True to enable flickering, false to disable</param>
    public void SetFlickering(bool enable)
    {
        if (enable == isFlickering)
            return;

        isFlickering = enable;

        if (isFlickering)
        {
            StartFlickering();
        }
        else
        {
            StopFlickering();

            // Set light to normal intensity if keepLightOnWhenDisabled is true
            if (keepLightOnWhenDisabled && targetLightComponents.Length > 0)
            {
                for (int i = 0; i < targetLightComponents.Length; i++)
                {
                    targetLightComponents[i].targetLight.intensity = targetLightComponents[i].normalIntensity;
                }
            }
            else if (!keepLightOnWhenDisabled && targetLightComponents.Length >0)
            {
                for (int i = 0; i < targetLightComponents.Length; i++)
                {
                    targetLightComponents[i].targetLight.intensity = 0;
                } // Turn off completely
            }
        }
    }

    /// <summary>
    /// Toggles the flickering state
    /// </summary>
    public void ToggleFlickering()
    {
        SetFlickering(!isFlickering);
    }

    /// <summary>
    /// Gets the current flickering state
    /// </summary>
    public bool IsFlickering()
    {
        return isFlickering;
    }

    /// <summary>
    /// Starts the flickering coroutine
    /// </summary>
    private void StartFlickering()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
        }
        flickerCoroutine = StartCoroutine(FlickerRoutine());
    }

    /// <summary>
    /// Stops the flickering coroutine
    /// </summary>
    private void StopFlickering()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        // Reset light intensity to normal
        if (targetLightComponents.Length >0 && isCurrentlyFlickering)
        {
            for (int i = 0; i < targetLightComponents.Length; i++)
            {
                targetLightComponents[i].targetLight.intensity = targetLightComponents[i].normalIntensity;
            }
            isCurrentlyFlickering = false;
        }
    }

    /// <summary>
    /// Main flickering coroutine
    /// </summary>
    private IEnumerator FlickerRoutine()
    {
        while (isFlickering)
        {
            // Hold the light for a random duration
            float holdTime = Random.Range(minHoldTime, maxHoldTime);
            yield return new WaitForSeconds(holdTime);

            // Only flicker if still enabled after the hold time
            if (!isFlickering) break;

            // Determine random number of flickers
            int flickerCount = Random.Range(minFlickerCount, maxFlickerCount + 1);

            // Perform the flickers
            isCurrentlyFlickering = true;

            for (int i = 0; i < flickerCount; i++)
            {
                if (!isFlickering) break;

                // Flicker off or to random intensity
                for (int j = 0; j < targetLightComponents.Length; j++)
                {
                    float flickerIntensity = Random.Range(targetLightComponents[j].flickerIntensityMin, targetLightComponents[j].flickerIntensityMax);
                    targetLightComponents[j].targetLight.intensity = flickerIntensity;
                }
                // Random interval between flickers
                float flickerInterval = Random.Range(minFlickerInterval, maxFlickerInterval);
                yield return new WaitForSeconds(flickerInterval);

                if (!isFlickering) break;

                // Return to normal intensity between flickers
                for (int j = 0; j < targetLightComponents.Length; j++)
                {
                    targetLightComponents[j].targetLight.intensity = targetLightComponents[j].normalIntensity;
                }

                // Small pause between flickers
                if (i < flickerCount - 1)
                {
                    yield return new WaitForSeconds(flickerInterval);
                }
            }

            // Ensure light returns to normal after flickering
            if (isFlickering && targetLightComponents.Length>0)
            {
                for (int i = 0; i < targetLightComponents.Length; i++)
                {
                    targetLightComponents[i].targetLight.intensity = targetLightComponents[i].normalIntensity;
                }
            }

            isCurrentlyFlickering = false;
        }

        flickerCoroutine = null;
    }

}