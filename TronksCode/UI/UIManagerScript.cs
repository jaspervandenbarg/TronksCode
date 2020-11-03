using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// dit script zorgt voor:
///     het laden van levels,
///     pauzer van het spel,
///     volume aanpassen,
///     ui elementen laten zien
/// </summary>


public class UIManagerScript : MonoBehaviour
{
    [HideInInspector] public bool paused;
    [HideInInspector] public bool playerDead;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject gameStatusCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] public GameObject interLevelCanvas;
    [SerializeField] private GameObject gameOverPanel, winPanel, retryButton, rateButton;
    GameStatusScript GameStatusScript;
    [SerializeField] public bool isMainMenu;

    private bool cantPauze;
    [HideInInspector] public bool loading;
    int nextLevel;
    bool muted = false;

    void Awake()
    {
        GameStatusScript = gameStatusCanvas.GetComponent<GameStatusScript>();
        playerDead = false;
        paused = false;
        loading = false;
    }

    private void Start()
    {
        NeverUnload.Instance.playerKillable = true;
        if (SceneManager.GetActiveScene().buildIndex > 1 && !NeverUnload.Instance.testing)
            StartCoroutine(InterLevelCanvas());

        if ((NeverUnload.Instance.oldLevel - 1) >= 5 && !NeverUnload.Instance.rated)
            rateButton.SetActive(true);
    }

    void Update()
    {
        //check voor enemies in scene, als alle enemies dood zijn laad volgende level
        if (GameStatusScript.enemiesInLevel.Count == 0 && !isMainMenu && !loading)
        {
            LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }

        //test volume
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!muted)
            {
                AdjustMasterVolume(-80);
                muted = true;
            }
            else
            {
                AdjustMasterVolume(0);
                muted = false;
            }
        }
    }

    public void PauseGame()
    {
        //pause menu
        if (!isMainMenu && !cantPauze)
        {
            if (!paused)
            {
                Time.timeScale = 0;
                paused = true;
                NeverUnload.Instance.timeStopped = true;
                pauseMenuCanvas.SetActive(true);
            }
            else
            {
                pauseMenuCanvas.SetActive(false);
                paused = false;
                NeverUnload.Instance.timeStopped = false;
                Time.timeScale = 1;
            }
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        NeverUnload.Instance.timeStopped = false;
        NeverUnload.Instance.ResetStats();
        NeverUnload.Instance.oldLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(1);
        //Application.UnloadLevel(Application.loadedLevel);
    }

    //laad level
    private IEnumerator _LoadLevel(int level)
    {
        NeverUnload.Instance.playerKillable = false;
        NeverUnload.Instance.oldLevel = SceneManager.GetActiveScene().buildIndex;
        loading = true;
        cantPauze = true;
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(2);
        Time.timeScale = 1;
        cantPauze = false;
        if (level + 1 > SceneManager.sceneCountInBuildSettings)
            Win();
        else
            SceneManager.LoadScene(level);
    }
    public void LoadLevel(int level)
    {
        StartCoroutine(_LoadLevel(level));
    }
    //laad altijd het eerste level
    public void PlayGame()
    {
        NeverUnload.Instance.oldLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(2);
    }
    //sluit het spel
    public void ExitGame()
    {
        Application.Quit();
    }

    //wanneer je wint
    private IEnumerator _Win()
    {
        playerDead = true;
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(2);
        gameOverCanvas.SetActive(true);
        gameOverPanel.SetActive(false);
        winPanel.SetActive(true);
        paused = true;
        Time.timeScale = 0;
        NeverUnload.Instance.timeStopped = true;
    }
    private void Win()
    {
        StartCoroutine(_Win());
    }

    //gameover scherm
    private IEnumerator _GameOver()
    {
        playerDead = true;
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(2);
        gameOverCanvas.SetActive(true);
        if (NeverUnload.Instance.levelGameOvers > 0) retryButton.SetActive(false);
        NeverUnload.Instance.levelGameOvers++;
        paused = true;
        Time.timeScale = 0;
        NeverUnload.Instance.timeStopped = true;
    }

    public void GameOver()
    {
        StartCoroutine(_GameOver());
    }

    //als het level geladen wordt
    private IEnumerator InterLevelCanvas()
    {
        Time.timeScale = 0;
        cantPauze = true;
        NeverUnload.Instance.timeStopped = true;
        yield return new WaitForSecondsRealtime(4);
        interLevelCanvas.SetActive(false);
        yield return new WaitForSecondsRealtime(0.25f);
        NeverUnload.Instance.timeStopped = false;
        cantPauze = false;
        Time.timeScale = 1;
    }

    //verranderd het volume
    public void AdjustMusicVolume(float volume)
    {
        SoundAssets.Instance.Music.audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
        NeverUnload.Instance.musicLevel = volume;
    }
    public void AdjustEffectVolume(float volume)
    {
        SoundAssets.Instance.Effects.audioMixer.SetFloat("effectVolume", Mathf.Log10(volume) * 20);
        NeverUnload.Instance.effectLevel = volume;
    }
    public void AdjustMasterVolume(float volume)
    {
        SoundAssets.Instance.Master.audioMixer.SetFloat("masterVolume", volume);
    }
}
