using UnityEngine;
using UniRx;
using NUnit.Framework;
using System.Collections.Generic;

public class AudioController : MonoBehaviour
{
    [SerializeField] private SoccerBall soccerBall;
    [SerializeField] private List<AudioClip> clips;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private FreeKicker freeKicker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soccerBall.OnBallHit.Subscribe(_ => {
            audioSource.PlayOneShot(clips[0]);
        }).AddTo(this);

        freeKicker.kickerKnee.OnBallHit.Subscribe(_ => {
            audioSource.PlayOneShot(clips[0]);
        }).AddTo(this);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
