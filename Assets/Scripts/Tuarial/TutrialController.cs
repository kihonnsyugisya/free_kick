using System.Collections.Generic;
using Coffee.UIExtensions;
using NUnit.Framework;
using UnityEngine;

public class TutrialController : MonoBehaviour
{
    [SerializeField] private List<TutrialMessage> messages;
    [SerializeField] private UnmaskRaycastFilter filter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }

    //[SerializeField] private Tex
}
