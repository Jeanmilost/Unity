using UnityEngine;
using UnityEngine.UI;

/**
* This class manages the user messages
* @author Jean-Milost Reymond
*/
public class GS_Message : MonoBehaviour
{
    #region Public members

    public Text  m_Text;
    public float m_MsgVisibleTime = 5.0f;

    #endregion

    #region Private members

    private string m_Message;
    private float  m_MsgVisibleTimeStamp;

    #endregion

    #region Public properties

    /**
    * Gets or sets the user message
    */
    public string Message
    { 
        get
        {
            return m_Text.text;
        }

        set
        {
            // show the message on the user interface
            m_Text.text           = value;
            m_MsgVisibleTimeStamp = Time.time;
            m_Text.gameObject.SetActive(true);
        }
    }

    #endregion

    #region Private functions

    /**
    * Starts the script
    */
    void Start()
    {
        // get the text displayer
        Debug.Assert(m_Text);
        m_Text.gameObject.SetActive(false);
    }

    /**
    * Updates the scene (synchronous, once per frame)
    */
    void Update()
    {
        // show the message until the message visible time is elapsed
        if (Time.time - m_MsgVisibleTimeStamp > m_MsgVisibleTime)
            m_Text.gameObject.SetActive(false);
    }

    #endregion
}
