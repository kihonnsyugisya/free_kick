using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HandSwipeAnimation : MonoBehaviour
{
    [Tooltip("手の画像の RectTransform")]
    [SerializeField] private RectTransform handImage;

    [Tooltip("手の画像 (Image)")]
    [SerializeField] private Image handImageComponent;

    [Tooltip("右矢印の画像")]
    [SerializeField] private Image rightArrow;

    [Tooltip("左矢印の画像")]
    [SerializeField] private Image leftArrow;

    [Tooltip("移動にかかる時間")]
    [SerializeField] private float moveDuration = 1.5f;

    [Tooltip("フェードアウトの時間")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Tooltip("スワイプを検知する閾値（画面の%）")]
    [SerializeField] private float swipeThresholdPercentage = 0.15f; // 画面幅の20%

    private Vector2 startTouchPosition;
    private bool isSwiped = false;

    private void Start()
    {
        StartHandAnimation();
    }

    private void StartHandAnimation()
    {
        if (isSwiped) return;

        // 親のRectTransform（Canvasなど）を取得
        RectTransform parentRect = handImage.parent as RectTransform;
        float moveDistance = (parentRect.rect.width / 2) * 0.5f; // 画面幅の25%程度を移動

        // 初期位置
        Vector2 startPos = handImage.anchoredPosition;

        // 矢印の表示をセット
        ShowRightArrow();

        // 左右に移動するアニメーション
        handImage.DOAnchorPosX(startPos.x + moveDistance, moveDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .OnStepComplete(ToggleArrows);
    }

    private void ShowRightArrow()
    {
        rightArrow.gameObject.SetActive(true);
        leftArrow.gameObject.SetActive(false);
    }

    private void ShowLeftArrow()
    {
        rightArrow.gameObject.SetActive(false);
        leftArrow.gameObject.SetActive(true);
    }

    private void ToggleArrows()
    {
        if (rightArrow.gameObject.activeSelf)
        {
            ShowLeftArrow();
        }
        else
        {
            ShowRightArrow();
        }
    }

    private void Update()
    {
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (isSwiped) return;

        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 endTouchPosition = Input.mousePosition;
            float swipeDistance = Mathf.Abs(endTouchPosition.x - startTouchPosition.x);

            float screenWidth = Screen.width;
            float swipeThreshold = screenWidth * swipeThresholdPercentage;

            if (swipeDistance >= swipeThreshold)
            {
                HideTutorial();
            }
        }
    }

    private void HideTutorial()
    {
        if (isSwiped) return;

        isSwiped = true;

        // DOTweenのSequenceを作成
        Sequence fadeSequence = DOTween.Sequence();

        fadeSequence
            .Join(handImageComponent.DOFade(0f, fadeDuration))  // 手の画像フェード
            .Join(rightArrow.DOFade(0f, fadeDuration))          // 右矢印フェード
            .Join(leftArrow.DOFade(0f, fadeDuration))           // 左矢印フェード
            .OnComplete(() =>
            {
                // 全てのフェードが終わったら非表示
                handImage.gameObject.SetActive(false);
                rightArrow.gameObject.SetActive(false);
                leftArrow.gameObject.SetActive(false);
            });
    }

}
