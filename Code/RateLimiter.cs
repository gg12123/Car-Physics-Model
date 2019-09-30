using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RateLimiter
{
    public float CurrentValue { get; set; }

    private float m_MaxRate;

    public RateLimiter(float maxChangeRate)
    {
        m_MaxRate = maxChangeRate;
    }

    public void Execute(float target, float dt)
    {
        if (CanReachTarget(target, dt))
        {
            CurrentValue = target;
            return;
        }

        CurrentValue += Mathf.Sign(target - CurrentValue) * m_MaxRate * dt;
    }

    public bool CanReachTarget(float target, float dt)
    {
        var reqRate = (target - CurrentValue) / dt;
        return Mathf.Abs(reqRate) <= m_MaxRate;
    }
}
