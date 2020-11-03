using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTestScript : MonoBehaviour
{
    public ParticleSystem particle;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * Time.deltaTime * 2, Space.World);
        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * Time.deltaTime * 2, Space.World);
        if (Input.GetKeyDown(KeyCode.Space))
            particle.Play();
    }
}
