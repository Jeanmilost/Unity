using UnityEngine;
using UnityEngine.UI;

public class CS_FadeOutToggleController : MonoBehaviour
{
    private Toggle m_Toggle;
    private GameObject m_Curve;
    private CS_CurveController m_CurveController;

    void Start()
    {
        //Fetch the Toggle GameObject
        m_Toggle = GetComponent<Toggle>();

        //Add listener for when the state of the Toggle changes, to take action
        m_Toggle.onValueChanged.AddListener(delegate{OnFadeOutToggleValueChanged(m_Toggle);});

        // get the curve object
        m_Curve = GameObject.Find("Curve");
        Debug.Assert(m_Curve);

        // get the curve script
        m_CurveController = m_Curve.GetComponentInChildren<CS_CurveController>();
        Debug.Assert(m_CurveController);
    }

    //Output the new state of the Toggle into Text
    void OnFadeOutToggleValueChanged(Toggle change)
    {
        m_CurveController.m_FadeOut = m_Toggle.isOn;
    }
}
