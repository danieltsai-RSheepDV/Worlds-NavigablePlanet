using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    [SerializeField] private Vector3 orbitCenter = Vector3.zero;
    [SerializeField] private float orbitRadius;
    [SerializeField] private float orbitTime;

    private float circumferenceDivSpeed;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.position = orbitCenter;
        foreach (Transform child in transform)
        {
            child.localPosition = new Vector3(0, 0, orbitRadius);
        }
        circumferenceDivSpeed = 360 / orbitTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // rotate around sun
        transform.Rotate(new Vector3(0, circumferenceDivSpeed * Time.deltaTime, 0));

        // rotate planet around its own axis
        foreach (Transform child in transform)
        {
            if (child.tag != "SpinOwnAxis")
                continue;
            Vector3 translate = child.TransformPoint(Vector3.zero);
            child.Translate(-translate, Space.World);
            child.Rotate(new Vector3(0, circumferenceDivSpeed * Time.deltaTime, 0));
            child.Translate(translate, Space.World);
        }
    }
}
