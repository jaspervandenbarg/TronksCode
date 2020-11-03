using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.Audio;

/// <summary>
/// dit script is voor
///     het bewaren van alle player stats,
///     het bewaren van game stats,
///     het updaten van het level tijdens speeltijd,
///     updaten van gegevens zoals geluid tijdens speeltijd
/// </summary>
public class NeverUnload : MonoBehaviour
{
    public static NeverUnload Instance { get; set; }

    public int playerLives, playerMagMax, playerMineMax, playerMaxBulletBounce;
    public float playerMovementSpeed, playerFireSpeed, projectileVelocity;
    
    [HideInInspector] public bool timeStopped, rated;

    [SerializeField] public bool testing;

    [HideInInspector] public int currentLevel, oldLevel, levelGameOvers;
    //private UIManagerScript UIManagerScript;

    public List<GameObject> minesInGame = new List<GameObject>();
    private int currentLayedMines;
    [HideInInspector] public NavMeshSurface navMeshSurface;

    [HideInInspector] public int destroyedTanks, bulletsFired, minesLayed;
    [HideInInspector] public float slomoSeconds = 0, realSeconds = 0;
    private float currentTimeScale;

    [HideInInspector] public float musicLevel = 1, effectLevel = 1;

    //als alle enemies dood zijn kan player niet alsnog dood gaan
    [HideInInspector] public bool playerKillable;
    public bool mobile, testAds;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        currentLevel = 1;
        levelGameOvers = 0;
        playerKillable = true;

        if (testing) return;

        SoundManager.PlaySound(SoundManager.Sounds.Music);
        StartCoroutine(LoadMainMenu());
    }

    private void Update()
    {
        // zorgt dat currentLevel geupdate wordt naar de build index van het huidige level
        // hier kan ook iets gedaan worden dat alleen de eerste keer moet gebeuren dat een bepaald level geladen wordt
        if (currentLevel != null && SceneManager.GetActiveScene().buildIndex >= 1)
        {
            if (currentLevel != SceneManager.GetActiveScene().buildIndex)
            {
                currentLevel = SceneManager.GetActiveScene().buildIndex;
                levelGameOvers = 0;
                //if (currentLevel % 3 == 0) playerLives++; //player lives
            }
        }

        //zorgt dat het geluid de goede snelheid heeft
        if (Time.timeScale != currentTimeScale)
        {
            currentTimeScale = Time.timeScale;
            SoundAssets.Instance.Master.audioMixer.SetFloat("masterPitch", Time.timeScale);
        }

        //zorgt dat de navmesh word geupdate als er een mine meer of minder in het spel komt
        if (currentLayedMines != minesInGame.Count)
        {
            currentLayedMines = minesInGame.Count;
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        }
    }

    //zorgt dat de eerste scene word overgeslagen, de scene die dit object bevat, en gelijk naar het hoofdmenu gegaan word
    private IEnumerator LoadMainMenu()
    {
        yield return null;
        SceneManager.LoadScene(1);
    }

    //reset stats wanneer je in main menu bent
    public void ResetStats()
    {
        playerLives = 3;
        //destroyedTanks = 0;
        //bulletsFired = 0;
        //minesLayed = 0;
        //slomoSeconds = 0;
        //realSeconds = 0;
    }

    public bool IsDifferentLevel()
    {
        return currentLevel != oldLevel ? true : false;
    }
}
