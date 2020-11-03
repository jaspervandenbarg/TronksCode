using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    [SerializeField] private GameObject barrelEnd;
    [SerializeField] private int raycastReflections;
    private Vector3 outDirection;
    private Ray ray;
    LayerMask wall = 9;
    void Awake()
    {
        wall = 9;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Cast());
    }

    bool Cast()
    {
        int reflected = raycastReflections;

        ray = new Ray(barrelEnd.transform.position, barrelEnd.transform.forward);
        RaycastHit hit;

        //Debug.Log("==========================================================");

        for(int i = 0; i <= raycastReflections; i++)
        {
            if(reflected == raycastReflections)
            {
                if(Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.layer == wall)
                    {
                        outDirection = Vector3.Reflect(ray.direction, hit.normal);

                        ray = new Ray(hit.point, outDirection);
                    }
                    else if (hit.transform.tag == "PlayerPosition")
                        return true;
                }
            }
            else if(reflected > 0 && reflected < raycastReflections)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.layer == wall)
                    {
                        outDirection = Vector3.Reflect(ray.direction, hit.normal);

                        ray = new Ray(hit.point, outDirection);
                    }
                    else if (hit.transform.name == "PlayerPosition")
                        return true;
                }
            }
        }

        return false;
    }
}
