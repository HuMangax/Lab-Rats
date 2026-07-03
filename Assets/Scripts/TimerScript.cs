using UnityEngine;
using TMPro;

public class LevelTimerScript : MonoBehaviour
{
    public TextMeshProUGUI timer;
    private float time;

    void Update()
    {
        time += Time.deltaTime;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
