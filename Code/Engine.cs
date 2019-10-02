using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Gear
{
    public float m_TopSpeed;
    public float m_TimeInGear;

    public float Acc { get; private set; }

    public void CalculateAcc(float prevGearTopSpeed)
    {
        Acc = (m_TopSpeed - prevGearTopSpeed) / m_TimeInGear;
    }
}

public class Engine : MonoBehaviour
{
    [SerializeField]
    private float m_StoppingTimeWithBrake = 2.0f;
    [SerializeField]
    private float m_StoppingTimeIdle = 10.0f;

    private float m_MaxBrakeDcc;
    private float m_IdleDcc;

    [SerializeField]
    private Gear[] m_Gears;

    private int m_CurrGear;

    public float CurrEngineSpeed { get; private set; }
    public float CurrEngineAcc { get; private set; }
    public float TopSpeed { get { return m_Gears[m_Gears.Length - 1].m_TopSpeed; } }

    private void SelectGear()
    {
        while (m_CurrGear < m_Gears.Length - 1 && CurrEngineSpeed > m_Gears[m_CurrGear].m_TopSpeed)
            m_CurrGear++;

        while (m_CurrGear > 0 && CurrEngineSpeed < m_Gears[m_CurrGear - 1].m_TopSpeed)
            m_CurrGear--;
    }

    private void CalculateCurrAcc()
    {
        var acc = GetAccInput();
        var brake = GetBrakeInput();

        if (acc > 0.0f || brake > 0.0f)
        {
            CurrEngineAcc = 0.0f;
            CurrEngineAcc += acc * m_Gears[m_CurrGear].Acc;
            CurrEngineAcc -= brake * m_MaxBrakeDcc;
        }
        else
        {
            CurrEngineAcc = -m_IdleDcc;
        }
    }

    private void CalculateCurrSpeed()
    {
        CurrEngineSpeed += CurrEngineAcc * Time.fixedDeltaTime;
        CurrEngineSpeed = Mathf.Clamp(CurrEngineSpeed, 0.0f, TopSpeed);
    }

    private void InitGears()
    {
        m_Gears[0].CalculateAcc(0.0f);

        for (int i = 1; i < m_Gears.Length; i++)
        {
            var prevTopSpeed = m_Gears[i - 1].m_TopSpeed;

            if (prevTopSpeed >= m_Gears[i].m_TopSpeed)
                Debug.LogWarningFormat("Top speed of gear {0} is greater than top speed of gear {1}", i - 1, i);

            m_Gears[i].CalculateAcc(prevTopSpeed);

            if (m_Gears[i].Acc >= m_Gears[i - 1].Acc)
                Debug.LogWarningFormat("The acc of gear {0} is greater than the acc of gear {1}", i, i - 1);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        CurrEngineAcc = 0.0f;
        CurrEngineSpeed = 0.0f;
        m_CurrGear = 0;

        InitGears();

        m_MaxBrakeDcc = TopSpeed / m_StoppingTimeWithBrake;
        m_IdleDcc = TopSpeed / m_StoppingTimeIdle;
    }

    public void Execute()
    {
        SelectGear();
        CalculateCurrAcc();
        CalculateCurrSpeed();
    }

    protected virtual float GetAccInput()
    {
        return Input.GetMouseButton(0) ? 1.0f : 0.0f;
    }

    protected virtual float GetBrakeInput()
    {
        return Input.GetMouseButton(1) ? 1.0f : 0.0f;
    }
}
