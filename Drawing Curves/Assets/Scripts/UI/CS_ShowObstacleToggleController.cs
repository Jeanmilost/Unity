using UnityEngine;
using UnityEngine.UI;

/**
* Provides the show obstacle toggle controller
*@author Jean-Milost Reymond
*/
public class CS_ShowObstacleToggleController : MonoBehaviour
{
    private Toggle     m_Toggle;
    private GameObject m_Obstacle;

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

        // get the obstacle object
        m_Obstacle = GameObject.Find("Obstacle");
        Debug.Assert(m_Obstacle);
        m_Obstacle.SetActive(m_Toggle.isOn);
    }

    /**
    * Called when the toggle value changed
    *@param sender - event sender
    */
    void OnFadeOutToggleValueChanged(Toggle sender)
    {
        m_Obstacle.SetActive(m_Toggle.isOn);
    }
}
