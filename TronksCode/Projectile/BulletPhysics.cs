using System.Collections;
using UnityEngine;

public class BulletPhysics : MonoBehaviour
{

    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float velocity;
    [SerializeField] private int projectileLife;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float offset;
    private bool hit, active = false;

    [HideInInspector] public GameObject parent;

    EnemyAI EnemyAI;
    [SerializeField] private Transform Right;
    [SerializeField] private Transform Left;
    [Range(1.0f, 30.0f)]
    [SerializeField] private float chechingDistance;
    [SerializeField] private GameObject hitParticle;

    void Awake()
    {
        hit = false;
        SoundManager.PlaySound(SoundManager.Sounds.BulletBounce);
    }

    void Start()
    {
        if (parent.GetComponent<Entity>().isPlayerEntity)
        {
            projectileLife = NeverUnload.Instance.playerMaxBulletBounce;
            velocity = NeverUnload.Instance.projectileVelocity;
        }
        else
        {
            projectileLife = parent.GetComponent<EnemyAI>().raycastReflections;
        }

        StartCoroutine(ActivateDelay(0.05f));
    }

    void Update()
    {
        transform.Translate(Vector3.forward * velocity * Time.deltaTime, Space.Self);
        Bounce();
        InPath();

        // wanneer de bullet toch perongeluk door de muur gaat
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        if (!onScreen) InitiateObjectKill(1);

    }

    //stuiteren op muren;
    void Bounce()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Time.deltaTime * velocity + offset, collisionMask))
        {
            GameObject part = Instantiate(hitParticle, transform.position + (transform.forward * offset), transform.rotation);
            Destroy(part, part.GetComponent<ParticleSystem>().startLifetime);

            if (projectileLife == 0)
                InitiateObjectKill();
            else
            {
                Vector3 reflectDir = Vector3.Reflect(ray.direction, hit.normal);
                float rot = 90 - Mathf.Atan2(reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, rot, 0);
                projectileLife--;
            }
            SoundManager.PlaySound(SoundManager.Sounds.BulletBounce);
        }
    }

    //objectkill voor bullet
    public void InitiateObjectKill(float killTime = 0)
    {
        if (EnemyAI != null)
            EnemyAI.inBulletPath = false;

        if (parent != null)
        {
            parent.GetComponent<FireMechanic>().bulletsFired.Remove(this.gameObject);
            //parent.GetComponent<FireMechanic>().ChangeAmmo();
        }

        Destroy(gameObject, killTime);
    }

    //collision met entity of bullet
    private void OnTriggerEnter(Collider other)
    {
        if (hit || !active) return;

        if (other.tag == "PlayerPosition" || other.tag == "Enemy")
        {
            GameObject part = Instantiate(hitParticle, transform.position + (transform.forward * offset), transform.rotation);
            Destroy(part, part.GetComponent<ParticleSystem>().startLifetime);
            hit = true;
            other.GetComponent<Entity>().InitiateObjectKill();
            this.InitiateObjectKill();
        }
        else if (other.tag == "Bullet")
        {
            GameObject part = Instantiate(hitParticle, transform.position + (transform.forward * offset), transform.rotation);
            Destroy(part, part.GetComponent<ParticleSystem>().startLifetime);
            hit = true;
            other.GetComponent<BulletPhysics>().InitiateObjectKill();
            SoundManager.PlaySound(SoundManager.Sounds.BulletBounce);
        }
    }

    private bool rayHitTarget = true;
    //checkt of een enemy in de line of sight van een bullet is
    //lets the enemy AI know if its in the path of the bullet
    private void InPath()
    {
        if (EnemyAI != null)
            EnemyAI.inBulletPath = false;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, 0.5f, out hit, chechingDistance))
        {
            if (hit.transform.tag == "Enemy")
            {
                EnemyAI = hit.transform.GetComponentInParent<EnemyAI>();
                EnemyAI.inBulletPath = true;
                if (rayHitTarget)
                {
                    Vector3 ghostPosition = RightOrLeft();
                    EnemyAI.evadeDirection = new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(ghostPosition.x, 0, ghostPosition.z);
                    rayHitTarget = false;
                }
            }
            else
                rayHitTarget = true;
        }

        //StartCoroutine(CheckPath());
    }
    private Vector3 RightOrLeft()
    {
        if (Random.Range(0, 2) < 1)
            return Right.position;
        else
            return Left.position;
    }

    private IEnumerator ActivateDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        active = true;
    }
}
