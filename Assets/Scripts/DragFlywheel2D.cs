using UnityEngine;

public class DragFlywheel2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform wheel;
    [SerializeField] private Collider2D dragTarget;
    [SerializeField] private Transform visualCursor;
    [SerializeField] private Transform pedalVisual;

    [Header("Feel")]
    [SerializeField] private float driveStrength = 12f;
    [SerializeField] private float windUpResistance = 8f;
    [SerializeField] private float draggingDrag = 0.15f;
    [SerializeField] private float releaseDrag = 0.8f;
    [SerializeField] private float maxAngularVelocity = 900f;

    [Header("Optional Snapping")]
    [SerializeField] private bool useSnapping;
    [SerializeField] private float snapAngle = 15f;
    [SerializeField] private float snapWhenSlowerThan = 20f;

    [Header("Debug")]
    [SerializeField] private float angularVelocity;

    public float AngularVelocity => angularVelocity;

    private Camera mainCamera;
    private bool isDragging;
    private float previousMouseAngle;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (wheel == null)
            wheel = transform;

        if (dragTarget == null)
            dragTarget = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartDrag();

        if (Input.GetMouseButtonUp(0))
            StopDrag();

        if (isDragging)
            DragWheel();

        ApplyWheelMotion();
        UpdateCursorVisual();
    }

    private void TryStartDrag()
    {
        Vector2 mouseWorld = GetMouseWorldPosition();

        if (dragTarget == null)
            return;

        if (!dragTarget.OverlapPoint(mouseWorld))
            return;

        isDragging = true;
        previousMouseAngle = GetAngleFromWheelCenter(mouseWorld);
    }

    private void StopDrag()
    {
        isDragging = false;
    }

    private void DragWheel()
    {
        Vector2 mouseWorld = GetMouseWorldPosition();

        float currentMouseAngle = GetAngleFromWheelCenter(mouseWorld);
        float mouseAngleDelta = Mathf.DeltaAngle(previousMouseAngle, currentMouseAngle);

        // Positive wheel velocity is clockwise, so invert Unity's normal CCW-positive angle.
        float desiredTorque = -mouseAngleDelta * driveStrength;

        float resistance = windUpResistance;

        // If trying to reverse current momentum, make it struggle more.
        if (angularVelocity != 0f && Mathf.Sign(desiredTorque) != Mathf.Sign(angularVelocity))
            resistance *= 1.75f;

        angularVelocity += desiredTorque / Mathf.Max(0.01f, resistance);
        angularVelocity = Mathf.Clamp(angularVelocity, -maxAngularVelocity, maxAngularVelocity);

        previousMouseAngle = currentMouseAngle;
    }

    private void ApplyWheelMotion()
    {
        float drag = isDragging ? draggingDrag : releaseDrag;

        angularVelocity = Mathf.MoveTowards(
            angularVelocity,
            0f,
            drag * 360f * Time.deltaTime
        );

        wheel.Rotate(0f, 0f, -angularVelocity * Time.deltaTime);

        if (useSnapping && !isDragging && Mathf.Abs(angularVelocity) <= snapWhenSlowerThan)
            SnapWheel();
    }

    private void SnapWheel()
    {
        if (snapAngle <= 0f)
            return;

        float currentZ = wheel.eulerAngles.z;
        float targetZ = Mathf.Round(currentZ / snapAngle) * snapAngle;

        float newZ = Mathf.LerpAngle(currentZ, targetZ, Time.deltaTime * 8f);
        wheel.rotation = Quaternion.Euler(0f, 0f, newZ);
    }

    private void UpdateCursorVisual()
    {
        if (visualCursor == null || pedalVisual == null)
            return;

        if (isDragging)
        {
            visualCursor.gameObject.SetActive(true);
            visualCursor.position = pedalVisual.position;
        }
        else
        {
            visualCursor.gameObject.SetActive(false);
        }
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mouse);
    }

    private float GetAngleFromWheelCenter(Vector2 worldPosition)
    {
        Vector2 direction = worldPosition - (Vector2)wheel.position;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}