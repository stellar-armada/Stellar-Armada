using System.Collections.Generic;
using System.Linq;
using SpaceCommander.Game;
using UnityEngine;

public class Timer
{
    private float Length;
    public float currentTime;
    public GameManager.EventHandler OnTimerFinished;
    private SortedDictionary<float, GameManager.EventHandler> Triggers;
    private int currentTrigger;
    private bool running;

    public Timer Start()
    {
        currentTime = 0;
        currentTrigger = 0;
        running = true;
        return this;
    }

    public void SetTrigger(float _t, GameManager.EventHandler _e)
    {
        Triggers.Add(_t, _e);
    }

    public Timer(MatchClock clock, float length)
    {
        clock.OnUpdate += Update;
        Triggers = new SortedDictionary<float, GameManager.EventHandler>();

        Length = length;
    }

    private int lastInt;

    public void Update()
    {
        if (!running) return;
        if (Input.GetKeyDown(KeyCode.N))
            currentTime += 10;
        
        currentTime += Time.deltaTime;
        if (Mathf.FloorToInt(currentTime) != lastInt)
        {
            lastInt = Mathf.FloorToInt(currentTime);
           // Debug.Log(currentTime);
        }

        var tList = Triggers.Keys.ToList();
        if (currentTrigger < tList.Count)
        {
            float tTime = tList[currentTrigger];
            if (currentTime > tTime)
            {
                currentTrigger++;
                Triggers[tTime]?.Invoke();
            }
        }

        if (currentTime > Length)
        {
            OnTimerFinished?.Invoke();
            running = false;
        }
    }
}