using UnityEngine;

public class WheelPoweredLift2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DragFlywheel2D wheel;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Transform liftPoint;

    [Header("Lift")]
    [SerializeField] private float velocityForFullLift = 720f;
    [SerializeField] private float maxLiftForce = 20f;
    [SerializeField] private bool clockwiseGivesLift = true;

    [Header("Tilt")]
    [SerializeField] private Transform tiltPoint;
    [SerializeField] private float tiltTorque = 8f;
    [SerializeField] private float maxTiltAngle = 25f;

    private void Awake()
    {
        if (body == null)
            body = GetComponent<Rigidbody2D>();

        if (liftPoint == null)
            liftPoint = transform;

        if (tiltPoint == null)
            tiltPoint = transform;
    }

    private void FixedUpdate()
    {
        ApplyLift();
        ApplyTilt();
    }

    private void ApplyLift()
    {
        float wheelSpeed = wheel.AngularVelocity;

        if (!clockwiseGivesLift)
            wheelSpeed *= -1f;

        float throttle = Mathf.InverseLerp(0f, velocityForFullLift, wheelSpeed);
        float liftForce = throttle * maxLiftForce;

        Vector2 liftDirection = transform.up;

        body.AddForceAtPosition(liftDirection * liftForce, liftPoint.position, ForceMode2D.Force);
    }

    private void ApplyTilt()
    {
        float currentAngle = Mathf.DeltaAngle(0f, body.rotation);

        if (Mathf.Abs(currentAngle) >= maxTiltAngle)
            return;

        float tiltInput = 0f;

        if (UnityEngine.InputSystem.Keyboard.current.aKey.isPressed)
            tiltInput = 1f;
        else if (UnityEngine.InputSystem.Keyboard.current.dKey.isPressed)
            tiltInput = -1f;

        body.AddTorque(tiltInput * tiltTorque, ForceMode2D.Force);
    }
}