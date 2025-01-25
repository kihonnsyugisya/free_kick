using UnityEngine;
using UniRx;
using System;

public class SoccerBall : MonoBehaviour
{
    // 転がり判定の速度しきい値
    [SerializeField] private float rollingThreshold = 0.1f;

    // 通知の間隔 (秒)
    [SerializeField] private float notificationCooldown = 0.5f;

    // 衝突イベント通知
    private readonly Subject<Unit> onBallHit = new Subject<Unit>();
    public IObservable<Unit> OnBallHit => onBallHit;

    // Rigidbody参照
    public Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 転がっている場合は通知を抑制
        if (IsRolling())
        {
            return;
        }

        // 衝突通知を発行
        onBallHit.OnNext(Unit.Default);
    }

    /// <summary>
    /// 転がっているかを判定
    /// </summary>
    /// <returns>転がっている場合はtrue</returns>
    private bool IsRolling()
    {
        if (rigidbody == null) return false;

        // XZ軸の速度がしきい値を超えているかどうかで転がりを判定
        Vector3 horizontalVelocity = new Vector3(rigidbody.linearVelocity.x, 0, rigidbody.linearVelocity.z);
        return horizontalVelocity.magnitude > rollingThreshold && Mathf.Abs(rigidbody.linearVelocity.y) < 0.1f;
    }
}
