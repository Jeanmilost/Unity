using UnityEngine;

/**
* This class manages the level and all the events happening inside it
* @author Jean-Milost Reymond
*/
public class GS_Level : MonoBehaviour
{
    #region Private members

    private GameObject m_Room1;
    private GameObject m_Room2;
    private GameObject m_Room3;
    private GameObject m_Laure;
    private Camera     m_Camera1;
    private Camera     m_Camera2;
    private Camera     m_Camera3;
    private GS_Player  m_Player;
    private bool       m_CursorLocked;

    #endregion

    #region Private functions

    /**
    * Updates the scene (synchronous, once per frame)
    */
    void Start()
    {
        // get the rooms
        m_Room1 = GameObject.Find("Room1");
        m_Room2 = GameObject.Find("Room2");
        m_Room3 = GameObject.Find("Room3");

        Debug.Assert(m_Room1);
        Debug.Assert(m_Room2);
        Debug.Assert(m_Room3);

        // get the cameras
        m_Camera1 = m_Room1.GetComponentInChildren<Camera>();
        m_Camera2 = m_Room2.GetComponentInChildren<Camera>();
        m_Camera3 = m_Room3.GetComponentInChildren<Camera>();

        Debug.Assert(m_Camera1);
        Debug.Assert(m_Camera2);
        Debug.Assert(m_Camera3);

        // update camera status
        m_Camera1.enabled = true;
        m_Camera2.enabled = false;
        m_Camera3.enabled = false;

        // get the player character
        m_Laure = GameObject.Find("Laure");
        Debug.Assert(m_Laure);

        // get the player character script
        m_Player = m_Laure.GetComponentInChildren<GS_Player>();
        Debug.Assert(m_Player);
        m_Player.OnTriggerInside = OnPlayerTriggerInside;

        // lock the cursor
        LockCursor(true);
        m_CursorLocked = true;
    }

    /**
    * Updates the scene (synchronous, once per frame)
    */
    void Update()
    {
        bool oldCursorLocked = m_CursorLocked;

        // esc key pressed or left mouse button clicked?
        if (Input.GetKeyUp(KeyCode.Escape))
            m_CursorLocked = false;
        else
        if (Input.GetMouseButtonUp(0))
            m_CursorLocked = true;

        if (m_CursorLocked != oldCursorLocked)
            LockCursor(m_CursorLocked);
    }

    /**
    * Locks or unlocks the cursor
    *@param value - if true, cursor will be locked, unlocked otherwise
    */
    public void LockCursor(bool value)
    {
        // do lock the cursor?
        if (value)
        {
            // lock and hide the mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        // unlock and show the mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /**
    * Called while the character stays inside a trigger zone
    *@param sender - event sender
    *@param playerController - player controller
    *@param collider - trigger zone collider in which the character is staying
    */
    public void OnPlayerTriggerInside(object sender, CharacterController playerController, Collider collider)
    {
        // search for entered room and activate the matching camera
        if (collider.tag == "Room1")
        {
            // is first camera enabled?
            if (!m_Camera1.enabled)
            {
                // enable it and disable others
                m_Camera1.enabled = true;
                m_Camera2.enabled = false;
                m_Camera3.enabled = false;
            }
        }
        else
        if (collider.tag == "Room2")
        {
            // is second camera enabled?
            if (!m_Camera2.enabled)
            {
                // enable it and disable others
                m_Camera1.enabled = false;
                m_Camera2.enabled = true;
                m_Camera3.enabled = false;
            }
        }
        else
        if (collider.tag == "Room3")
        {
            // is third camera enabled?
            if (!m_Camera3.enabled)
            {
                // enable it and disable others
                m_Camera1.enabled = false;
                m_Camera2.enabled = false;
                m_Camera3.enabled = true;
            }
        }
    }

    #endregion
}
