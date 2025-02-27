using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class StagePreviewCam : MonoBehaviour
{
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private UIController uIController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await PlayStageSequence();
    }

    private async Task PlayStageSequence()
    {
        uIController.ShowControllUis(false);
        cameraSwitcher.SwitchCamera(CameraSwitcher.CameraType.Replay2);
        await Task.Delay(3200);
        await Task.Delay(1000);
        cameraSwitcher.SwitchCamera(CameraSwitcher.CameraType.Kicker);
        await Task.Delay(900);
        uIController.ShowControllUis(true);
        uIController.powerSlider.hideArrowSlider();
        uIController.ShowRetryButton(false);
    }
}
