﻿using UnityEngine;
using UnityEngine.UI;

/**
* Provides the cast shadows toggle controller
*@author Jean-Milost Reymond
*/
public class CS_CastShadowsToggleController : MonoBehaviour
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
        m_Toggle.onValueChanged.AddListener(delegate{OnToggleValueChanged(m_Toggle);});

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
    void OnToggleValueChanged(Toggle sender)
    {
        m_CurveController.m_CastShadows = m_Toggle.isOn;
    }
}
