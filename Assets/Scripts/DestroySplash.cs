using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySplash : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        StartCoroutine(WaitAndDestory());
    }

    IEnumerator WaitAndDestory()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
