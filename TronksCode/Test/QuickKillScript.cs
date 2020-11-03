using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickKillScript : MonoBehaviour
{
    GameStatusScript GameStatusScript;

    void Start()
    {
        GameStatusScript = GameObject.FindGameObjectWithTag("GameStatusCanvas").GetComponent<GameStatusScript>();
    }
    public void InitiateObjectKill()
    {
        //GameStatusScript.UpdateEnemyCount(1);
        Destroy(gameObject);
    }
}
