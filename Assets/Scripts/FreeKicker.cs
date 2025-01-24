using System;
using UniRx;
using UnityEngine;

public class FreeKicker : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerStartPos;

    // ---- キックパラメータ関連 -------------------------
    public float kickForce = 10f;    // キックの強さ
    public Vector3 kickDirection;


    // 通知専用のSubject
    private Subject<Unit> onBallHit = new Subject<Unit>();
    public IObservable<Unit> OnBallHit => onBallHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    // キックイベントで呼び出すメソッド
    public void KickBall()
    {
        animator.Play("FreeKick");
    }

    public void Retry()
    {
        transform.position = playerStartPos.position;
        animator.applyRootMotion = false;
        transform.rotation = Quaternion.identity;
        animator.applyRootMotion = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        // ボールに触れた場合
        if (other.CompareTag("Ball"))
        {
            onBallHit.OnNext(Unit.Default); // 通知を発行
            Debug.Log("FreeKicker「ボールけったぜ」");
        }
    }


}
