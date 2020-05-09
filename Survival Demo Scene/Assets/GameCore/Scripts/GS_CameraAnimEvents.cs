using UnityEngine;

/**
* This class manages the opening door interlude events
* @author Jean-Milost Reymond
*/
public class GS_CameraAnimEvents : MonoBehaviour
{
    #region Private members
    
    private GameObject   m_InterludeScene;
    private GS_Interlude m_Interlude;

    #endregion

    #region Public functions

    /**
    * Called when walking animation is starting
    */
    public void StartWalk()
    {
        m_Interlude.OnStartWalk();
    }

    /**
    * Called when door is reached
    */
    public void DoorReached()
    {
        m_Interlude.OnDoorReached();
    }

    /**
    * Called when door is opened
    */
    public void DoorOpened()
    {
        m_Interlude.OnDoorOpened();
    }

    /**
    * Called when walking animation is stopped
    */
    public void StopWalk()
    {
        m_Interlude.OnStopWalk();
    }

    #endregion

    #region Private functions

    /**
    * Starts the script
    */
    void Start()
    {
        // get the door interlude object
        m_InterludeScene = GameObject.Find("Interlude");
        Debug.Assert(m_InterludeScene);

        // get the door interlude script
        m_Interlude = m_InterludeScene.GetComponentInChildren<GS_Interlude>();
        Debug.Assert(m_Interlude);
    }

    #endregion
}
