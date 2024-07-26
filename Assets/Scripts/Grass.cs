using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public Material[] materials;
    public Transform player;
    Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("writeToMaterial");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator writeToMaterial()
    {
        while (true)
        {
            position = player.transform.position;
            int length = materials.Length;
            for (int i = 0; i < length; i++)
            {
                materials[i].SetVector("_PlayerPosition", position);
            }

            yield return null;
        }
    }
}
