using System.Collections.Generic;
using System.Linq;
using StellarArmada.Match;
using UnityEngine;

public class MatchTimer
{
    // The actual timer object that gets created and ticked down by the Clock
    // TO-DO: Integrate
    private float Length;
    public float currentTime;

    public delegate void EventHandler();
    public EventHandler OnTimerFinished;
    private SortedDictionary<float, EventHandler> Triggers;
    private int currentTrigger;
    private bool running;

    public MatchTimer Start()
    {
        currentTime = 0;
        currentTrigger = 0;
        running = true;
        return this;
    }

    public void SetTrigger(float _t, EventHandler _e)
    {
        Triggers.Add(_t, _e);
    }

    public MatchTimer(MatchClock clock, float length)
    {
        clock.OnUpdate.AddListener(Update);
        Triggers = new SortedDictionary<float, EventHandler>();

        Length = length;
    }

    private int lastInt;

    void Update()
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