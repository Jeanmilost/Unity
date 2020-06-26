using UnityEngine;

/**
* Provides the main controller
*@author Jean-Milost Reymond
*/
public class CS_MainController : MonoBehaviour
{
    private GameObject         m_Curve;
    private CS_CurveController m_CurveController;

    /**
    * Starts the script
    */
    void Start()
    {
        // get the curve object
        m_Curve = GameObject.Find("Curve");
        Debug.Assert(m_Curve);

        // get the curve script
        m_CurveController = m_Curve.GetComponentInChildren<CS_CurveController>();
        Debug.Assert(m_CurveController);

        m_CurveController.OnDoIgnoreHit = OnDoIgnoreHit;
    }

    /**
    * Called to check if a hit between the curve and the hit object is allowed
    *@param sender - event sender
    *@param hitCollider - the curve hit collider
    *@return true if the hit should be ignored, otherwise false
    */
    public bool OnDoIgnoreHit(object sender, Collider hitCollider)
    {
        return (hitCollider.gameObject.name == "StartSphere" ||
                hitCollider.gameObject.name == "EndSphere"   ||
                hitCollider.gameObject.name == "ControlSphere");
    }
}
