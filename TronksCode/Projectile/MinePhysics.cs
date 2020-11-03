using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EZCameraShake;
public class MinePhysics : MonoBehaviour
{

    Transform parent;
    [HideInInspector] public GameObject parentTank;

    [HideInInspector] public bool armed;
    private bool stopDetection;
    [HideInInspector] public bool collisionEntered;

    private float timer, time;

    [SerializeField] private bool rotationCollider;

    void Awake()
    {
        parent = transform.parent;
        armed = false;
        collisionEntered = false;
        stopDetection = false;
        time = 0;
        timer = 10;
    }

    void Start()
    {
        parentTank = parent.GetComponent<MineExplosion>().parentTank;
        SoundManager.PlaySound(SoundManager.Sounds.MinePlacement);
    }

    void Update()
    {
        if (armed)
        {
            time += Time.deltaTime;
            if (time > timer) Explode();

            if(!collisionEntered)
            {
                //zorgt ervoor dat de collider die de bullet moet vangen op de juiste positie blijft
                if (rotationCollider)
                    transform.Rotate(0, 750 * Time.deltaTime, 0);
            }
            return;
        }

        // zet de mijn op actief als de parent tank op een bepaalde afstand is of dood is
        if (parentTank != null)
        {
            if (Vector3.Distance(parentTank.transform.position, transform.position) > 3) armed = true;
        }
        else armed = true;
    }

    //checkt of er een bullet of entity in collision is zodat de parent kan ontploffen
    void OnTriggerEnter(Collider other)
    {
        if (stopDetection || !armed)
            return;

        if (armed)
        {
            if (other.tag == "PlayerPosition" || other.tag == "Enemy")  //voor player of enemy gewoon ontploffen, de rest wordt door MineExplosionDetection geregeld
                Explode(other.tag);
            if (other.tag == "Bullet")  //bij bullet moet de bullet gelijk verwijderd worden
            {
                other.GetComponent<BulletPhysics>().InitiateObjectKill();
                Explode(other.tag);
            }
        }
    }

    // zet alle explosie waardes op true zodat er niet nog een collision wordt gedetekteerd en de mijn begint met ontploffen
    public void Explode(string otherTag = "")
    {
        parent.GetComponent<MineExplosion>().explode = true;
        foreach (Transform child in parent.GetComponent<MineExplosion>().children)
            if (child != parent.transform)
                Destroy(child.gameObject);
        //Debug.Log(otherTag);
        //Debug.Log("BOOM");
        CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 1.4f);
        SoundManager.PlaySound(SoundManager.Sounds.MineExplosion);
        collisionEntered = true;
        stopDetection = true;
        armed = true;
    }
}
