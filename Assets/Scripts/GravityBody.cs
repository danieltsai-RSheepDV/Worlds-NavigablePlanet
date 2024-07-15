using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Based on Sebastian Lague's First Person Controller: Spherical Worlds tutorial
 * Link: https://www.youtube.com/watch?v=TicipSVT-T8&t=1s
 */

[RequireComponent (typeof (Rigidbody))]
public class GravityBody : MonoBehaviour
{
    GravityAttractor planet;

    void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        planet.Attract(transform);
    }
}
