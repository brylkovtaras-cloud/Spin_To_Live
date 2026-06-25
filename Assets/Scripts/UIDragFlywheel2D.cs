using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragFlywheel2D : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform wheel;
    [SerializeField] private RectTransform visualCursor;
    [SerializeField] private RectTransform pedalVisual;

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

    private bool isDragging;
    private float previousPointerAngle;

    private void Awake()
    {
        if (wheel == null)
            wheel = (RectTransform)transform;
    }

    private void Update()
    {
        ApplyWheelMotion();
        UpdateCursorVisual();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        previousPointerAngle = GetAngleFromWheelCenter(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        float currentPointerAngle = GetAngleFromWheelCenter(eventData);
        float pointerAngleDelta = Mathf.DeltaAngle(previousPointerAngle, currentPointerAngle);

        // Positive velocity means clockwise.
        float desiredTorque = -pointerAngleDelta * driveStrength;

        float resistance = windUpResistance;

        if (angularVelocity != 0f && Mathf.Sign(desiredTorque) != Mathf.Sign(angularVelocity))
            resistance *= 1.75f;

        angularVelocity += desiredTorque / Mathf.Max(0.01f, resistance);
        angularVelocity = Mathf.Clamp(angularVelocity, -maxAngularVelocity, maxAngularVelocity);

        previousPointerAngle = currentPointerAngle;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
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

    private float GetAngleFromWheelCenter(PointerEventData eventData)
    {
        Vector2 localPointerPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            wheel,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition
        );

        return Mathf.Atan2(localPointerPosition.y, localPointerPosition.x) * Mathf.Rad2Deg;
    }

    private void UpdateCursorVisual()
    {
        if (visualCursor == null || pedalVisual == null)
            return;

        visualCursor.gameObject.SetActive(isDragging);

        if (isDragging)
            visualCursor.position = pedalVisual.position;
    }
}