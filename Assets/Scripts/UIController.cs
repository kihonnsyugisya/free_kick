using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Unity.VisualScripting;
using DG.Tweening;
using System;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// ボールの参照はこのクラス一個でコントロールする
/// よって、ボールの再配置（リトライ）やボールに力を与えるのもこのクラス
/// 理想はUIは別クラスに分けて、ボタンが押された通知をこのクラスで購読する形にしたい
/// </summary>
public class UIController : MonoBehaviour
{
    // ---- UI関連 -------------------------
    [Header("UI関連 ---------------------------------------")]
    [SerializeField] private Button retryButton;
    public Button kickButton;
    [SerializeField] private SliderController powerSlider;   
    [SerializeField] private Slider yAxisSlider;
    [SerializeField] private CanvasGroup stageTexts;
    [SerializeField] private TextMeshProUGUI stageNum;
    [SerializeField] private CanvasGroup clearText;
    [SerializeField] private CanvasGroup darkScreen; // 暗転用のCanvasGroup
    [SerializeField] private GameObject controllUIs;

    // ---- ゲームコントローラ関連 -------------------------
    [Header("ゲームコントローラ関連 ------------------------")]
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform startPos;
    public FreeKicker freeKicker;
    [SerializeField] private SoccerBall soccerBall;  // サッカーボールの Rigidbody
    [SerializeField] private CameraSwitcher cameraSwitcher;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        freeKicker.ball = ball.transform;

        darkScreen.alpha = 0f; // 初期状態を完全に非表示に設定

        yAxisSlider.OnValueChangedAsObservable().Subscribe(value => {
            freeKicker.kickDirection.y = value;
        }).AddTo(this);

        retryButton.onClick.AddListener(() => {
            Retry();
        });

        kickButton.onClick.AddListener(() => {
            powerSlider.StopSlider();
            freeKicker.kickForce = powerSlider.powerSlider.value;
            freeKicker.KickBall(); 
        });

        freeKicker.kickerKnee.OnBallHit.Subscribe(_ => {
            Shoot();
        }).AddTo(this);

        freeKicker.hasKicked.Skip(1).Subscribe(value => { 
            ShowControllUis(!value);
        }).AddTo(this);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Retry()
    {
        clearText.alpha = 0f;
        ResetBall();
        freeKicker.Retry();
        powerSlider.Retry();
        cameraSwitcher.SwitchToKickerCamera();
    }


    /// <summary>
    /// ボールに力を加えて飛ばす
    /// 力加減はキッカーを参照する
    /// </summary>
    private void Shoot()
    {
        // AddForceでボールを飛ばす
        soccerBall.rigidbody.AddForce(freeKicker.kickDirection.normalized * freeKicker.kickForce, ForceMode.Impulse);
        cameraSwitcher.SwitchToBallCamera();
    }

    private void ResetBall()
    {
        soccerBall.rigidbody.angularVelocity = Vector3.zero;
        soccerBall.rigidbody.linearVelocity = Vector3.zero;
        ball.transform.position = startPos.position;
    }

    // "UI関連 ---------------------------------------"

    public void ShowClearText()
    {
        FadeIn(clearText);
        foreach (Transform ui in controllUIs.transform)
        {
            ui.gameObject.SetActive(false);
        }
    }

    public void ShowControllUis(bool isShow)
    {
        foreach (Transform ui in controllUIs.transform)
        {
            ui.gameObject.SetActive(isShow);
        }
    }

    public async Task FadeToBlackForOneSecond()
    {
        // フェードイン（暗転開始）
        FadeIn(darkScreen);

        // フェードイン時間 + 暗転時間を待機 (0.7秒 + 1.0秒)
        await Task.Delay(1700);

        // フェードアウト（暗転終了）
        FadeOut(darkScreen);
    }

    // FadeOutメソッド
    private void FadeOut(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1f; // 初期状態を完全に表示するように設定
        canvasGroup.DOFade(0, 1f); // 1秒間でフェードアウト
    }

    // FadeInメソッド
    private void FadeIn(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f; // 初期状態を完全に非表示に設定
        canvasGroup.DOFade(1, 1f); // 1秒間でフェードイン
    }


}
