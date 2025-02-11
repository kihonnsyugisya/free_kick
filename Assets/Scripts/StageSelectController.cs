using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectController : MonoBehaviour
{
    [SerializeField] private StageSelectButton stageSelectButtonPrefab; // ボタンのプレハブ
    [SerializeField] private Transform buttonParent; // ボタンを配置する親Transform
    [SerializeField] private Color lockedColor = Color.gray; // 未クリアのステージの色

    private void Start()
    {
        List<string> stageSceneNames = SceneListUtility.GetStageSceneNamesInBuildSettings();
        string lastClearedStage = SaveLoadManager.LoadLastStage();
        int lastClearedStageNumber = GetStageNumber(lastClearedStage);

        foreach (string sceneName in stageSceneNames)
        {
            StageSelectButton stageSelectButton = Instantiate(stageSelectButtonPrefab, buttonParent);
            stageSelectButton.buttonLabel.text = sceneName;

            // ステージ番号を取得し、クリア済みステージと比較
            int stageNumber = GetStageNumber(sceneName);
            bool isUnlocked = stageNumber <= lastClearedStageNumber;

            stageSelectButton.button.interactable = isUnlocked;

            if (isUnlocked)
            {
                stageSelectButton.button.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
            }
        }
    }

    /// <summary>
    /// "Stage3" → 3 のように数値部分を取得する
    /// 数値がない場合は 0 を返す
    /// </summary>
    private int GetStageNumber(string stageName)
    {
        if (string.IsNullOrEmpty(stageName)) return 0;

        string numberPart = System.Text.RegularExpressions.Regex.Match(stageName, @"\d+").Value;
        return int.TryParse(numberPart, out int number) ? number : 0;
    }
}
