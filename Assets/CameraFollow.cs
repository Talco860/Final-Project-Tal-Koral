using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the object you want the camera to follow
    public float minSmoothness = 5f; // Minimum smoothness of camera movement
    public float maxSmoothness = 16f; // Maximum smoothness of camera movement
    public Vector3 offset = new Vector3(0f, 0f, -5f); // Adjust the offset values

    void FixedUpdate()
    {
        if (target != null)
        {
            // Calculate the target position with the offset
            Vector3 targetPosition = target.position + offset;

            // Calculate the smoothness based on the car's speed
            float speedFactor = Mathf.Clamp01(target.GetComponent<Rigidbody2D>().velocity.magnitude / 50f); // Adjust 10f according to your car's max speed
            float currentSmoothness = Mathf.Lerp(maxSmoothness, minSmoothness, speedFactor);

            // Smoothly move the camera towards the target position using Lerp
            transform.position = Vector3.Lerp(transform.position, targetPosition, currentSmoothness * Time.deltaTime);
        }
    }
}
