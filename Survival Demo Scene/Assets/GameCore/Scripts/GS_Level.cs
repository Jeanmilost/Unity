using UnityEngine;

/**
* This class manages the level and all the events happening inside it
* @author Jean-Milost Reymond
*/
public class GS_Level : MonoBehaviour
{
    #region Private members

    private GameObject   m_Room1;
    private GameObject   m_Room2;
    private GameObject   m_Room3;
    private GameObject   m_Room4;
    private GameObject   m_Player;
    private GameObject   m_Zombie;
    private GameObject   m_Interlude;
    private Camera       m_Camera1;
    private Camera       m_Camera2;
    private Camera       m_Camera3;
    private Camera       m_Camera4;
    private Camera       m_Camera5;
    private GS_Player    m_PlayerManager;
    private GS_Zombie    m_ZombieManager;
    private GS_Interlude m_InterludeManager;
    private bool         m_CursorLocked;

    #endregion

    #region Public functions

    /**
    * Called when the player is opening a door
    *@param sender - event sender
    *@param sourceTag - the source room where the player was when the door was opened
    */
    public void OnPlayerOpeningDoor(object sender, string sourceTag)
    {
        // is player coming from room 4?
        if (sourceTag == "Room4")
            // notify the zombie to be idle
            m_ZombieManager.MachineState = GS_Zombie.IEMachineState.IE_MS_Idle;
    }

    /**
    * Called when the player entered inside a new room
    *@param sender - event sender
    *@param sourceTag - the source room where the player was when the door was opened
    */
    public void OnPlayerEnteredNewRoom(object sender, string sourceTag)
    {
        // is player coming from corridor?
        if (sourceTag == "Room3")
        {
            // put the player in room 4 and notify the zombie to begin to chase
            m_PlayerManager.SetPlayerPosAndDir(new Vector3(2.8f, 0.075f, -15.0f), new Vector3(0.0f, 0.0f, 0.0f));
            m_ZombieManager.MachineState = GS_Zombie.IEMachineState.IE_MS_Chasing;
        }
        else
            // put back the player in corridor
            m_PlayerManager.SetPlayerPosAndDir(new Vector3(-11.75f, 0.075f, -12.4f), new Vector3(0.0f, 180.0f, 0.0f));
    }

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
        m_Room4 = GameObject.Find("Room4");

        Debug.Assert(m_Room1);
        Debug.Assert(m_Room2);
        Debug.Assert(m_Room3);
        Debug.Assert(m_Room4);

        // get the interlude scene
        m_Interlude = GameObject.Find("Interlude");
        Debug.Assert(m_Interlude);

        // get the door interlude script
        m_InterludeManager = m_Interlude.GetComponentInChildren<GS_Interlude>();
        Debug.Assert(m_InterludeManager);
        m_InterludeManager.OnPlayerOpeningDoor    = OnPlayerOpeningDoor;
        m_InterludeManager.OnPlayerEnteredNewRoom = OnPlayerEnteredNewRoom;

        // get the cameras
        m_Camera1 = m_Room1.GetComponentInChildren<Camera>();
        m_Camera2 = m_Room2.GetComponentInChildren<Camera>();
        m_Camera3 = m_Room3.GetComponentInChildren<Camera>();
        m_Camera4 = m_Room4.GetComponentInChildren<Camera>();
        m_Camera5 = m_Interlude.GetComponentInChildren<Camera>();

        Debug.Assert(m_Camera1);
        Debug.Assert(m_Camera2);
        Debug.Assert(m_Camera3);
        Debug.Assert(m_Camera4);
        Debug.Assert(m_Camera5);

        // update camera status
        m_Camera1.enabled = true;
        m_Camera2.enabled = false;
        m_Camera3.enabled = false;
        m_Camera4.enabled = false;
        m_Camera5.enabled = false;

        // get the player character
        m_Player = GameObject.Find("Laure");
        Debug.Assert(m_Player);

        // get the player character script
        m_PlayerManager = m_Player.GetComponentInChildren<GS_Player>();
        Debug.Assert(m_PlayerManager);
        m_PlayerManager.OnTriggerInside = OnPlayerTriggerInside;

        // get the zombie character
        m_Zombie = GameObject.Find("Zombie");
        Debug.Assert(m_Zombie);

        // get the zombie character script
        m_ZombieManager = m_Zombie.GetComponentInChildren<GS_Zombie>();
        Debug.Assert(m_ZombieManager);

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
        // disable all the camera if interlude is running
        if (m_InterludeManager.IsRunning)
        {
            m_Camera1.enabled = false;
            m_Camera2.enabled = false;
            m_Camera3.enabled = false;
            m_Camera4.enabled = false;
            m_Camera5.enabled = true;
            return;
        }

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
                m_Camera4.enabled = false;
                m_Camera5.enabled = false;
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
                m_Camera4.enabled = false;
                m_Camera5.enabled = false;
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
                m_Camera4.enabled = false;
                m_Camera5.enabled = false;
            }
        }
        else
        if (collider.tag == "Room4")
        {
            m_Camera1.enabled = false;
            m_Camera2.enabled = false;
            m_Camera3.enabled = false;
            m_Camera4.enabled = true;
            m_Camera5.enabled = false;
        }
    }

    #endregion
}
