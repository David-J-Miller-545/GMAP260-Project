using UnityEngine;

public class CameraHorizontalMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 3f;         // Starting speed
    [SerializeField] private float accelerationRate = 4f;   // How quickly it accelerates
    [SerializeField] private float maxSpeed = 12f;          // Cap the speed

    public Transform following;

    private InputManager input => InputManager.Instance;

    private float accelerationTimer = 0f;  // Increases while holding movement
    private float currentSpeed = 0f;

    public static CameraHorizontalMover Instance { get; private set; }
    public float min_y = 3;


    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (GameStateController.Instance.Is(GameState.Live))
        {
            float axis = input.CameraMoveAxis;

            if (following == null)
            {

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
                if (transform.position.y > min_y)
                    transform.position = new Vector3(transform.position.x, Mathf.MoveTowards(transform.position.y, min_y, baseSpeed), transform.position.z);
            }
            else
            {
                float new_y = (following.position.y > min_y) ? following.position.y : min_y;
                transform.position = new Vector3(following.position.x, min_y, transform.position.z);
            }
        }
        
    }
}
