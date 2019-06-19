using UnityEngine;

/**
* This class manages the collectible key object
* @author Jean-Milost Reymond
*/
public class GS_Key : MonoBehaviour
{
    public float m_KeyHighlightVelocity  = 5.0f;
    public float m_KeyHighlightSleepTime = 2.5f;

    private GameObject m_Key;
    private GameObject m_KeyHighLight;
    private Renderer   m_KeyHighLightRenderer;
    private float      m_KeyHighlightAlpha  = 0.0f;
    private float      m_KeyHighlightOffset = 1.0f;
    private float      m_KeyHighlightSleepTimeStamp;
    private bool       m_DoWait;

    public bool IsKept { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Key = GameObject.Find("Key");
        Debug.Assert(m_Key);

        m_KeyHighLight = GameObject.Find("Key_Highlight");
        Debug.Assert(m_KeyHighLight);

        m_KeyHighLightRenderer = m_KeyHighLight.GetComponent<Renderer>();
        Debug.Assert(m_KeyHighLightRenderer);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_DoWait)
        {
            m_KeyHighlightAlpha += (m_KeyHighlightVelocity * Time.deltaTime * m_KeyHighlightOffset);

            if (m_KeyHighlightAlpha <= 0.0f)
            {
                m_KeyHighlightAlpha          = 0.0f;
                m_KeyHighlightSleepTimeStamp = Time.time;
                m_DoWait                     = true;
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
        {
            m_KeyHighlightOffset = -m_KeyHighlightOffset;
            m_DoWait             =  false;
        }

        m_KeyHighLightRenderer.material.color = new Color(m_KeyHighLightRenderer.material.color.r,
                                                          m_KeyHighLightRenderer.material.color.g,
                                                          m_KeyHighLightRenderer.material.color.b,
                                                          m_KeyHighlightAlpha);
    }

    /**
    * Called while the character stays inside a trigger zone
    *@param other - trigger zone collider in which the character is staying
    */
    void OnTriggerStay(Collider other)
    {
        if (other.name == "Laure" && Input.GetKeyUp(KeyCode.Space))
        {
            m_Key.SetActive(false);
            IsKept = true;
        }
    }
}
