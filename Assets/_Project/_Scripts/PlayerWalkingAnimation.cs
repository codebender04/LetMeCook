using DG.Tweening;
using UnityEngine;

public class PlayerWalkingAnimation : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [SerializeField] private float bobHeight = 0.08f;
    [SerializeField] private float bobDuration = 0.15f;
    [SerializeField] private float tiltAngle = 5f;

    private Vector3 initialPosition;
    private Tween bobTween;
    private Tween tiltTween;
    private Vector3 lastPosition;

    private void Awake()
    {
        initialPosition = visual.localPosition;
    }

    private void Update()
    {
        bool walking = (transform.position - lastPosition).sqrMagnitude > 0.0001f;

        SetWalking(walking);

        lastPosition = transform.position;
    }
    private void SetWalking(bool walking)
    {
        if (walking)
        {
            if (bobTween == null || !bobTween.IsActive())
            {
                bobTween = visual.DOLocalMoveY(
                    initialPosition.y + bobHeight,
                    bobDuration
                )
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
            }

            if (tiltTween == null || !tiltTween.IsActive())
            {
                tiltTween = visual.DOLocalRotate(
                    new Vector3(0f, 0f, tiltAngle),
                    bobDuration
                )
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
            }
        }
        else
        {
            bobTween?.Kill();
            tiltTween?.Kill();

            visual.localPosition = initialPosition;
            visual.localRotation = Quaternion.identity;
        }
    }

    private void OnDestroy()
    {
        bobTween?.Kill();
        tiltTween?.Kill();
    }
}