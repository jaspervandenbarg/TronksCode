using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshWake : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;
    // Start is called before the first frame update
    void Start()
    {
        navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
        NeverUnload.Instance.navMeshSurface = navMeshSurface;
        navMeshSurface.BuildNavMesh();
    }
}
