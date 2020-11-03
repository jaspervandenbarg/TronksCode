using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// dit script regelt het gedrag en de acties van de AI voor de enemy
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [HideInInspector] protected Entity entity;
    protected NavMeshAgent myNavAgent;

    protected GameObject turrent;
    [SerializeField] protected GameObject nonRotatingLink;
    private GameObject barrelEnd;
    protected GameObject playerTarget;

    //voor het draaien van de turrent en body
    [SerializeField] protected float turrentRotationSpeed;
    protected int rotationDirection;
    protected float rotationSpeed;
    protected Quaternion startRotation;

    //voor raycast reflectie
    Vector3 outDirection;
    LayerMask wall = 9;
    public int raycastReflections;

    //bools om te checken of iets gedaan word
    protected bool targetLost;

    //bools om te checken wat de ai kan
    [SerializeField] protected bool canLookAt;
    [SerializeField] private bool canWander;
    [SerializeField] private bool canShoot;     //voor testing
    [SerializeField] private bool canLayMine;
    [SerializeField] private bool canInvisible;

    //voor het wanderen van de enemy
    private NavMeshHit navHit;
    private Vector3 wanderTarget;
    [Range(1, 32)]
    [SerializeField] private float wanderRange;
    [Range(10, 90)]
    [SerializeField] protected float searchingAngle;

    [HideInInspector] public bool inBulletPath;
    [HideInInspector] public Vector3 bulletposition;
    [HideInInspector] public Vector3 ghostbullet;
    [HideInInspector] public Vector3 evadeDirection;

    [HideInInspector] public MeshRenderer[] meshes;

    protected virtual void Awake()
    {
        //dingen die uit de entity geleend moeten worden;
        //Entity = GetComponentInChildren<Entity>();
        entity = GetComponent<Entity>();
        turrent = entity.turrent;
        barrelEnd = entity.barrelEnd;
        //playerLayer = playerTarget.layer;
        rotationSpeed = entity.rotationSpeed;

        if (GetComponent<NavMeshAgent>() != null)
            myNavAgent = GetComponent<NavMeshAgent>();
        myNavAgent.updateRotation = false;
        if (!canWander)
            myNavAgent.updatePosition = false;
        myNavAgent.speed = entity.movementSpeed;
        targetLost = true;
        //nonrotatinglink start rotatie
        startRotation = nonRotatingLink.transform.rotation;
    }

    protected virtual void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("PlayerPosition");
        rotationDirection = GetRandomDirection();
        myNavAgent.ResetPath();
        //onRoute = true;
        if(canInvisible)
            meshes = gameObject.GetComponentsInChildren<MeshRenderer>();
    }

    protected virtual void Update()
    {
        //als de player dood is wordt er niet geupdate zodat er geen errors komen
        if(playerTarget == null)
        {
            Debug.LogWarning("No player target");
            return;
        }

        //als de enemie ontzichtbaar kan worden.
        if(meshes != null)
            if (!NeverUnload.Instance.timeStopped)
                foreach (MeshRenderer mesh in meshes)
                    if(mesh.enabled)
                        mesh.enabled = false;

        //schiet op de player
        if (CanShootPlayer() && canShoot)
            entity.FireMechanic.Fire();

        //leg mines neer
        if (CanLayMine() && canLayMine)
            entity.FireMechanic.LayMine();

        EnemyBehaviour();
    }

    protected virtual void EnemyBehaviour()
    {
        if (entity.FieldOfView.playerVisible && canLookAt)
            LookAtTarget(turrent, playerTarget.transform.position, turrentRotationSpeed);
        else
        {
            LookInGeneralDirection(playerTarget.transform, searchingAngle);
            LookForTarget(turrent, turrentRotationSpeed, rotationDirection);
        }
    }

    private void UltraSmartBoiAI()                          //this boi extra smart
    {

    }

    //========================================================================================================================================================================

    //laat de enemy naar de player kijken.
    protected void LookAtTarget(GameObject aimer, Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 direction = targetPosition - aimer.transform.position;
        direction = new Vector3(direction.x, 0f, direction.z);
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        aimer.transform.rotation = Quaternion.RotateTowards(aimer.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    //laat de enemy de speler achtervolgen
    protected void ChaseTarget(Vector3 targetPosition)
    {
        myNavAgent.SetDestination(targetPosition);
    }

    //laat de enemy naar de laatste bekende positie van de speler toe gaan;
    protected void SearchTarget(Vector3 target)
    {
        Debug.Log("Searching");
        myNavAgent.SetDestination(target);
        targetLost = true;
    }

    //check of de entity op zijn bestemming is aangekomen
    protected bool DestinationReached()
    {
        //destinynation reached
        if (myNavAgent.remainingDistance < myNavAgent.stoppingDistance) return true;
        else return false;
    }

    //random waarde die -1 of 1 is voor de rotatie richting
    protected int GetRandomDirection()
    {
        if (Random.Range(0, 2) < 1) return -1;
        else return 1;
    }

    //laat de enemy naar een random positie toe gaan;
    protected void Wander()
    {
        if (RandomWanderTarget(transform.position, wanderRange, out wanderTarget))
        {
            myNavAgent.SetDestination(wanderTarget);
        }
    }
    //set random target voor wander
    protected bool RandomWanderTarget(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;     //range was wanderRange
        if (NavMesh.SamplePosition(randomPoint, out navHit, 1.0f, 1<<0))
        {
            result = navHit.position;
            return true;
        }
        else
        {
            result = center;
            return false;
        }
    }

    //RaycastHit hit;
    //wanneer de player in line of sight is
    protected private bool CanShootPlayer()
    {
        Ray ray = new Ray(barrelEnd.transform.position, barrelEnd.transform.forward);
        RaycastHit hit;
        int reflected = raycastReflections;

        for (int i = 0; i <= raycastReflections; i++)
        {
            if (reflected == raycastReflections)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.layer == wall)
                    {
                        outDirection = Vector3.Reflect(ray.direction, hit.normal);

                        ray = new Ray(hit.point, outDirection);
                    }
                    else if (hit.transform.tag == "PlayerPosition") return true;
                    else return false;
                }
            }
            else if (reflected > 0 && reflected < raycastReflections)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.layer == wall)
                    {
                        outDirection = Vector3.Reflect(ray.direction, hit.normal);

                        ray = new Ray(hit.point, outDirection);
                    }
                    else if (hit.transform.name == "PlayerPosition") return true;
                    else return false;
                }
            }
        }
        return false;
    }

    //mijnen leggen
    protected bool CanLayMine()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 8.0f);
        foreach(Collider col in hits)
        {
            if (col.tag == "Mine") return false;
        }
        return true;
    }

    //onwijken van de projectiles
    protected void EvadeBullet()
    {
        myNavAgent.ResetPath();
        myNavAgent.velocity = evadeDirection.normalized * entity.movementSpeed;
    }

    //laat enemies altijd richting de player kijken
    bool passedTarget = true;
    protected void LookInGeneralDirection(Transform target, float maxAngle)
    {
        Vector3 direction = (target.position - turrent.transform.position).normalized;
        float angle = Mathf.Abs(Vector3.Angle(turrent.transform.forward, direction));

        if (angle < 3) passedTarget = true;

        if(angle >= maxAngle && passedTarget)
        {
            passedTarget = false;
            rotationDirection *= -1;
        }
    }
    //turrent random laten ronddraaien
    protected void LookForTarget(GameObject aimer, float rotationSpeed, int direction)
    {
        aimer.transform.Rotate(0, direction * rotationSpeed * Time.deltaTime, 0);
    }
}
