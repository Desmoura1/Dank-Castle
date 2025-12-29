using UnityEngine;
using System.Collections;

public class SquashAndStretch : MonoBehaviour
{
    [Header("Animation Settings")]
    public AnimationCurve squashCurve; // The shape of the bounce
    public float duration = 0.5f;      // How fast the squeeze is
    public float intensity = 0.2f;     // How "stretchy" it is
    public float loopDelay = 5f;       // Time between breaths

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        // Start the infinite loop
        StartCoroutine(BreathingLoop());
    }

    IEnumerator BreathingLoop()
    {
        while (true)
        {
            yield return StartCoroutine(PlaySquash());
            yield return new WaitForSeconds(loopDelay);
        }
    }

    IEnumerator PlaySquash()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / duration;

            // Get the value from your custom curve
            float curveValue = squashCurve.Evaluate(percentage);

            // Calculate new scale (Squash Y, Stretch X to preserve volume)
            float squashY = 1 + (curveValue * intensity);
            float stretchX = 1 / squashY; 

            transform.localScale = new Vector3(
                originalScale.x * stretchX, 
                originalScale.y * squashY, 
                originalScale.z
            );

            yield return null;
        }

        // Reset to perfect original scale
        transform.localScale = originalScale;
    }
}