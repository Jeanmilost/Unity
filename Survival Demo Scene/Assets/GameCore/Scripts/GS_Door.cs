using UnityEngine;

/**
* This class manages the door object
* @author Jean-Milost Reymond
*/
public class GS_Door : MonoBehaviour
{
    #region Private members

    private GameObject  m_Key;
    private GameObject  m_Message;
    private GS_Key      m_KeyManager;
    private GS_Message  m_MessageManager;
    private AudioSource m_KeyLock;

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

        // get the key lock audio source
        m_KeyLock = GetComponentInChildren<AudioSource>();
        Debug.Assert(m_KeyLock);
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
                m_KeyLock.Play();
            }
            else
            {

            }
        }
    }

    #endregion
}
