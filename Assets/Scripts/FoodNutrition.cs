using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FoodLogic : MonoBehaviour
{
    [NonSerialized] public bool active = true;
    public float nutritionPoints = 10f;

    public GameObject seed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetNutritionPoints()
    {
        if (active)
            return nutritionPoints;
        return 0;
    }
}
