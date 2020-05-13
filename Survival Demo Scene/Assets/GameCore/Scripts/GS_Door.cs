using UnityEngine;

/**
* This class manages the door object
* @author Jean-Milost Reymond
*/
public class GS_Door : MonoBehaviour
{
    #region Private members

    private GameObject   m_Key;
    private GameObject   m_Message;
    private GameObject   m_Interlude;
    private GS_Key       m_KeyManager;
    private GS_Message   m_MessageManager;
    private GS_Interlude m_InterludeManager;
    private AudioSource  m_KeyLock;
    private AudioSource  m_KeyUnlock;
    private AudioSource  m_OpenDoor;
    private bool         m_Unlocked;

    #endregion

    #region Private functions

    /**
    * Starts the script
    */
    void Start()
    {
        // get the key object
        m_Key = GameObject.Find("Key");
        Debug.Assert(m_Key);

        // get the key manager script
        m_KeyManager = m_Key.GetComponentInChildren<GS_Key>();
        Debug.Assert(m_KeyManager);

        // get the message object
        m_Message = GameObject.Find("Message");
        Debug.Assert(m_Message);

        // get the message script
        m_MessageManager = m_Message.GetComponentInChildren<GS_Message>();
        Debug.Assert(m_MessageManager);

        // get the door interlude object
        m_Interlude = GameObject.Find("Interlude");
        Debug.Assert(m_Interlude);

        // get the door interlude script
        m_InterludeManager = m_Interlude.GetComponentInChildren<GS_Interlude>();
        Debug.Assert(m_InterludeManager);

        // get the children audio sources
        Component[] components = GetComponentsInChildren<AudioSource>();

        // there are 2 kind of doors. Closed doors contains 3 sounds, whereas
        // opened doors contains only one sound
        if (components.Length == 1)
        {
            // get the open door audio source
            m_OpenDoor = components[0] as AudioSource;
            Debug.Assert(m_OpenDoor);

            m_Unlocked = true;
        }
        else
        if (components.Length == 3)
        {
            // get the key lock audio source
            m_KeyLock = components[0] as AudioSource;
            Debug.Assert(m_KeyLock);

            // get the key unlock audio source
            m_KeyUnlock = components[1] as AudioSource;
            Debug.Assert(m_KeyUnlock);

            // get the open door audio source
            m_OpenDoor = components[2] as AudioSource;
            Debug.Assert(m_OpenDoor);

            m_Unlocked = false;
        }
        else
            throw new System.Exception("Unknown door type - sounds mismatch - " + components.Length.ToString());
    }

    /**
    * Called while the character stays inside a trigger zone
    *@param other - trigger zone collider in which the character is staying
    */
    void OnTriggerStay(Collider other)
    {
        // player entered the door trigger zone and space key is pressed?
        if (other.name == "Laure" && Input.GetKeyUp(KeyCode.Space))
        {
            // key was picked up?
            if (m_KeyManager == null || !m_KeyManager.IsPickedUp)
            {
                // show the locked message and play the locked sound
                m_MessageManager.Message = "It's locked!";

                if (m_KeyLock)
                    m_KeyLock.Play();
            }
            else
            if (!m_Unlocked)
            {
                // show the unlocked message and play the unlocked sound
                m_MessageManager.Message = "You unlocked the door.";

                if (m_KeyUnlock)
                    m_KeyUnlock.Play();

                m_Unlocked = true;
            }
            else
            if (!m_InterludeManager.IsRunning)
            {
                // go to other room
                if (m_OpenDoor)
                    m_OpenDoor.Play();

                // start the door opening interlude
                m_InterludeManager.Run(tag);
            }
        }
    }

    #endregion
}
