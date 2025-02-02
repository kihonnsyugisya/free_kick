using UnityEngine;
using UniRx;
using System;

/// <summary>
/// サッカーボールの物理挙動を管理するクラス。
/// - 地面に触れている間のみ摩擦による減速を適用
/// - 衝突時のイベント通知
/// - ボールが完全に停止したことを通知
/// </summary>
public class SoccerBall : MonoBehaviour
{
    [Tooltip("転がっていると判定する速度のしきい値")]
    [SerializeField] private float rollingThreshold = 0.1f;

    [Tooltip("ボールの摩擦係数（1に近いほど減速が緩やか）")]
    [SerializeField] private float frictionFactor = 0.98f;

    [Tooltip("停止したと判定する速度のしきい値")]
    [SerializeField] private float stopThreshold = 0.05f;

    // 衝突イベント通知（ボールが壁や地面に衝突したときに発火）
    private readonly Subject<Unit> onBallHit = new Subject<Unit>();
    public IObservable<Unit> OnBallHit => onBallHit;

    // ボールの停止状態（ReactivePropertyを使って変更時に通知）
    public BoolReactiveProperty IsStopped { get; private set; } = new BoolReactiveProperty(false);

    // Rigidbodyの参照
    private Rigidbody rigidbody;

    // 地面との接触判定
    private bool isGrounded = false;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // 地面に接しているときのみ摩擦を適用
        if (isGrounded)
        {
            ApplyFriction();
        }
    }

    /// <summary>
    /// ボールの速度に摩擦を適用して、徐々に減速させる（地面に触れている間のみ）
    /// </summary>
    private void ApplyFriction()
    {
        if (rigidbody == null) return;

        // 現在の速度を取得
        Vector3 velocity = rigidbody.linearVelocity;

        // 水平方向の速度のみ考慮（地面との摩擦を再現するため）
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

        // 摩擦を適用して速度を減少させる
        horizontalVelocity *= frictionFactor;

        // 速度が停止判定のしきい値以下になった場合、完全に停止
        bool wasStopped = IsStopped.Value;  // 以前の停止状態を保持
        bool nowStopped = horizontalVelocity.magnitude < stopThreshold;

        if (nowStopped)
        {
            horizontalVelocity = Vector3.zero;
        }

        // 更新後の速度をRigidbodyに適用（Y方向の速度は変更しない）
        rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);

        // 停止状態が変わったら通知
        if (wasStopped != nowStopped)
        {
            IsStopped.Value = nowStopped;
        }
    }

    /// <summary>
    /// 衝突時の処理（ボールが壁や地面に当たったときに通知を発火）
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // 転がっている最中なら通知しない
        if (IsRolling()) return;

        // 衝突イベントを発火
        onBallHit.OnNext(Unit.Default);
    }

    /// <summary>
    /// 地面に接している間は isGrounded を true にする
    /// </summary>
    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    /// <summary>
    /// 地面から離れたら isGrounded を false にする
    /// </summary>
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    /// <summary>
    /// ボールが転がっているかを判定
    /// </summary>
    /// <returns>転がっている場合は true</returns>
    private bool IsRolling()
    {
        if (rigidbody == null) return false;

        // XZ軸の速度（水平移動）を取得
        Vector3 horizontalVelocity = new Vector3(rigidbody.linearVelocity.x, 0, rigidbody.linearVelocity.z);

        // 速度がしきい値を超えているか、Y方向の移動がほぼない場合は転がっていると判定
        return horizontalVelocity.magnitude > rollingThreshold && Mathf.Abs(rigidbody.linearVelocity.y) < 0.1f;
    }
}
