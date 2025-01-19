using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdUnitIDs", menuName = "ScriptableObjects/AdUnitIDs")]
public class AdUnitIdSetting : ScriptableObject
{
    [Header("テスト広告を利用しますか？")]
    public bool isTest = false;

    [Header("バナー")]
    public string bannerIOS;
    public string bannerAndroid;
    [Space(4f)]
    public string testBannerIOS = "ca-app-pub-3940256099942544/2934735716";
    public string testBannerAndroid = "ca-app-pub-3940256099942544/6300978111";

    [Space(8f)]
    [Header("インステ")]
    public string interstitialIOS;
    public string interstitialAndroid;
    [Space(4f)]
    public string testInterstitialIOS = "ca-app-pub-3940256099942544/4411468910";
    public string testInterstitialAndroid = "ca-app-pub-3940256099942544/1033173712";

    [Space(8f)]
    [Header("リワード")]
    public string rewardIOS;
    public string rewardAndroid;
    [Space(4f)]
    public string testRewardIOS = "ca-app-pub-3940256099942544/1712485313";
    public string testRewardAndroid = "ca-app-pub-3940256099942544/5224354917";

    [Space(8f)]
    [Header("アプリ起動")]
    public string appOpenIOS;
    public string appOpenAndroid;
    [Space(4f)]
    public string testAppOpenIOS = "ca-app-pub-3940256099942544/5575463023";
    public string testAppOpenAndroid = "ca-app-pub-3940256099942544/9257395921";
}
