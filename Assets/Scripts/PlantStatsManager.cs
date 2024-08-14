using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantStatsManager : MonoBehaviour
{
    private float m_sunlightScore;
    [SerializeField] private float sunlightMax = 50f;
    [SerializeField] private float sunlightIncreaseFactor = 5f;
    private float sunlightScore
    {
        get
        {
            return m_sunlightScore;
        }
        set
        {
            if (value == m_sunlightScore)
                return;

            m_sunlightScore = value;
            if (value > sunlightMax)
                m_sunlightScore = sunlightMax;
            else if (value < 0)
                m_sunlightScore = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.collider.gameObject.name == "DayCollider")
            {
                sunlightScore += Time.fixedDeltaTime * sunlightIncreaseFactor;
                Debug.Log(sunlightScore);
            }
        }
    }
}
