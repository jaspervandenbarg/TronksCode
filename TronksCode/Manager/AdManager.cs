using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public enum RewardType
{
    Retry,
    MenuLife,
    GameLife
}

public class AdManager : MonoBehaviour, IUnityAdsListener
{

    private string androidId = "3726181", appleId = "3726180", storeId = "";
    private string videoAd = "video", rewardAd = "rewardedVideo";

    private float masterVolume;

    private UIManagerScript UIManagerScript;
    public RewardType rewardType;

    void Start()
    {
        UIManagerScript = GetComponent<UIManagerScript>();
        InitialzeAds();
    }

    private void InitialzeAds()
    {
        //add listner
        if(!Advertisement.isInitialized) Advertisement.Initialize(androidId, NeverUnload.Instance.testAds);
        Advertisement.AddListener(this);
        //Debug.Log(EditorUserBuildSettings.activeBuildTarget);
    }

    public void ShowAd()
    {
        if (Advertisement.IsReady(rewardAd))
        {
            Advertisement.Show(rewardAd);
            if (UIManagerScript.isMainMenu)
            {
                GameObject.Find("Ad").SetActive(false);
            }
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        //mute volume
        UIManagerScript.AdjustMasterVolume(-80);
    }

    public void OnUnityAdsReady(string placementId)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidError(string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        //throw new System.NotImplementedException();
        //unmute audio

        switch (showResult)
        {
            case ShowResult.Failed:
                UIManagerScript.AdjustMasterVolume(0);
                break;
            case ShowResult.Skipped:
                UIManagerScript.AdjustMasterVolume(0);

                break;
            case ShowResult.Finished:
                UIManagerScript.AdjustMasterVolume(0);
                if (placementId == rewardAd)
                {
                    Debug.Log("watched video");
                    RewardPlayer();
                }
                break;
        }
    }

    // zorg op een manier dat er een verschil is tussen game over en tussen scherm.
    private void RewardPlayer()
    {
        // bij game over wordt op een of andere manier de geselecteerde enum en de volgende enum gecalled
        // finished wordt dus twee keer gecalled voor verschillende enums

        switch (rewardType)
        {
            case RewardType.Retry:
                NeverUnload.Instance.oldLevel = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //change to build index
                //reload level give no lives
                break;
            case RewardType.GameLife:
                //give life and load next level
                NeverUnload.Instance.playerLives++;
                NeverUnload.Instance.oldLevel = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
            case RewardType.MenuLife:
                //give life and go back to main
                //  deze triggerd op een of andere manier wanneer je gameover gaat
                NeverUnload.Instance.playerLives++;
                break;
        }
    }

    public void RateUs()
    {
        //Android
        try
        {
            Application.OpenURL("market://details?id=com.BargProjects.Tronks");
            if (UIManagerScript.isMainMenu)
            {
                GameObject.Find("RateUs").SetActive(false);
                NeverUnload.Instance.rated = true;
            }
        }
        catch
        {
            Debug.LogWarning("Failed to open URL! Internet acces may be restricted");
        }
    }

    void OnDestroy()
    {
        Advertisement.RemoveListener(this);
    }
}
