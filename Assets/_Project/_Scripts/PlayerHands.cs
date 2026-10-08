using UnityEngine;

/// <summary>
/// Procedural floating hands. Each hand eases toward a target picked by the
/// current pose. Works in the flipped Visual's local space, so it mirrors with
/// the player for free. Runs on every client; no input or ownership needed.
/// </summary>
public class PlayerHands : MonoBehaviour
{
    public enum Pose { Idle, Walking, Holding }

    [Tooltip("Player root. Its movement decides whether we are walking.")]
    [SerializeField] private Transform body;
    [SerializeField] private PlayerInteraction interaction;
    [SerializeField] private Transform backHand;
    [SerializeField] private Transform frontHand;
    [Tooltip("Must share a parent space with the hands (both under Visual).")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float smoothTime = 0.05f;

    [Header("Idle")]
    [SerializeField] private float idleBobAmount = 0.015f;
    [SerializeField] private float idleBobSpeed = 2.5f;

    [Header("Walking")]
    [SerializeField] private float swayAmount = 0.07f;
    [SerializeField] private float swayLift = 0.02f;
    [SerializeField] private float swaySpeed = 14f;
    [Tooltip("Keeps the walking pose briefly after stopping, so the hands don't flicker.")]
    [SerializeField] private float walkGraceTime = 0.08f;

    [Header("Holding")]
    [Tooltip("Hands sit this far either side of the hold point.")]
    [SerializeField] private Vector2 gripOffset = new(0.11f, 0f);
    [SerializeField] private float holdBobAmount = 0.02f;

    public Pose CurrentPose { get; private set; }

    private Vector3 backRest;
    private Vector3 frontRest;
    private Vector3 backVelocity;
    private Vector3 frontVelocity;
    private Vector3 lastBodyPosition;
    private float lastMoveTime = float.NegativeInfinity;

    private void Awake()
    {
        backRest = backHand.localPosition;
        frontRest = frontHand.localPosition;
        lastBodyPosition = body.position;
    }

    private void Update()
    {
        // Position delta works for remote players too (their transform is synced).
        if ((body.position - lastBodyPosition).sqrMagnitude > 0.000001f)
            lastMoveTime = Time.time;
        lastBodyPosition = body.position;

        bool walking = Time.time - lastMoveTime < walkGraceTime;
        CurrentPose = interaction.IsHolding ? Pose.Holding : walking ? Pose.Walking : Pose.Idle;

        GetTargets(CurrentPose, walking, out Vector3 backTarget, out Vector3 frontTarget);

        backHand.localPosition = Vector3.SmoothDamp(backHand.localPosition, backTarget, ref backVelocity, smoothTime);
        frontHand.localPosition = Vector3.SmoothDamp(frontHand.localPosition, frontTarget, ref frontVelocity, smoothTime);
    }

    // Add new poses (Chop, Aim, ...) here.
    private void GetTargets(Pose pose, bool walking, out Vector3 back, out Vector3 front)
    {
        float t = Time.time;

        switch (pose)
        {
            case Pose.Walking:
            {
                // Hands swing forward/back in opposite phase.
                float s = Mathf.Sin(t * swaySpeed);
                float lift = Mathf.Abs(s) * swayLift;
                back = backRest + new Vector3(s * swayAmount, lift, 0f);
                front = frontRest + new Vector3(-s * swayAmount, lift, 0f);
                break;
            }
            case Pose.Holding:
            {
                Vector3 grip = holdPoint.localPosition;
                float bob = walking ? Mathf.Abs(Mathf.Sin(t * swaySpeed)) * holdBobAmount : 0f;
                Vector3 offset = new(gripOffset.x, gripOffset.y, 0f);
                back = grip - offset + Vector3.up * bob;
                front = grip + offset + Vector3.up * bob;
                break;
            }
            default:
            {
                float bob = Mathf.Sin(t * idleBobSpeed) * idleBobAmount;
                back = backRest + Vector3.up * bob;
                front = frontRest + Vector3.up * bob;
                break;
            }
        }
    }
}
