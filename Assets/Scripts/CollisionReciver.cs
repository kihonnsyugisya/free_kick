using System;
using UniRx;
using UnityEngine;

public class CollisionReciver : MonoBehaviour
{
    private Subject<Unit> onBallHit = new Subject<Unit>();
    public IObservable<Unit> OnBallHit => onBallHit;

    private void OnTriggerEnter(Collider other)
    {
        // ボールに触れた場合
        if (other.CompareTag("Ball"))
        {
            onBallHit.OnNext(Unit.Default); // 通知を発行
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ボールに触れた場合
        if (collision.gameObject.CompareTag("Ball"))
        {
            onBallHit.OnNext(Unit.Default); // 通知を発行
        }
    }
}
