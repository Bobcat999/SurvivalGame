using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TimeManager : MonoBehaviour
{
    [SerializeField] Light2D globalLighting;

    public static TimeManager instance;

    //time 
    public float worldTime = 0f;

    public const float DAY_TIME = 5f * 60f;//number of minutes times 60 seconds
    public const float NIGHT_TIME = 3f * 60f;
    public const float DAY_NIGHT_CYCLE = DAY_TIME + NIGHT_TIME;
    public const float MIN_INTENSITY = .01f;
    public const float MAX_INTENSITY = .6f;
    //allways day before night

    [SerializeField] Gradient dayNightGradient;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        //add the deltaTime to the worldTime
        worldTime += Time.deltaTime;


        // Calculate the current time of day
        float currentTime = Mathf.Repeat(worldTime, DAY_NIGHT_CYCLE);

        float intensity = MIN_INTENSITY;

        //smooth out between day and night
        if (currentTime <= DAY_TIME)
        {
            intensity = Mathf.Lerp(MIN_INTENSITY, MAX_INTENSITY, dayNightGradient.Evaluate(currentTime / DAY_TIME).a);
        }

        globalLighting.intensity = intensity;

    }

    public bool IsDay()
    {
        return Mathf.Repeat(worldTime, DAY_NIGHT_CYCLE) <= DAY_TIME;
    }
}
