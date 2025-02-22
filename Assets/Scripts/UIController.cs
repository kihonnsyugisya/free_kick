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
using GoogleMobileAds.Sample;

/// <summary>
/// ボールの挙動を制御するUIコントローラー
/// 画面上のボタンやスライダーなどを管理し、ボールの操作を行うクラス
/// ステージ表示用のUIは別クラスで管理し、ボタン操作などの機能に特化させる
/// </summary>
public class UIController : MonoBehaviour
{
    // ---- UI要素 -------------------------
    [Header("UI要素 ---------------------------------------")]
    public Button retryButton;
    public Button kickButton;
    public Button NextButtonAlpa; // リプレイ中に表示される透明なスキップボタン
    public ClearTextEffect clearTextEffect;
    public LifeManager lifeManager;
    public SliderController powerSlider;
    [SerializeField] private Slider yAxisSlider;
    [SerializeField] private CanvasGroup stageTexts;
    [SerializeField] private TextMeshProUGUI stageNum;
    [SerializeField] private CanvasGroup darkScreen; // 画面暗転用CanvasGroup
    [SerializeField] private GameObject irisCanvas;
    [SerializeField] private GameObject controllUIs;
    [SerializeField] private GameObject replayText;
    [SerializeField] private BannerViewController bannerViewController;

    // ---- ゲームコントローラー要素 -------------------------
    [Header("ゲームコントローラー要素 ------------------------")]
    [SerializeField] private GameObject ball;
    [SerializeField] private Transform startPos;
    public FreeKicker freeKicker;
    [SerializeField] private SoccerBall soccerBall;  // サッカーボールの Rigidbody
    public CameraSwitcher cameraSwitcher;

    private void Awake()
    {
        UiInit();
    }

    void Start()
    {
        freeKicker.ball = ball.transform;

        var stageName = SceneManager.GetActiveScene().name;
        ShowStageText(SceneListUtility.GetBaseStageName(stageName));

        yAxisSlider.OnValueChangedAsObservable().Subscribe(value => {
            freeKicker.kickDirection.y = value;
        }).AddTo(this);

        kickButton.onClick.AddListener(() => {
            powerSlider.StopSlider();
            freeKicker.kickForce = powerSlider.powerSlider.value;
            freeKicker.KickBall();
        });

        freeKicker.kickerKnee.OnBallHit.Subscribe(_ => {
            Shoot();
        }).AddTo(this);

        freeKicker.hasKicked.Skip(1).Subscribe(value => {
            if(value) ShowControllUis(!value);
        }).AddTo(this);
    }

    /// <summary>
    /// UnityEditorでプレイした後になぜかUIが非表示になったじょうたいになるから初期化するようにした
    /// </summary>
    private void UiInit()
    {
        darkScreen.alpha = 0f;
        darkScreen.gameObject.SetActive(true);
        //irisCanvas.SetActive(true); アイリスインやろうとしたがあきらめた

        foreach (Transform ui in controllUIs.transform)
        {
            ui.gameObject.SetActive(true);
        }
        powerSlider.Retry();
        retryButton.gameObject.SetActive(false);
    }

    public void Retry()
    {
        ShowControllUis(true);
        ResetBall();
        freeKicker.Retry();
        powerSlider.Retry();
        cameraSwitcher.SwitchCamera(CameraSwitcher.CameraType.Kicker);
        ShowRetryButton(false);
    }

    /// <summary>
    /// ボールを蹴る処理
    /// </summary>
    private void Shoot()
    {
        soccerBall.GetComponent<Rigidbody>().AddForce(freeKicker.kickDirection.normalized * freeKicker.kickForce, ForceMode.Impulse);
        cameraSwitcher.SwitchCamera(CameraSwitcher.CameraType.Ball);
    }

    private void ResetBall()
    {
        Rigidbody rb = soccerBall.GetComponent<Rigidbody>();

        rb.isKinematic = true; // 物理シミュレーションを停止
        rb.linearVelocity = Vector3.zero;
        rb.MovePosition(startPos.position);
        rb.MoveRotation(Quaternion.identity);
        Physics.SyncTransforms(); // 物理エンジンに強制的に反映
        rb.isKinematic = false; // 物理シミュレーションを再開
    }



    public async Task ShowClearText()
    {
        //FadeIn(clearText);
        foreach (Transform ui in controllUIs.transform)
        {
            ui.gameObject.SetActive(false);
        }
        await clearTextEffect.ShowClearText("CLEAR");
    }

    public async Task ShowStageClearText()
    {
        await clearTextEffect.ShowClearTextWithBounce("ステージクリア");
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
        bannerViewController.HideAd();
        // フェードイン（暗転開始）
        FadeIn(darkScreen);
        //irisCanvas.SetActive(false);

        // フェードイン完了後、暗転状態を維持する時間 (0.7秒 + 1.0秒)
        await Task.Delay(1700);

        // フェードアウト（暗転解除）
        //irisCanvas.SetActive(true);
        FadeOut(darkScreen);

        bannerViewController.ShowAd();
    }

    public async Task ShowStageText(GameObject stageText)
    {
        bannerViewController.HideAd();
        // フェードイン（暗転開始）
        FadeIn(darkScreen);

        // フェードイン完了後、暗転状態を維持する時間 (0.7秒 + 1.0秒)
        await Task.Delay(1700);

        // フェードアウト（暗転解除）
        FadeOut(darkScreen);
        bannerViewController.ShowAd();
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

    public void ShowRetryButton(bool isShow)
    { 
        retryButton.gameObject.SetActive(isShow);
    }

    public void PlayReplay()
    { 
        replayText.SetActive(true);
        foreach (Transform ui in controllUIs.transform)
        {
            ui.gameObject.SetActive(false);
        }
        //clearText.gameObject.SetActive(false);
        clearTextEffect.gameObject.SetActive(false);

        ResetBall();
    }
}
