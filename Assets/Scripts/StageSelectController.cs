using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections; // DOTween を使用

/// <summary>
/// ステージ選択画面で、ステージボタンを動的に生成するクラス
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private VerticalLayoutGroup buttonParent; // ボタンの親オブジェクト
    [SerializeField] private StageSelectButton stageSelectButtonPrefab; // ボタンのプレハブ
    [SerializeField] private Color activeStageColor = Color.white;
    [SerializeField] private Color inActiveStageColor = Color.white;
    [SerializeField] private Color statusNewColor = Color.white;
    [SerializeField] private float slideInDuration = 0.5f; // スライドの時間
    [SerializeField] private float slideInDelay = 0.1f; // 各ボタンの遅延時間
    [SerializeField] private float slideStartOffset = -500f; // 初期位置オフセット（左にずらす）

    private CanvasGroup canvasGroup;
    private Dictionary<StagePrefix, string> stageDisplayMapping;

    public static string currentStageName = null;

    private void Start()
    {
        canvasGroup = buttonParent.GetComponent<CanvasGroup>();
        InitializeStageDisplayMapping();
        CreateStageButtons();
        // 🔽🔽🔽 ここから Tween アニメーション追加 🔽🔽🔽
        StartCoroutine(DelayedSlideIn());
    }

    /// <summary>
    /// ステージのプレフィックスごとのボタン表示名を設定
    /// </summary>
    private void InitializeStageDisplayMapping()
    {
        stageDisplayMapping = new Dictionary<StagePrefix, string>
        {
            { StagePrefix.Tu, "チュートリアル" },
            { StagePrefix.A, "Stage1" },
            { StagePrefix.B, "Stage2" },
            { StagePrefix.C, "Stage3" },
            { StagePrefix.D, "Stage4" }
        };
    }

    /// <summary>
    /// ステージ選択ボタンを動的に生成し、シーン遷移の処理を設定
    /// </summary>
    private void CreateStageButtons()
    {
        canvasGroup.alpha = 0;
        foreach (StagePrefix prefix in stageDisplayMapping.Keys)
        {
            // ボタンを生成
            StageSelectButton stageSelectButton = Instantiate(stageSelectButtonPrefab, buttonParent.transform);
            stageSelectButton.buttonLabel.text = stageDisplayMapping[prefix]; // ボタンの表示名をセット

            string sceneName = "Stage1" + prefix.ToString();

            int isUnlocked = SaveLoadManager.LoadStagePrefix(prefix);

            // ステージセレクトボタンの表示ステータス設定
            stageSelectButton.statusText.text = isUnlocked switch
            {
                (int)SaveStatus.NEW => "New",
                (int)SaveStatus.CLEAR => "Clear",
                (int)SaveStatus.NON_CLEAR => "",
                (int)SaveStatus.LOCK => "",
                _ => stageSelectButton.statusText.text // デフォルトの値
            };

            // ステータスがLOCKの場合、ボタンをインタラクティブにしない
            stageSelectButton.button.interactable = isUnlocked != (int)SaveStatus.LOCK;

            // カラーブロックを取得して無効時の色を変更
            ColorBlock colors = stageSelectButton.button.colors;
            colors.disabledColor = inActiveStageColor;
            stageSelectButton.button.colors = colors;

            // アンロックされている場合のみクリックイベントを追加
            if (isUnlocked != (int)SaveStatus.LOCK)
            {
                stageSelectButton.button.image.color = activeStageColor;
                stageSelectButton.button.onClick.AddListener(() => {
                    SaveLoadManager.SaveStagePrefix(prefix, SaveStatus.NON_CLEAR);
                    currentStageName = stageDisplayMapping[prefix];
                    SceneManager.LoadScene(sceneName);
                });
            }

            if (isUnlocked == (int)SaveStatus.NEW)
            {
                stageSelectButton.statusText.color = statusNewColor;
                stageSelectButton.StartBlinkingStatusText();
            }
        }
    }

    private IEnumerator DelayedSlideIn()
    {
        yield return new WaitForEndOfFrame(); // フレームの最後まで待つ
        SlideInSelectButtons();
    }

    private void SlideInSelectButtons()
    {
        int index = 0; // 階段状の遅延をつけるためのカウンター

        buttonParent.enabled = false;

        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonParent.GetComponent<RectTransform>());

        canvasGroup.alpha = 1.0f;

        foreach (Transform button in buttonParent.transform)
        {
            RectTransform rectTransform = button.GetComponent<RectTransform>();

            //// 初期位置を左側にオフセット
            Vector2 originalPosition = rectTransform.anchoredPosition;
            float buttonWidth = rectTransform.rect.width; // ボタンの幅を取得
                                                          // 初期位置を **右側** にオフセット
            rectTransform.anchoredPosition = new Vector2(originalPosition.x + buttonWidth + 300, originalPosition.y);

            // スライドインアニメーション
            rectTransform.DOAnchorPos(originalPosition, slideInDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(index * slideInDelay);

            index++; // 遅延用カウンターを増やす
        }
    }



}


/// <summary>
/// ステージのプレフィックスを管理するEnum
/// </summary>
public enum StagePrefix
{
    Tu = 0,
    A = 1,
    B = 2,
    C = 3,
    D = 4
}