using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Unity.VisualScripting;

/// <summary>
/// ボールの参照はこのクラス一個でコントロールする
/// よって、ボールの再配置（リトライ）やボールに力を与えるのもこのクラス
/// 理想はUIは別クラスに分けて、ボタンが押された通知をこのクラスで購読する形にしたい
/// </summary>
public class UIController : MonoBehaviour
{
    // ---- UI関連 -------------------------
    [SerializeField] private Button retryButton;
    [SerializeField] private Button kickButton;

    // ---- ゲームコントローラ関連 -------------------------
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform startPos;
    [SerializeField] private FreeKicker freeKicker;
    [SerializeField] private Rigidbody ballRigidbody;  // サッカーボールの Rigidbody


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        retryButton.onClick.AddListener(() => {
            Retry();
        });

        kickButton.onClick.AddListener(() => {
            freeKicker.KickBall(); 
        });

        freeKicker.OnBallHit.Subscribe(_ => {
            Shoot();
        }).AddTo(this);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Retry()
    {
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.linearVelocity = Vector3.zero;
        ball.transform.position = startPos.position;
        freeKicker.Retry();
    }


    /// <summary>
    /// ボールに力を加えて飛ばす
    /// 力加減はキッカーを参照する
    /// </summary>
    private void Shoot()
    {
        // AddForceでボールを飛ばす
        ballRigidbody.AddForce(freeKicker.kickDirection.normalized * freeKicker.kickForce, ForceMode.Impulse);
    }






}
