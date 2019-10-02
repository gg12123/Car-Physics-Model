using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrictionApplier : MonoBehaviour
{
    [SerializeField]
    private float m_MaxAccForward = 1.0f;

    [SerializeField]
    private float m_MaxAccSide = 1.0f;

    private Rigidbody m_Body;
    private Engine m_Engine;

    // Start is called before the first frame update
    void Awake()
    {
        m_Body = GetComponent<Rigidbody>();
        m_Engine = GetComponent<Engine>();
    }

    private float CalculateSecondMaxAccForward(float vf, float vs, float A)
    {
        var discrim = (2.0f * vf) * (2.0f * vf) - 4.0f * (vs * vs + vf * vf - A * A);

        if (discrim < 0.0f)
            return 0.0f;

        var deltaV = (-2.0f * vf + Mathf.Sqrt(discrim)) / 2.0f;
        return deltaV < 0.0f ? 0.0f : deltaV / Time.fixedDeltaTime;
    }

    // Update is called once per frame
    public void Execute()
    {
        var f = m_Body.rotation * Vector3.forward;
        var s = m_Body.rotation * Vector3.right;

        var vs = Vector3.Dot(s, m_Body.velocity);
        var vf = Vector3.Dot(f, m_Body.velocity);

        var as_req = -vs / Time.fixedDeltaTime;
        var af_req = (m_Engine.CurrEngineSpeed - vf) / Time.fixedDeltaTime;

        var as_actual = Mathf.Sign(as_req) * Mathf.Min(Mathf.Abs(as_req), m_MaxAccSide);
        var vs_after_as = vs + as_actual * Time.fixedDeltaTime;

        var af_actual = af_req > 0.0f ?
            Mathf.Min(af_req, m_MaxAccForward, CalculateSecondMaxAccForward(vf, vs_after_as, m_Engine.CurrEngineSpeed)) :
            Mathf.Max(af_req, -m_MaxAccForward);

        m_Body.AddForce(m_Body.mass * as_actual * s);
        m_Body.AddForce(m_Body.mass * af_actual * f);
    }
}
