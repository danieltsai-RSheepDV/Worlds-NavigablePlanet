using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    [SerializeField] private float orbitRadius;
    [SerializeField] private float orbitTime;

    private float circumferenceDivSpeed;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.GetChild(0).localPosition = new Vector3(0, 0, orbitRadius);
        circumferenceDivSpeed = 360 / orbitTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // rotate around sun
        transform.Rotate(new Vector3(0, circumferenceDivSpeed * Time.deltaTime, 0));

        // rotate around its own axis
        Transform child = transform.GetChild(0);
        Vector3 translate = child.TransformPoint(Vector3.zero);
        child.Translate(-translate, Space.World);
        child.Rotate(new Vector3(0, circumferenceDivSpeed * Time.deltaTime, 0));
        child.Translate(translate, Space.World);
    }
}
