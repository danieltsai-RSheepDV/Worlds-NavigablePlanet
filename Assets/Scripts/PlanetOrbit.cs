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
        transform.Rotate(new Vector3(0, circumferenceDivSpeed * Time.deltaTime, 0));
    }
}
