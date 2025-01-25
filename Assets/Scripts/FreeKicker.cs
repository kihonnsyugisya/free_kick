using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeKicker : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerStartPos;

    // ---- キックパラメータ関連 -------------------------
    [HideInInspector] public float kickForce;    // キックの強さ
    [HideInInspector] public Vector3 kickDirection;

    // 通知専用のSubject
    public CollisionReciver kickerKnee;

    [HideInInspector] public Transform ball; // ボール（中心）のTransform
    [Header("Orbit Settings")]
    [Tooltip("プレイヤーとボールの距離（円の半径）")]
    public float orbitRadius = 2f; // 円軌道の半径
    [Tooltip("スワイプによる回転速度")]
    public float rotationSpeed = 5f; // 回転速度

    private float currentAngle = 0f; // 現在の回転角度
    private Vector2 startTouchPosition; // スワイプの開始位置
    private Vector2 currentTouchPosition; // 現在のタッチ位置
    private bool isSwiping = false;

    private static readonly int KickStateHash = Animator.StringToHash("FreeKick"); // アニメーション状態のハッシュ

    [HideInInspector] public BoolReactiveProperty hasKicked = new(); // キックしたかどうかを判定するフラグ

    void Start()
    {

    }

    void Update()
    {
        // キック後はUpdatePlayerPositionを呼び出さない
        if (!hasKicked.Value)
        {
            HandleSwipe();
            UpdatePlayerPosition();
        }
    }

    // キックイベントで呼び出すメソッド
    public void KickBall()
    {
        animator.Play(KickStateHash); // キックアニメーションを再生
        hasKicked.Value = true; // キックしたフラグを設定                          
        CalculateKickDirection();// ボールが蹴られる方向を計算
    }

    public void Retry()
    {
        transform.position = playerStartPos.position;
        animator.applyRootMotion = false;
        transform.rotation = Quaternion.identity;
        animator.applyRootMotion = true;
        hasKicked.Value = false; // キックしたフラグをリセット
    }

    private void HandleSwipe()
    {
        // タッチが存在しているかどうかをチェック
        if (Input.touchCount == 0)
            return;

        // UIがタッチを受け取っている場合、スワイプ処理を無視
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            // nGUI上をクリックしているので処理をキャンセルする。
            return;
        }

        Touch touch = Input.GetTouch(0);  // 最初のタッチを取得

        switch (touch.phase)
        {
            case TouchPhase.Began:
                // スワイプの開始位置を記録
                startTouchPosition = touch.position;
                isSwiping = true;
                break;

            case TouchPhase.Moved:
                if (isSwiping)
                {
                    // スワイプ中の現在の位置
                    currentTouchPosition = touch.position;

                    // 水平方向の移動量を計算
                    float deltaX = currentTouchPosition.x - startTouchPosition.x;

                    // 角度を更新
                    currentAngle += deltaX * rotationSpeed * Time.deltaTime;

                    // 現在の位置を新たな開始位置として記録
                    startTouchPosition = currentTouchPosition;
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                // スワイプ終了
                isSwiping = false;
                break;
        }
    }


    private void UpdatePlayerPosition()
    {
        // プレイヤーの新しい位置を計算（円軌道）
        float radianAngle = currentAngle * Mathf.Deg2Rad; // 角度をラジアンに変換
        Vector3 offset = new Vector3(Mathf.Cos(radianAngle), 0, Mathf.Sin(radianAngle)) * orbitRadius;

        // プレイヤーの位置を更新（y値を固定）
        Vector3 newPosition = ball.position + offset;
        newPosition.y = transform.position.y; // プレイヤーの高さは変更しない
        transform.position = newPosition;

        // プレイヤーをボールの位置に向ける（y軸だけ）
        Vector3 targetPosition = new Vector3(ball.position.x, transform.position.y, ball.position.z);
        transform.LookAt(targetPosition);

        // X軸の回転をリセット
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = 0;
        transform.rotation = Quaternion.Euler(rotation);
    }

    // プレイヤーがボールを蹴った方向を計算
    private void CalculateKickDirection()
    {
        // プレイヤーの位置からボールの位置を引いて、蹴られた方向を取得
        Vector3 direction = (ball.position - transform.position).normalized;
        direction.y = kickDirection.y;

        kickDirection = direction; // 計算した方向をキック方向に保存
    }

}
