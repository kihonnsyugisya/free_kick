using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectController : MonoBehaviour
{
    [SerializeField] private StageSelectButton stageSelectButtonPrefab; // ボタンのプレハブ（Textコンポーネントが付いている前提）
    [SerializeField] private Transform buttonParent;     // ボタンを配置する親Transform（例：ScrollViewのContent）

    public static StageSelectController Instance { get; private set; }

    private void Start()
    {
        // "Stage" + 数字 のシーン名リストを取得
        List<string> stageSceneNames = SceneListUtility.GetStageSceneNamesInBuildSettings();

        // 各シーン名に対してボタンを生成し、クリック時にシーンをロードする処理を登録
        foreach (string sceneName in stageSceneNames)
        {
            StageSelectButton stageSelectButton = Instantiate(stageSelectButtonPrefab, buttonParent);

            // ボタンのテキストをシーン名に設定
            stageSelectButton.buttonLabel.text = sceneName;

            // クリック時に該当シーンをロードする処理を追加
            stageSelectButton.button.onClick.AddListener(() => {
                SceneManager.LoadScene(sceneName);
            });
        }
    }
}
