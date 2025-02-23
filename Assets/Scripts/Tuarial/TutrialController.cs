using System.Collections.Generic;
using Coffee.UIExtensions;
using GoogleMobileAds.Sample;
using NUnit.Framework;
using UnityEngine;

public class TutrialController : MonoBehaviour
{
    [SerializeField] private List<TutrialMessage> messages;
    [SerializeField] private UnmaskRaycastFilter filter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SaveLoadManager.SaveStagePrefix(StagePrefix.Tu);
        foreach (TutrialMessage message in messages) 
        {
            message.nextButton.onClick.AddListener(() => { 
                message.gameObject.SetActive(false);
                message.unmask.gameObject.SetActive(false);
                if (message.nextMessage != null)
                {
                    message.nextMessage.gameObject.SetActive(true);
                    filter.targetUnmask = message.nextMessage.unmask;
                    message.nextMessage.unmask.gameObject.SetActive(true);
                }
                else {
                    filter.gameObject.SetActive(false);
                }                       
            });
        }
    }
}
