using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SliderController : MonoBehaviour
{
    public Slider powerSlider;  // スライダー
    [SerializeField] private Slider arrowSlider;  // 前回の位置を示すスライダー
    [SerializeField] private Button stopButton;   // ストップボタン

    [SerializeField] private float minValue = 0f;  // スライダーの最小値
    [SerializeField] private float maxValue = 50f; // スライダーの最大値
    [SerializeField] private float speed = 5f;     // スライダーの動きの速さ

    [HideInInspector] public bool isMoving = true;

    void Start()
    {
        // スライダーの初期設定
        powerSlider.minValue = minValue;
        powerSlider.maxValue = maxValue;
        arrowSlider.minValue = minValue;
        arrowSlider.maxValue = maxValue;
        arrowSlider.gameObject.SetActive(false);

        // スライダーの往復動作（Rxを使用してリアクティブに動かす）
        Observable.EveryUpdate()
            .Where(_ => isMoving)
            .Subscribe(_ => MoveSlider())
            .AddTo(this);
    }

    // スライダーを左右に往復させる
    private void MoveSlider()
    {
        // スライダーの値を現在位置に基づいて変更
        powerSlider.value = Mathf.PingPong(Time.time * speed, maxValue - minValue) + minValue;
    }

    // スライダーをストップし、現在の値を表示
    public void StopSlider()
    {
        isMoving = false; // スライダーを停止
        arrowSlider.gameObject.SetActive(true);
        arrowSlider.value = powerSlider.value;
        Debug.Log("威力: " + powerSlider.value); // 現在のスライダーの値を表示
    }

    public void Retry()
    {
        isMoving = true;
    }

    public void NextStage()
    {
        arrowSlider.value = 0f;
        arrowSlider.gameObject.SetActive(false);
        isMoving = true;
    }

    public void hideArrowSlider()
    { 
        arrowSlider.gameObject.SetActive(false);
    }
}
