using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CatapultArm : MonoBehaviour
{
    public enum CatapultState
    {
        Held,
        Drawing,
        Loosening,
        Free
    }

    [SerializeField] private GameObject projectile_to_spawn;
    [SerializeField] private GameObject projectile_spawner;
    [SerializeField] private GameObject current_ammo;
    public float angularAcceleration = 1000f; 
    public float maxAngularSpeed = 720f;
    public float aimSpeed = 120f;
    [SerializeField] private float top_z_rot = -35;
    [SerializeField] private float reload_z_rot = 30;
    [SerializeField] private float bot_z_rot = 35;
    [SerializeField] private CatapultState current_state = CatapultState.Held;
    [SerializeField] private bool armed = false;
    private float lastAngle;  // for velocity calculation
    private float angularVelocity = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reload();
    }

    // Update is called once per frame
    void Update()
    {
        var input = InputManager.Instance;

        if (input.IsArmMoving && GameStateController.Instance.Is(GameState.Live))
        {
            Debug.Log($"ArmMoving: {input.ArmMoveAxis}");
            armMove(input.ArmMoveAxis);
        }

        if (input.ReloadPressed) reload();
        if (input.FirePressed) fire();

        if (current_state == CatapultState.Free) arm_free_swing();
        else
        {
            lastAngle = transform.localEulerAngles.z;
            angularVelocity = 0;
        }
    }

    void armMove(float dir)
    {
        float currentZ = NormalizeAngle(transform.localEulerAngles.z);
        currentZ = Mathf.MoveTowards(currentZ, 0f, aimSpeed * dir * Time.deltaTime);
        if (Mathf.Abs(currentZ + bot_z_rot) > 0.5f && Mathf.Abs(currentZ - top_z_rot) > 0.5f)
        {
            transform.localEulerAngles = new Vector3(0, 0, currentZ);
        }
    }

    void arm_free_swing()
    {
        if (current_state == CatapultState.Free && GameStateController.Instance.Is(GameState.Live))
        {
            float currentZ = NormalizeAngle(transform.localEulerAngles.z);

            // Apply angular acceleration (speed up rotation)
            angularVelocity = Mathf.Min(angularVelocity + angularAcceleration * Time.deltaTime, maxAngularSpeed);

            // Move the arm forward (negative direction)
            currentZ = Mathf.MoveTowards(currentZ, top_z_rot, angularVelocity * Time.deltaTime);
            transform.localEulerAngles = new Vector3(0, 0, currentZ);

            // Release ammo near top of swing (when at or near maxRotation)
            if (!(current_ammo == null) && Mathf.Abs(currentZ - top_z_rot) < 1f)
            {
                ReleaseAmmo(angularVelocity);
            }

            // Stop the swing after reaching full motion
            if (Mathf.Abs(currentZ - top_z_rot) < 0.5f)
            {
                current_state = CatapultState.Held;
            }
        }
    }

    void ReleaseAmmo(float angularVelocity)
    {
        if (current_ammo == null)
            return;

        // Detach ammo
        current_ammo.transform.SetParent(null);
        current_ammo.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        // Compute throw direction (tangent to arm)
        Vector2 pivot = transform.position;
        Vector2 ammoPos = current_ammo.transform.position;
        Vector2 radius = ammoPos - pivot;

        // Tangent direction (perpendicular to radius)
        Vector2 tangentDir = new Vector2(radius.y, -radius.x).normalized;


        // Convert angular velocity (deg/s) to rad/s
        float angularVelRad = angularVelocity * Mathf.Deg2Rad;

        // Tangential linear velocity
        Vector2 launchVelocity = tangentDir * angularVelRad * radius.magnitude;

        current_ammo.GetComponent<Rigidbody2D>().linearVelocity = launchVelocity;
        current_ammo = null;
    }

    void fire()
    {
        if (current_state != CatapultState.Free && GameStateController.Instance.Is(GameState.Live))
        {
            current_state = CatapultState.Free;
            armed = false;
        }
    }

    void reload()
    {
        if (current_state == CatapultState.Held && GameStateController.Instance.Is(GameState.Live) && !armed)
        {
            GameObject spawnedProjectile = Instantiate(projectile_to_spawn, projectile_spawner.transform.position, 
                                                        projectile_spawner.transform.rotation);
            spawnedProjectile.transform.parent = transform;
            current_ammo = spawnedProjectile;
            current_ammo.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            armed = true;
        }
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        else if (angle < -180f)
            angle += 360f;
        return angle;
    }

}
