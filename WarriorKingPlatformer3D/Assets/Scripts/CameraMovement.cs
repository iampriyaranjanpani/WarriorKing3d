using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform target; // Reference to the player's transform
    [SerializeField] float smoothSpeed = 0.5f; // Smoothing factor for camera movement
    public float minY = -3f; // Minimum Y position for the camera
    public float maxY = 3f; // Maximum Y position for the camera

    private Vector3 offset; // Offset between the camera and the player

    private void Start()
    {
        // Calculate the initial offset between the camera and the player
        offset = transform.position - target.position;
    }

    private void FixedUpdate()
    {
        // Calculate the target position for the camera
        Vector3 targetPosition = target.position + offset;
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY); // Clamp the Y position within the specified range

        // Smoothly move the camera towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
