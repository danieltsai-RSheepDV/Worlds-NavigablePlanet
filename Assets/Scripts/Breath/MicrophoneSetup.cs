using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MicrophoneSetup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image circle;

    [SerializeField] private ModelTester breathModel;
    private List<float> min = new List<float>();
    private List<float> max = new List<float>();
    private float timer = 0;
    private int stage = 0;
    
    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        switch (stage)
        {
            case 0:
                if (Input.GetKeyDown("space"))
                {
                    timer = 5;
                    stage = 1;
                    text.text = "Hold.";
                }
                break;
            case 1:
                if (timer > 0 && timer < 4)
                {
                    min.Add(breathModel.getChestSize());
                }else if (timer < 0)
                {
                    stage = 2;
                    PlayerPrefs.SetFloat("MinChestSize", Average(min.ToArray()));
                    text.text = "Calibration done! Breath all the way in. Hold your breath until calibration is done. Press Space to Continue.";
                }
                break;
            case 2:
                if (Input.GetKeyDown("space"))
                {
                    timer = 5;
                    stage = 3;
                    text.text = "Hold.";
                }
                break;
            case 3:
                if (timer > 0 && timer < 4)
                {
                    max.Add(breathModel.getChestSize());
                }else if (timer < 0)
                {
                    stage = 4;
                    PlayerPrefs.SetFloat("MaxChestSize", Average(max.ToArray()));
                    text.text = "Calibration complete!";
                }
                break;
            case 4:
                circle.transform.localScale = 
                    Mathf.Clamp01((breathModel.getChestSize() - PlayerPrefs.GetFloat("MinChestSize")) / (PlayerPrefs.GetFloat("MaxChestSize") - PlayerPrefs.GetFloat("MinChestSize")))
                    * Vector3.one;
                break;
        }
    }

    private float Average(float[] array)
    {
        float sum = 0;
        foreach (float f in array)
        {
            sum += Mathf.Abs(f);
        }
        sum /= array.Length;
        
        return sum;
    }
}
