using System.Collections;
using UnityEngine;

public class WindEffect : MonoBehaviour
{
    // Wind parameters
    public float windStrength = 1f;
    public float windFrequency = 1f;
    public float maxXOffset = 1f; // Maximum horizontal offset from the original position

    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Store the initial position of the tree
        initialPosition = transform.position;

        // Start a coroutine to apply wind effect
        StartCoroutine(ApplyWindEffect());
    }

    IEnumerator ApplyWindEffect()
    {
        while (true)
        {
            // Calculate wind displacement based on sine wave
            float displacement = Mathf.Sin(Time.time * windFrequency) * windStrength;

            // Apply displacement to the tree object's position within the maximum offset
            Vector3 newPosition = initialPosition + Vector3.right * Mathf.Clamp(displacement, -maxXOffset, maxXOffset);
            transform.position = newPosition;

            // Wait for the next frame
            yield return null;
        }
    }
}
