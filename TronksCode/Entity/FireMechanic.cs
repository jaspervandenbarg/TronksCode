using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FireMechanic : MonoBehaviour
{

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject barrelEnd;
    [SerializeField] private GameObject mine;
    [HideInInspector] private int bulletMagazine;
    [HideInInspector] private int mineMagazine;
    [SerializeField] public int maxBulletMagazine;
    [SerializeField] public int maxMineMagazine;
    [SerializeField] private bool canShoot;
    private bool inCollider;
    [SerializeField] private bool canLayMine;
    [SerializeField] private float fireSpeed;
    private Entity entity;
    private int checkLayedMines;

    public List<GameObject> bulletsFired = new List<GameObject>();
    public List<GameObject> minesLayed = new List<GameObject>();
    private NavMeshSurface NavMeshSurface;

    void Awake()
    {
        entity = gameObject.GetComponent<Entity>();
    }

    
    //void FixedUpdate()
    //{
    //    if(NavMeshSurface == null)
    //        NavMeshSurface = GameObject.FindGameObjectWithTag("NavMesh").GetComponent<NavMeshSurface>();

    //    if (checkLayedMines != minesLayed.Count)
    //    {
    //        checkLayedMines = minesLayed.Count;
    //        NavMeshSurface.BuildNavMesh();
    //    }
    //}
    

    void Start()
    {
        if (entity.isPlayerEntity)
        {
            if(NeverUnload.Instance != null)
            {
                fireSpeed = NeverUnload.Instance.playerFireSpeed;
                maxBulletMagazine = NeverUnload.Instance.playerMagMax;
                maxMineMagazine = NeverUnload.Instance.playerMineMax;
            }
            mineMagazine = maxMineMagazine;
            bulletMagazine = maxBulletMagazine;
        }
        else
        {
            bulletMagazine = maxBulletMagazine;
        }

        canShoot = true;
        canLayMine = true;

        //ChangeAmmo();
    }

    //schieten van de kogel
    public void Fire()
    {
        if (canShoot && !inCollider)
        {
            if (bulletsFired.Count < maxBulletMagazine)
            {
                GameObject go = Instantiate(bullet, barrelEnd.transform.position, barrelEnd.transform.rotation);
                bulletsFired.Add(go);
                go.GetComponent<BulletPhysics>().parent = this.gameObject;
                if (entity.isPlayerEntity) NeverUnload.Instance.bulletsFired++;
                //ChangeAmmo();
                StartCoroutine(CanShoot(fireSpeed));
            }
        }
    }

    public void LayMine()
    {
        if (canLayMine)
        {
            if (minesLayed.Count < maxMineMagazine)
            {
                GameObject stay = Instantiate(mine, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.Euler(0, 0, 0));
                minesLayed.Add(stay);
                stay.GetComponent<MineExplosion>().parentTank = this.gameObject;
                StartCoroutine(CanLayMine(0.1f));
                if(entity.isPlayerEntity) NeverUnload.Instance.minesLayed++;
            }
        }
    }

    IEnumerator CanShoot(float shotCooldown)
    {
        canShoot = false;
        if(entity.isPlayerEntity)
            yield return new WaitForSecondsRealtime(shotCooldown);
        else
            yield return new WaitForSeconds(shotCooldown);
        canShoot = true;
    }
    IEnumerator CanLayMine(float mineCooldown)
    {
        canLayMine = false;
        yield return new WaitForSeconds(mineCooldown);
        canLayMine = true;
    }

    //public void ChangeAmmo()
    //{
    //    bulletMagazine = maxBulletMagazine - bulletsFired.Count;
    //}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9) inCollider = true;
    }
    private void OnTriggerExit(Collider other)
    {
        inCollider = false;
    }

}
