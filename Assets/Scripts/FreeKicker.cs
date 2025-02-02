using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeKicker : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerStartPos;
    [SerializeField] private Transform ballStartPos; // ボールの初期位置を指定するオブジェクト

    [HideInInspector] public float kickForce;
    [HideInInspector] public Vector3 kickDirection;

    public CollisionReciver kickerKnee;
    [HideInInspector] public Transform ball;

    [Tooltip("スワイプによる回転速度")]
    public float rotationSpeed = 5f;

    private float currentAngle = -90f; // デフォルトで -90° に設定
    private Vector2 startTouchPosition;
    private bool isSwiping = false;

    private static readonly int KickStateHash = Animator.StringToHash("FreeKick");

    [HideInInspector] public BoolReactiveProperty hasKicked = new();

    void Start()
    {
        // 初期位置 & 回転を設定
        transform.position = playerStartPos.position;
        transform.rotation = Quaternion.Euler(0, currentAngle, 0);

        UpdateBallPosition(); // ボールの位置を更新
    }

    void Update()
    {
        if (!hasKicked.Value)
        {
            HandleSwipe();
        }
    }

    public void KickBall()
    {
        animator.Play(KickStateHash);
        hasKicked.Value = true;
        CalculateKickDirection();
    }

    public void Retry()
    {
        // 位置のみリセットし、回転はリセットしない
        transform.position = playerStartPos.position;

        animator.applyRootMotion = false;
        animator.applyRootMotion = true;

        hasKicked.Value = false;

        UpdateBallPosition(); // ボールの位置を再設定
    }

    private void HandleSwipe()
    {
        if (Input.touchCount == 0) return;
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startTouchPosition = touch.position;
                isSwiping = true;
                break;
            case TouchPhase.Moved:
                if (isSwiping)
                {
                    float deltaX = touch.position.x - startTouchPosition.x;
                    currentAngle += deltaX * rotationSpeed * Time.deltaTime;
                    currentAngle = Mathf.Clamp(currentAngle, -180f, 0f); // 右手から左手の範囲に制限
                    transform.rotation = Quaternion.Euler(0, currentAngle, 0);
                    startTouchPosition = touch.position;
                    UpdateBallPosition();
                }
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isSwiping = false;
                break;
        }
    }

    private void UpdateBallPosition()
    {
        if (ball != null && ballStartPos != null)
        {
            // ボールを `ballStartPos` のワールド座標に配置
            ball.position = ballStartPos.position;
        }
    }

    private void CalculateKickDirection()
    {
        Vector3 direction = transform.forward;
        direction.y = kickDirection.y;
        kickDirection = direction;
    }
}
