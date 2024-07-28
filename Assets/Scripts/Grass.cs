using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public Material[] materials;
    public Transform player;
    Vector3 position;
    Vector3 headPosition;

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
            position = player.transform.position + player.transform.up;
            headPosition = player.transform.position + (player.transform.up * 2.5f);
            int length = materials.Length;
            for (int i = 0; i < length; i++)
            {
                materials[i].SetVector("_PlayerPosition", position);
                materials[i].SetVector("_PlayerHeadPosition", headPosition);
            }

            yield return null;
        }
    }
}
