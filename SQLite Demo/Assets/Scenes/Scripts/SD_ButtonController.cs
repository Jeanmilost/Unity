using UnityEngine;
using UnityEngine.UI;

/**
* Provides a button controller
*@author Jean-Milost Reymond
*/
public class SD_ButtonController : MonoBehaviour
{
    /**
    * Called when the button is clicked
    *@param sender - the event sender
    */
    public delegate void OnButtonClickedEvent(object sender);

    private Button m_Button;

    /**
    * Gets or sets the OnButtonClicked event
    */
    public OnButtonClickedEvent OnButtonClicked { get; set; }

    /**
    * Starts the script
    */
    void Start()
    {
        // get the ball object
        m_Button = GetComponent<Button>();
        Debug.Assert(m_Button);
    }

    /**
    * Called when the button is clicked
    */
    public void OnClick()
    {
        // notify that button was clicked
        OnButtonClicked?.Invoke(this);
    }
}
