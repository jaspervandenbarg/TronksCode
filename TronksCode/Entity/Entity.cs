using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Entity : MonoBehaviour
{
    [SerializeField] public float movementSpeed;
    [SerializeField] public float movementAccelerationFactor;
    [SerializeField] public float rotationSpeed;

    //gameobjects
    //[SerializeField] public GameObject tankBody; //kan gebruikt worden om de turrent en body onafhankelijk te laten roteren. moet in ai gedaan worden
    [SerializeField] public GameObject turrent;
    [SerializeField] public GameObject barrelEnd;
    [SerializeField] private GameObject tankTrail;
    [SerializeField] private GameObject deathParticle;

    [HideInInspector] public bool isPlayerEntity;

    //scripts
    [HideInInspector] public FireMechanic FireMechanic;
    [HideInInspector] public FieldOfView FieldOfView;
    [HideInInspector] public GameStatusScript GameStatusScript;
    [HideInInspector] public UIManagerScript UIManagerScript;

    private GameObject parent;

    private float movementAcceleration;
    private Vector3 lastDirection;

    private Rigidbody ridge;

    private AudioSource audioSource;
    private AudioClip audioClip;
    private float maxVolume;

    private void Awake()
    {
        //playerTarget = GameObject.FindGameObjectWithTag("PlayerPosition");
        //dit werkt niet daarom wat hier boven is
        //player = Player.Instance.GetComponentInChildren<GameObject>();
        if(transform.parent != null) parent = transform.parent.gameObject;
        //isPlayerEntity = false;
        movementAcceleration = 0;
        lastDirection = Vector3.zero;

        
        ridge = GetComponent<Rigidbody>();
    }

    void Start()
    {
        FireMechanic = GetComponent<FireMechanic>();
        FieldOfView = turrent.GetComponent<FieldOfView>();
        GameStatusScript = GameObject.FindGameObjectWithTag("GameStatusCanvas").GetComponent<GameStatusScript>();
        UIManagerScript = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManagerScript>();

        if (isPlayerEntity)
        {
            if(NeverUnload.Instance != null)
            movementSpeed = NeverUnload.Instance.playerMovementSpeed;
        }
        else
        {
            GameStatusScript.enemiesInLevel.Add(this.gameObject);
            GameStatusScript.UpdateEnemyCount();
        }

        InitializeSound();
    }

    public void Move(Vector3 direction)
    {
        //als het spel gepauzeerd is update dan niet
        if (NeverUnload.Instance.timeStopped) return;
        
        //zorgt ervoor dat de entity niet door explosies opzij kan worden geduwt
        if(ridge.velocity.magnitude > 0)
            ridge.velocity = new Vector3(0, 0, 0);
        if(ridge.angularVelocity.magnitude >0)
            ridge.angularVelocity = new Vector3(0, 0, 0);

        //verander acceleration
        if(direction.magnitude > 0)
        {
            movementAcceleration += movementAccelerationFactor;
            lastDirection = direction;

            //rotation tank
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

            if (audioSource.volume < maxVolume)
                audioSource.volume += 0.01f;
        }
        else
        {
            movementAcceleration -= movementAccelerationFactor;
            if(audioSource.volume > 0)
                audioSource.volume -= 0.01f;
        }

        //clamp acceleration tussen 0 en 1
        movementAcceleration = Mathf.Clamp(movementAcceleration, 0f, 1f);

        //move
        transform.Translate(lastDirection * movementSpeed * movementAcceleration * Time.deltaTime, Space.World);            
    }

    //wanneer de entity gekilled wordt.
    public void InitiateObjectKill()
    {
        //als alle enemies dood gaan is de speler imuum
        if (isPlayerEntity && !NeverUnload.Instance.playerKillable) return;

        //if entity is player entity
        if (isPlayerEntity)
        {
            if(NeverUnload.Instance.playerLives == 1)
            {
                UIManagerScript.GetComponent<AdManager>().rewardType = RewardType.Retry;
                UIManagerScript.GameOver();
            }
            else
            {
                NeverUnload.Instance.playerLives--;
                UIManagerScript.LoadLevel(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            GameStatusScript.enemiesInLevel.Remove(this.gameObject);
            GameStatusScript.UpdateEnemyCount();            
            NeverUnload.Instance.destroyedTanks++;
            //Slider slider = GameObject.FindGameObjectWithTag("Slomobar").GetComponent<Slider>();
            //slider.value += 20;
        }

        Destroy(this.gameObject);
        tankTrail.transform.parent = null;
        Destroy(tankTrail, tankTrail.GetComponentInChildren<ParticleSystem>().startLifetime);
        GameObject part = Instantiate(deathParticle, this.transform);
        part.transform.parent = null;
        Destroy(part, part.GetComponentInChildren<ParticleSystem>().startLifetime);

        CameraShaker.Instance.ShakeOnce(1.5f, 1.5f, 0.1f, 0.6f);
        SoundManager.PlaySound(SoundManager.Sounds.EntityDeath);
    }

    //geluid voor de tanks
    private void InitializeSound()
    {
        audioClip = (AudioClip)Resources.Load("Effects/Engine1.2");
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SoundAssets.Instance.Effects;
        audioSource.clip = audioClip;
        maxVolume = isPlayerEntity ? 0.9f : 0.01f;
        audioSource.volume = maxVolume;
        audioSource.loop = true;
        audioSource.Play();
    }
}
