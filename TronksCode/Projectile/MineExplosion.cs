using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//om de breekbare muren te breken
//ik wist zo snel ff geen andere oplossing
public class MineExplosion : MonoBehaviour
{
    [SerializeField] private GameObject destroyedBlock;

    [HideInInspector] public bool armed;
    [HideInInspector] public GameObject parentTank;
    [HideInInspector] public bool explode;

    private float maxSize;
    private float minSize;
    private float almostMaxSize;
    private bool growing;

    [HideInInspector] public Transform[] children;

    private void Awake()
    {
        growing = true;
        maxSize = 4.5f;
        almostMaxSize = maxSize - 0.2f;
        minSize = 0.1f;

        children = GetComponentsInChildren<Transform>();
        armed = false;

        NeverUnload.Instance.minesInGame.Add(this.gameObject);
    }

    private void Update()
    {
        if (explode)
        {
            Explode();

            //om een of andere reden werkt de collider zelf niet werkt wordt het met een lijst en afstanden gedaan
            foreach (GameObject mine in NeverUnload.Instance.minesInGame)
                if (mine != this.gameObject && mine != null)
                    if (Vector3.Distance(mine.transform.position, transform.position) <= (transform.localScale.x - 0.5f))
                        if (mine.GetComponentInChildren<MinePhysics>() != null)
                            mine.GetComponentInChildren<MinePhysics>().Explode(this.tag);
        }
    }

    //checkt of er iets binnen de radius van de ontploffing is
    void OnTriggerEnter(Collider other)
    {
        if (armed)
        {
            if (other.tag == "BreakableBlock")
                destroyBlocks(other.transform);

            if (other.tag == "Enemy" || other.tag == "PlayerPosition")
                other.GetComponent<Entity>().InitiateObjectKill();

            if (other.tag == "Bullet")
                other.GetComponent<BulletPhysics>().InitiateObjectKill();
        }
    }

    //ontploffen van de mine rotatie
    public void Explode(float growth = 10f, float rotation = 500f)
    {
        armed = true;

        //wanneer de explosie weer gekrompen is en weg moet
        if (!growing && transform.localScale.x < minSize)
            InitiateObjectKill();

        //wanneer de explosie op zijn grootst is en moet krimpen
        if (transform.localScale.x >= maxSize && growing)
        {
            growing = false;
            //foreach(GameObject mine in parentTank.GetComponent<Entity>().FireMechanic.minesLayed)
            //{
            //    if(Vector3.Distance(mine.transform.position, this.transform.position) < maxSize && mine != this.gameObject)
            //        mine.GetComponent<MinePhysics>().Explode(mine.tag + "detected form explosion");
            //}
        }

        if (growing && transform.localScale.x < almostMaxSize)             //groei snel
        {
            transform.localScale += new Vector3(growth, growth, growth) * Time.deltaTime;
            transform.transform.Rotate(0, -rotation * Time.deltaTime, 0);
        }
        else if (growing && transform.localScale.x > almostMaxSize)         //groei even langzaam
        {
            transform.localScale += new Vector3(growth, growth, growth) * Time.deltaTime * 0.5f;
            transform.transform.Rotate(0, -rotation * Time.deltaTime, 0);
        }
        else                                                            //krimp snel
        {
            transform.localScale -= new Vector3(growth, growth, growth) * 3f * Time.deltaTime;
            transform.transform.Rotate(0, 3f * rotation * Time.deltaTime, 0);
        }
    }

    void InitiateObjectKill()
    {
        if(parentTank != null)
        parentTank.GetComponent<Entity>().FireMechanic.minesLayed.Remove(this.gameObject);
        Destroy(this.gameObject);
        NeverUnload.Instance.minesInGame.Remove(this.gameObject);     
    }

    void destroyBlocks(Transform trans)
    {
        GameObject block = Instantiate(destroyedBlock, trans.position, trans.rotation);
        Destroy(trans.gameObject);

        foreach (Rigidbody ridge in block.GetComponentsInChildren<Rigidbody>())
        {
            if (ridge.transform != block.transform)
            {
                Vector3 force = (ridge.position - this.transform.position).normalized;
                float distance = Mathf.Abs(Vector3.Distance(ridge.position, this.transform.position));
                ridge.AddForce(force * (500 / distance));
            }
        }
        Destroy(block, Random.Range(2, 3));
    }
}
