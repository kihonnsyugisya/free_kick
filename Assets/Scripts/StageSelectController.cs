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

            // ロックされているかどうかを判定
            bool isUnlocked = SaveLoadManager.LoadStagefPrefix(prefix);

            // ボタンのインタラクティブ設定
            stageSelectButton.button.interactable = isUnlocked;
            // アンロックされている場合のみクリックイベントを追加
            if (isUnlocked)
            {
                stageSelectButton.button.onClick.AddListener(() => {
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
