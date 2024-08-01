using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HungerDigestionManager : MonoBehaviour
{
    private enum HUNGER_STAGE
    {
        FULL,
        SATISFIED,
        HUNGRY,
        STARVING
    }
    private HUNGER_STAGE stage;

    [SerializeField] private float maxFull = 100;

    private float m_fullMeter;
    private float fullMeter
    {
        get
        {
            return m_fullMeter;
        }
        set
        {
            if (value == m_fullMeter)
                return;

            m_fullMeter = value;
            if (value > maxFull)
                m_fullMeter = maxFull;
            else if (value < 0)
                m_fullMeter = 0;

            hungerSlider.value = fullMeter;
        }
    }

    public enum RATE_TYPE
    {
        MOVE,
        JUMP
    }
    [SerializeField] private float moveRate = 2f;
    [SerializeField] private float jumpRate = 5f;

    [SerializeField] Slider hungerSlider;

    private FoodNutrition foodInRange = null;

    // Start is called before the first frame update
    void Start()
    {
        fullMeter = maxFull;

        hungerSlider.maxValue = maxFull;
        hungerSlider.minValue = 0;
        hungerSlider.value = fullMeter;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(fullMeter);
    }

    public void IncreaseHunger(RATE_TYPE type)
    {
        float rate;
        switch (type)
        {
            case RATE_TYPE.MOVE:
                rate = moveRate * Time.deltaTime;
                break;
            case RATE_TYPE.JUMP:
                rate = jumpRate;
                break;
            default:
                rate = 0;
                break;
        }

        fullMeter -= rate;
    }

    private void OnEat(InputValue eatValue)
    {
        if (eatValue.Get<float>() > 0)
        {
            if (foodInRange != null)
            {
                fullMeter += foodInRange.GetNutritionPoints();
                StartCoroutine(DigestAndPoop(foodInRange.gameObject));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FoodNutrition food = other.gameObject.GetComponent<FoodNutrition>();
        if ((food != null) && food.active)
            foodInRange = food;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<FoodNutrition>())
            foodInRange = null;
    }

    IEnumerator DigestAndPoop(GameObject food)
    {
        // TODO: don't poop in midair
        foodInRange = null;
        food.GetComponent<FoodNutrition>().active = false;
        food.SetActive(false);

        yield return new WaitForSeconds(5);

        food.transform.position = transform.position;
        food.transform.rotation = transform.rotation;

        food.SetActive(true);
        food.GetComponent<FoodNutrition>().active = true;
    }
}
