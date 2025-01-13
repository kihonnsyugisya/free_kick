using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ButtonController : MonoBehaviour
{
    private const int ButtonThrottleTimeMs = 2000; // ボタンの連続クリック防止時間 (ms)
    [SerializeField] private Button reviewButton; // レビュー機能ボタン
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ConfigureReviewButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// レビュー機能ボタンのクリックイベントを設定。
    /// </summary>
    private void ConfigureReviewButton()
    {
        reviewButton.OnClickAsObservable()
            .ThrottleFirst(TimeSpan.FromMilliseconds(ButtonThrottleTimeMs))
            .TakeUntilDestroy(this)
            .Subscribe(_ => StartCoroutine(InAppReviewManager.RequestReview()));
    }
}
