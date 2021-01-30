using UnityEngine;

/**
* Provides a fog controller
*@author Jean-Milost Reymond
*/
public class WS_FogController : MonoBehaviour
{
    private Quaternion m_InitialRotation;

    /**
    * Starts the script
    */
    void Start()
    {
        // save the initial fog particles rotation
        m_InitialRotation = transform.rotation;
    }

    /**
    * Post-updates the scene (synchronous, once per frame)
    */
    void LateUpdate()
    {
        // freeze the fog particles rotation
        transform.rotation = m_InitialRotation;
    }
}
