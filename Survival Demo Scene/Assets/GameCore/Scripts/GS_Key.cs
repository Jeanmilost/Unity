using UnityEngine;

/**
* This class manages the collectible key object
* @author Jean-Milost Reymond
*/
public class GS_Key : MonoBehaviour
{
    #region Public members

    public float m_KeyHighlightVelocity  = 5.0f;
    public float m_KeyHighlightSleepTime = 2.5f;

    #endregion

    #region Private members

    private GameObject  m_Key;
    private GameObject  m_KeyHighLight;
    private GameObject  m_Message;
    private GS_Message  m_MessageManager;
    private Renderer    m_KeyHighLightRenderer;
    private AudioSource m_PickUp;
    private float       m_KeyHighlightAlpha  = 0.0f;
    private float       m_KeyHighlightOffset = 1.0f;
    private float       m_KeyHighlightSleepTimeStamp;
    private float       m_MsgVisibleTimeStamp;
    private bool        m_DoWait;

    #endregion

    #region Public properties

    /**
    * Gets or sets if the key was picked up
    */
    public bool IsPickedUp { get; set; } = false;

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

        // get the key highlight object
        m_KeyHighLight = GameObject.Find("Key_Highlight");
        Debug.Assert(m_KeyHighLight);

        // get the key highlight renderer
        m_KeyHighLightRenderer = m_KeyHighLight.GetComponent<Renderer>();
        Debug.Assert(m_KeyHighLightRenderer);

        // get the message object
        m_Message = GameObject.Find("Message");
        Debug.Assert(m_Message);

        // get the message script
        m_MessageManager = m_Message.GetComponentInChildren<GS_Message>();
        Debug.Assert(m_MessageManager);

        // get the pick up audio source
        m_PickUp = GetComponentInChildren<AudioSource>();
        Debug.Assert(m_PickUp);
    }

    /**
    * Updates the scene (synchronous, once per frame)
    */
    void Update()
    {
        // do wait before running a new highlight flicker?
        if (!m_DoWait)
        {
            // calculate next highlight alpha value
            m_KeyHighlightAlpha += (m_KeyHighlightVelocity * Time.deltaTime * m_KeyHighlightOffset);

            // test alpha limits, invert the offset and/or start to wait if a limit is reached
            if (m_KeyHighlightAlpha <= 0.0f)
            {
                m_KeyHighlightAlpha          =  0.0f;
                m_KeyHighlightOffset         = -m_KeyHighlightOffset;
                m_KeyHighlightSleepTimeStamp =  Time.time;
                m_DoWait                     =  true;
            }
            else
            if (m_KeyHighlightAlpha >= 1.0f)
            {
                m_KeyHighlightAlpha  =  1.0f;
                m_KeyHighlightOffset = -m_KeyHighlightOffset;
            }
        }
        else
        if (Time.time - m_KeyHighlightSleepTimeStamp >= m_KeyHighlightSleepTime)
            // wait time elapsed, start a new cycle
            m_DoWait = false;

        // change highlight flicker alpha value
        m_KeyHighLightRenderer.material.color = new Color(m_KeyHighLightRenderer.material.color.r,
                                                          m_KeyHighLightRenderer.material.color.g,
                                                          m_KeyHighLightRenderer.material.color.b,
                                                          m_KeyHighlightAlpha);

        // disable the key only after pick up sound was played, otherwise sound will never be played
        if (IsPickedUp && !m_PickUp.isPlaying)
            m_Key.SetActive(false);
    }

    /**
    * Called while the character stays inside a trigger zone
    *@param other - trigger zone collider in which the character is staying
    */
    void OnTriggerStay(Collider other)
    {
        // key may still be picked up, player entered the key trigger zone and space key is pressed?
        if (!IsPickedUp && other.name == "Laure" && Input.GetKeyUp(KeyCode.Space))
        {
            // set the key as picked up
            IsPickedUp = true;

            // show the found key message and play the pick up sound
            m_MessageManager.Message = "You found the laboratory key!";
            m_PickUp.Play();
        }
    }

    #endregion
}
