using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class IrisCanvas : MonoBehaviour
{
    [SerializeField] RectTransform unmask;
    readonly Vector2 IRIS_IN_SCALE = new Vector2(30, 30);
    readonly float SCALE_DURATION = 1;

    //public void IrisIn()
    //{
    //    unmask.DOScale(IRIS_IN_SCALE, SCALE_DURATION).SetEase(Ease.InCubic);
    //}

    //public void IrisOut()
    //{
    //    unmask.DOScale(new Vector3(0, 0, 0), SCALE_DURATION).SetEase(Ease.OutCubic);
    //}

    //private async void Start()
    //{
    //    Debug.Log("DarkScreenはデフォルトでONにしといて。一時的にOFFにしてるだけ（開発しにくいから）");

    //    IrisOut();
    //}
}
