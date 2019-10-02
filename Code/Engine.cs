using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Gear
{
    public float m_TopSpeed;
    public float m_Acc;
}

public class Engine : MonoBehaviour
{
    [SerializeField]
    private float m_MaxBrakeDcc = 10.0f;

    [SerializeField]
    private float m_IdleDcc = 2.0f;

    [SerializeField]
    private Gear[] m_Gears;

    private int m_CurrGear;

    public float CurrEngineSpeed { get; private set; }
    public float CurrEngineAcc { get; private set; }
    public float TopSpeed { get { return m_Gears[m_Gears.Length - 1].m_TopSpeed; } }

    private void SelectGear()
    {
        while (CurrEngineSpeed > m_Gears[m_CurrGear].m_TopSpeed && m_CurrGear < m_Gears.Length)
            m_CurrGear++;

        while (CurrEngineSpeed < m_Gears[m_CurrGear - 1].m_TopSpeed && m_CurrGear > 0)
            m_CurrGear--;
    }

    private void CalculateCurrAcc()
    {
        var acc = GetAccInput();
        var brake = GetBrakeInput();

        if (acc > 0.0f || brake > 0.0f)
        {
            CurrEngineAcc = 0.0f;
            CurrEngineAcc += acc * m_Gears[m_CurrGear].m_Acc;
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

    // Start is called before the first frame update
    void Awake()
    {
        CurrEngineAcc = 0.0f;
        CurrEngineSpeed = 0.0f;
        m_CurrGear = 0;
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
