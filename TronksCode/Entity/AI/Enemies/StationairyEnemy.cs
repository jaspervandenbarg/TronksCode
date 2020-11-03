using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationairyEnemy : EnemyAI
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
        if (entity.FieldOfView.playerVisible && canLookAt)
            LookAtTarget(turrent, playerTarget.transform.position, turrentRotationSpeed);
        else
        {
            LookInGeneralDirection(playerTarget.transform, searchingAngle);
            LookForTarget(turrent, turrentRotationSpeed, rotationDirection);
        }
    }
}
