using UnityEngine;
using UnityEngine.UI;

/**
* Provides the point count edit controller
*@author Jean-Milost Reymond
*/
public class CS_PointCountEditController : MonoBehaviour
{
    private InputField         m_Edit;
    private GameObject         m_Curve;
    private CS_CurveController m_CurveController;

    /**
    * Starts the script
    */
    void Start()
    {
        // get the toggle component
        m_Edit = GetComponent<InputField>();
        Debug.Assert(m_Edit);

        // add listener to know when the Toggle value changes
        m_Edit.onEndEdit.AddListener(delegate{OnEndEdit(m_Edit);});

        // get the curve object
        m_Curve = GameObject.Find("Curve");
        Debug.Assert(m_Curve);

        // get the curve script
        m_CurveController = m_Curve.GetComponentInChildren<CS_CurveController>();
        Debug.Assert(m_CurveController);
    }

    /**
    * Called when the value edition ends on the edit control
    *@param sender - event sender
    */
    void OnEndEdit(InputField sender)
    {
        m_CurveController.m_Points = int.Parse(m_Edit.text);
    }
}
