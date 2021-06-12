using UnityEngine;

/**
* Provides a main application controller. Contains the application common tasks like e.g the cursor management
*@author Jean-Milost Reymond
*/
public class WS_MainController : MonoBehaviour
{
    bool m_CursorLocked;

    /**
    * Starts the script
    */
    void Start()
    {
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
}
