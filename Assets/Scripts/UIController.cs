using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Unity.VisualScripting;
using DG.Tweening;
using System;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening.Core.Easing;
using UnityEngine.SceneManagement;

/// <summary>
/// ボールの挙動を制御するUIコントローラー
/// 画面上のボタンやスライダーなどを管理し、ボールの操作を行うクラス
/// ステージ表示用のUIは別クラスで管理し、ボタン操作などの機能に特化させる
/// </summary>
public class UIController : MonoBehaviour
{
    // ---- UI要素 -------------------------
    [Header("UI要素 ---------------------------------------")]
    [SerializeField] private Button retryButton;
    public Button kickButton;
    [SerializeField] private SliderController powerSlider;
    [SerializeField] private Slider yAxisSlider;
    [SerializeField] private CanvasGroup stageTexts;
    [SerializeField] private TextMeshProUGUI stageNum;
    [SerializeField] private CanvasGroup clearText;
    [SerializeField] private CanvasGroup darkScreen; // 画面暗転用CanvasGroup
    [SerializeField] private GameObject controllUIs;

    // ---- ゲームコントローラー要素 -------------------------
    [Header("ゲームコントローラー要素 ------------------------")]
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform startPos;
    public FreeKicker freeKicker;
    [SerializeField] private SoccerBall soccerBall;  // サッカーボールの Rigidbody
    [SerializeField] private CameraSwitcher cameraSwitcher;

    void Start()
    {
        freeKicker.ball = ball.transform;
        darkScreen.alpha = 0f; // 初期状態で透明に設定

        ShowStageText(SceneManager.GetActiveScene().name);

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

    public void Retry()
    {
        clearText.alpha = 0f;
        ResetBall();
        freeKicker.Retry();
        powerSlider.Retry();
        cameraSwitcher.SwitchToKickerCamera();
    }

    /// <summary>
    /// ボールを蹴る処理
    /// </summary>
    private void Shoot()
    {
        soccerBall.GetComponent<Rigidbody>().AddForce(freeKicker.kickDirection.normalized * freeKicker.kickForce, ForceMode.Impulse);
        cameraSwitcher.SwitchToBallCamera();
    }

    private void ResetBall()
    {
        soccerBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        soccerBall.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        ball.transform.position = startPos.position;
    }

    public void ShowClearText()
    {
        FadeIn(clearText);
        foreach (Transform ui in controllUIs.transform)
        {
            ui.gameObject.SetActive(false);
        }
    }

    private void ShowStageText(string stageName)
    {
        stageNum.text = stageName;
        FadeIn(stageTexts);
        FadeOut(stageTexts);
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

        // フェードイン完了後、暗転状態を維持する時間 (0.7秒 + 1.0秒)
        await Task.Delay(1700);

        // フェードアウト（暗転解除）
        FadeOut(darkScreen);
    }

    public async Task ShowStageText(GameObject stageText)
    {
        // フェードイン（暗転開始）
        FadeIn(darkScreen);

        // フェードイン完了後、暗転状態を維持する時間 (0.7秒 + 1.0秒)
        await Task.Delay(1700);

        // フェードアウト（暗転解除）
        FadeOut(darkScreen);
    }

    // フェードアウト処理
    private void FadeOut(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0, 1f);
    }

    // フェードイン処理
    private void FadeIn(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1, 1f);
    }
}
