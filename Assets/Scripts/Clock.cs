using Unity.XR.GoogleVr;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public const int hoursInDay = 24, minsInHour = 60;

    public float dayDuration = 3000f;

    float totalTime = 0f;
    float currentTime = 0f;


    public Transform minuteHand, hourHand;

    const float hoursToDegrees = 360 /12, minsToDegrees = 360/60;
    void Start()
    {
        currentTime = Random.Range(0, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        currentTime = totalTime % dayDuration;

        hourHand.rotation = Quaternion.Euler(0, 0, -GetHour() * hoursToDegrees);
        minuteHand.rotation = Quaternion.Euler(0, 0, -GetMinute() * minsToDegrees);
    }

    float GetHour()
    {
        return currentTime * hoursInDay / dayDuration;
    }
    float GetMinute()
    {
        return (currentTime * hoursInDay * minsInHour / dayDuration)%minsInHour;
    }

}
