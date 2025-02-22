using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// ビルド設定に登録されているシーンの中から、
/// 「Stage + 数字（+ 英語）」のシーン名を取得して、数字順に並べるユーティリティクラス
/// </summary>
public static class SceneListUtility
{
    private static readonly string prefix = "Stage";

    /// <summary>
    /// ビルド設定に登録されたシーンのうち、
    /// 名前が "Stage" + 数字（+ 任意の文字列）の形式になっているものを昇順に並べてリストとして返す
    /// </summary>
    /// <returns>ステージシーンのリスト（例: Stage1, Stage2, Stage10, Stage3Hard, Stage4Hard）</returns>
    public static List<string> GetStageSceneNamesInBuildSettings()
    {
        List<string> stageScenes = new List<string>();
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            if (sceneName.StartsWith(prefix))
            {
                int stageNumber = GetStageNumber(sceneName);
                if (stageNumber > 0)
                {
                    stageScenes.Add(sceneName);
                }
            }
        }

        // 数字部分で昇順にソート
        stageScenes.Sort((a, b) =>
        {
            int numA = GetStageNumber(a);
            int numB = GetStageNumber(b);
            return numA.CompareTo(numB);
        });

        return stageScenes;
    }

    /// <summary>
    /// "Stage3" や "Stage3Hard" → 3 のように数値部分を取得する
    /// 数値がない場合は 0 を返す
    /// </summary>
    public static int GetStageNumber(string stageName)
    {
        if (string.IsNullOrEmpty(stageName)) return 0;

        var match = Regex.Match(stageName, @"\d+"); // 最初に見つかった数字を取得
        return match.Success && int.TryParse(match.Value, out int number) ? number : 0;
    }

    /// <summary>
    /// "Stage3Hard" → "Hard" のように数字以降の文字列を取得
    /// </summary>
    private static string GetStageSuffix(string stageName)
    {
        var match = Regex.Match(stageName, @"Stage\d+(.*)"); // "Stage3Hard" → "Hard"
        return match.Success ? match.Groups[1].Value : "";
    }

    /// <summary>
    /// 現在のシーン名が "Stage" + 数字（+ 英語）の形式になっていると仮定し、
    /// 数字部分を＋１したシーンをロードする。
    /// ただし、ボスステージ（StageBOSS_STAGE_NUMBER）の場合は "StageSelect" シーンをロードする。
    /// 例:
    ///   - "Stage3" → "Stage4"
    ///   - "Stage3Hard" → "Stage4Hard"
    ///   - "Stage5" (ボスステージ) → "StageSelect"
    ///   - "Stage5Hard" (ボスステージ) → "StageSelect"
    /// </summary>
    /// <param name="currentSceneName">現在のシーン名</param>
    public static void LoadNextStage(string currentSceneName)
    {
        // 現在のシーンがボスステージなら、"StageSelect" シーンへ遷移
        if (IsBossStage(currentSceneName))
        {
            SceneManager.LoadScene("StageSelect");
            return;
        }

        // シーン名が "Stage" で始まっているか確認
        if (currentSceneName.StartsWith("Stage"))
        {
            int currentStageNumber = GetStageNumber(currentSceneName);
            string suffix = GetStageSuffix(currentSceneName); // 例: "Hard" の部分

            if (currentStageNumber > 0)
            {
                int nextStageNumber = currentStageNumber + 1;
                string nextSceneName = "Stage" + nextStageNumber.ToString() + suffix;

                // 次のステージがビルド設定にあるか確認
                List<string> stageList = GetStageSceneNamesInBuildSettings();
                if (stageList.Contains(nextSceneName))
                {
                    SceneManager.LoadScene(nextSceneName);
                }
                else
                {
                    Debug.LogError($"次のステージがビルド設定にありません: {nextSceneName}");
                }
            }
            else
            {
                Debug.LogError($"現在のシーン名の数字部分をパースできませんでした: {currentSceneName}");
            }
        }
        else
        {
            // "Stage" で始まっていない場合、チュートリアル扱いとして "Stage1" に移動
            Debug.LogError($"現在のシーン名は 'Stage' で始まっていません: {currentSceneName}");
            SceneManager.LoadScene("Stage1");
        }
    }



    /// <summary>
    /// 指定されたシーン名がボスステージかどうかを判定する。
    /// </summary>
    public static bool IsBossStage(string sceneName)
    {
        const int BOSS_STAGE_NUMBER = 5; // ボスステージ番号

        if (sceneName.StartsWith("Stage"))
        {
            int stageNumber = GetStageNumber(sceneName);
            return stageNumber == BOSS_STAGE_NUMBER;
        }
        return false;
    }


    /// <summary>
    /// シーン名から "Stage + 数字" の部分のみを取得する。
    /// 例: "Stage3Hard" → "Stage3"
    /// </summary>
    public static string GetBaseStageName(string stageName)
    {
        var match = Regex.Match(stageName, @"^(Stage\d+)");
        return match.Success ? match.Groups[1].Value : "";
    }

}
