using UnityEngine;

public class CameraHorizontalMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 3f;         // Starting speed
    [SerializeField] private float accelerationRate = 4f;   // How quickly it accelerates
    [SerializeField] private float maxSpeed = 12f;          // Cap the speed

    private InputManager input => InputManager.Instance;

    private float accelerationTimer = 0f;  // Increases while holding movement
    private float currentSpeed = 0f;

    private void Update()
    {
        float axis = input.CameraMoveAxis;

        if (input.IsCameraMoving)
        {
            // Holding left or right → accelerate
            accelerationTimer += Time.deltaTime;
        }
        else
        {
            // Axis released → reset acceleration
            accelerationTimer = 0f;
        }

        // Calculate speed based on how long the key is held
        currentSpeed = Mathf.Lerp(baseSpeed, maxSpeed, accelerationTimer * accelerationRate);

        // Actually move the camera
        transform.position += Vector3.right * axis * currentSpeed * Time.deltaTime;
    }
}
