using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Based on Sebastian Lague's First Person Controller: Spherical Worlds tutorial
 * Link: https://www.youtube.com/watch?v=TicipSVT-T8&t=1s
 */

public class GravityAttractor : MonoBehaviour
{
    public float gravity = -10f;

    public void Attract(Transform body)
    {
        Vector3 targetDir = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        body.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.rotation;
        body.gameObject.GetComponent<Rigidbody>().AddForce(targetDir * gravity);
    }
}
