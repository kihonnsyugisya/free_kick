using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ステージ選択画面で、ステージボタンを動的に生成するクラス
/// </summary>
public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private Transform buttonParent; // ボタンの親オブジェクト
    [SerializeField] private StageSelectButton stageSelectButtonPrefab; // ボタンのプレハブ
    [SerializeField] private Color activeStageColor = Color.white;
    [SerializeField] private Color inActiveStageColor = Color.white;

    private Dictionary<StagePrefix, string> stageDisplayMapping;

    public static string currentStageName = null;

    private void Awake()
    {
        InitializeStageDisplayMapping();
        CreateStageButtons();
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
        foreach (StagePrefix prefix in stageDisplayMapping.Keys)
        {
            // ボタンを生成
            StageSelectButton stageSelectButton = Instantiate(stageSelectButtonPrefab, buttonParent);
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
            // カラーブロックを取得
            ColorBlock colors = stageSelectButton.button.colors;

            // 無効時の色を変更
            colors.disabledColor = inActiveStageColor;

            // 変更を適用
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
