using UnityEngine;
using UnityEngine.UI;

/**
* Provides the allow hit toggle controller
*@author Jean-Milost Reymond
*/
public class CS_AllowHitToggleController : MonoBehaviour
{
    private Toggle             m_Toggle;
    private GameObject         m_Curve;
    private CS_CurveController m_CurveController;

    /**
    * Starts the script
    */
    void Start()
    {
        // get the toggle component
        m_Toggle = GetComponent<Toggle>();
        Debug.Assert(m_Toggle);

        // add listener to know when the Toggle value changes
        m_Toggle.onValueChanged.AddListener(delegate { OnFadeOutToggleValueChanged(m_Toggle); });

        // get the curve object
        m_Curve = GameObject.Find("Curve");
        Debug.Assert(m_Curve);

        // get the curve script
        m_CurveController = m_Curve.GetComponentInChildren<CS_CurveController>();
        Debug.Assert(m_CurveController);
    }

    /**
    * Called when the toggle value changed
    *@param sender - event sender
    */
    void OnFadeOutToggleValueChanged(Toggle sender)
    {
        m_CurveController.m_AllowHit = m_Toggle.isOn;
    }
}
