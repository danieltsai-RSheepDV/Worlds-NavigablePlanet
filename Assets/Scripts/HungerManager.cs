using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HungerDigestionManager : MonoBehaviour
{
    private enum HUNGER_STAGE
    {
        HUNGRY,
        SATISFIED,
        FULL
    }
    private HUNGER_STAGE m_stage;
    private HUNGER_STAGE stage
    {
        get
        {
            return m_stage;
        }
        set
        {
            if (value == m_stage)
                return;

            m_stage = value;
            sliderFillImage.color = hungerColor[(int)m_stage];
            gameObject.GetComponent<PlayerController>().walkSpeed = walkSpeeds[(int)m_stage];
        }
    }
    private Color[] hungerColor = { Color.red, Color.yellow, Color.green };
    private float[] walkSpeeds;

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

            if (m_fullMeter == maxFull)
                stage = HUNGER_STAGE.FULL;
            else
                stage = (HUNGER_STAGE) Mathf.FloorToInt((m_fullMeter / maxFull) * 3);

            hungerSlider.value = m_fullMeter;
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
    Image sliderFillImage;

    private List<FoodNutrition> foodInRange = new List<FoodNutrition>();

    // Start is called before the first frame update
    void Start()
    {
        float curWalkSpeed = gameObject.GetComponent<PlayerController>().walkSpeed;
        walkSpeeds = new float[] { curWalkSpeed / 2, curWalkSpeed * (3f / 4), curWalkSpeed };

        sliderFillImage = hungerSlider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();

        stage = HUNGER_STAGE.FULL;
        fullMeter = maxFull;

        hungerSlider.maxValue = maxFull;
        hungerSlider.minValue = 0;
        hungerSlider.value = fullMeter;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(foodInRange.Count);
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
                int foodToEatIndex = foodInRange.Count - 1;
                FoodNutrition foodToEat = foodInRange[foodToEatIndex];
                foodInRange.RemoveAt(foodToEatIndex);
                fullMeter += foodToEat.GetNutritionPoints();
                StartCoroutine(DigestAndPoop(foodToEat.gameObject));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        FoodNutrition food = other.gameObject.GetComponent<FoodNutrition>();
        if ((food != null) && food.active)
            foodInRange.Add(food);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.name);
        if (other.gameObject.GetComponent<FoodNutrition>())
            foodInRange.Remove(other.gameObject.GetComponent<FoodNutrition>());
    }

    IEnumerator DigestAndPoop(GameObject food)
    {
        food.GetComponent<FoodNutrition>().active = false;
        food.SetActive(false);
        food.transform.parent.gameObject.SetActive(false);

        yield return new WaitForSeconds(5);

        food.transform.parent.position = transform.position;
        food.transform.parent.rotation = transform.rotation;

        food.transform.parent.gameObject.SetActive(true);
        food.SetActive(true);
        food.GetComponent<FoodNutrition>().active = true;
    }
}
