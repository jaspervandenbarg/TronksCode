using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S2
{
    public class AIMaster : MonoBehaviour
    {
        public Transform myTarget;

        public delegate void GeneralEventHandler();
        public event GeneralEventHandler EventEnemyDie;
        public event GeneralEventHandler EventEnemyWalking;
        public event GeneralEventHandler EventReachedNavTarget;
        public event GeneralEventHandler EventEnemyAttack;
        public event GeneralEventHandler EventEnemyLostTarget;

        public delegate void HealthEventHandler(int health);
        public event HealthEventHandler EventEnemyDeductHealth;
    }
}


