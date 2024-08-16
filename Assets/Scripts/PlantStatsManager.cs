using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantStatsManager : MonoBehaviour
{
    private float rayMaxDistance = 200f;
    private LayerMask raycastMask;

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

    private float m_waterScore;
    [SerializeField] private float waterMax = 50f;
    [SerializeField] private float waterIncreaseFactor = 5f;
    private float waterScore
    {
        get
        {
            return m_waterScore;
        }
        set
        {
            if (value == m_waterScore)
                return;

            m_waterScore = value;
            if (value > waterMax)
                m_waterScore = waterMax;
            else if (value < 0)
                m_waterScore = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        raycastMask = LayerMask.GetMask("DetectRaycast");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), rayMaxDistance, raycastMask);
        foreach (RaycastHit hit in hits)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            string name = hit.collider.gameObject.name;
            if (name == "DayCollider")
            {
                sunlightScore += Time.fixedDeltaTime * sunlightIncreaseFactor;
                Debug.Log("Sunlight: " + sunlightScore);
            }
            else if (name == "Rain")
            {
                waterScore += Time.fixedDeltaTime * waterIncreaseFactor;
                Debug.Log("Water: " + waterScore);
            }
        }
    }
}
