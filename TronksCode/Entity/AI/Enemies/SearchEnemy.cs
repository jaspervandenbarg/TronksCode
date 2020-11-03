using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchEnemy : EnemyAI
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void EnemyBehaviour()
    {
        //checks voor searching
        if (entity.FieldOfView.playerVisible) targetLost = false;

        //waar de ai naar moet kijken
        if (entity.FieldOfView.playerVisible && canLookAt)
            LookAtTarget(turrent, playerTarget.transform.position, turrentRotationSpeed);
        else
        {
            LookInGeneralDirection(playerTarget.transform, searchingAngle);
            LookForTarget(turrent, turrentRotationSpeed, rotationDirection);
        }

        //waar de ai naartoe moet bewegen
        if (inBulletPath)
            EvadeBullet();
        else if (!entity.FieldOfView.playerVisible && !targetLost)
            SearchTarget(playerTarget.transform.position);
        else if (DestinationReached())
            Wander();

        //zet de tank rotatie zoals die in de enetity;
        if (myNavAgent.velocity.normalized.magnitude > 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(myNavAgent.velocity.normalized), rotationSpeed * Time.deltaTime);
        //zodat de turrent niet mee draait met het lichaam
        nonRotatingLink.transform.rotation = startRotation;
    }
}
