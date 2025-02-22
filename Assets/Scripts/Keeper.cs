using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

public class Keeper : MonoBehaviour
{
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private Animator animator;
    [SerializeField] private UIController uIController;

    private enum Mode
    {
        SittingToYokone,
        Sankakuzuwari,
        Nesoberi,
        NeSumaho,
        StandingUkiUki
    }

    public enum StageMode
    {
        Stage5
    }

    [SerializeField] private StageMode currentMode;

    private readonly Dictionary<StageMode, Mode> stageToModeMap = new()
    {
        { StageMode.Stage5, Mode.SittingToYokone }
    };

    void Start()
    {
        if (stageToModeMap.TryGetValue(currentMode, out Mode mode))
        {
            _ = PlayStageSequence(mode);
        }
        else
        {
            Debug.LogWarning($"未知のモード: {currentMode}");
        }
    }

    private async Task PlayStageSequence(Mode mode)
    {
        uIController.ShowControllUis(false);
        cameraSwitcher.SwitchCamera(CameraSwitcher.CameraType.Replay2);
        await Task.Delay(3500);
        animator.Play(mode.ToString());
        await Task.Delay(2900);
        cameraSwitcher.SwitchCamera(CameraSwitcher.CameraType.Kicker);
        await Task.Delay(1000);
        uIController.ShowControllUis(true);
        uIController.powerSlider.hideArrowSlider();
        uIController.ShowRetryButton(false);
    }
}
