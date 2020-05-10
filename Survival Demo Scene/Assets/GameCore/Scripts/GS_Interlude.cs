using UnityEngine;

/**
* This class manages the opening door interlude
* @author Jean-Milost Reymond
*/
public class GS_Interlude : MonoBehaviour
{
    #region Public delegates

    /**
    * Called when the player is opening a door
    *@param sender - event sender
    *@param sourceTag - the source room where the player was when the door was opened
    */
    public delegate void OnPlayerOpeningDoorEvent(object sender, string sourceTag);

    /**
    * Called when the player entered inside a new room
    *@param sender - event sender
    *@param sourceTag - the tag of the room where the player was when the door was opened
    */
    public delegate void OnPlayerEnteredNewRoomEvent(object sender, string sourceTag);

    #endregion

    #region Private members

    private GameObject          m_InterludeScene;
    private GameObject          m_Door;
    private Camera              m_Camera;
    private Animator            m_DoorAnimator;
    private Animator            m_CameraAnimator;
    private AudioSource         m_DoorOpening;
    private AudioSource         m_FootSteps;
    private GS_CameraAnimEvents m_CameraAnimEvents;
    private string              m_SourceTag;
    private bool                m_IsRunning;

    #endregion

    #region Public properties

    /**
    * Gets if the interlude is running
    */
    public bool IsRunning
    {
        get
        {
            return m_IsRunning;
        }
    }

    /**
    * Gets or sets the OnPlayerEnteredNewRoom event
    */
    public OnPlayerOpeningDoorEvent OnPlayerOpeningDoor { get; set; }

    /**
    * Gets or sets the OnPlayerEnteredNewRoom event
    */
    public OnPlayerEnteredNewRoomEvent OnPlayerEnteredNewRoom { get; set; }

    #endregion

    #region Public functions

    /**
    * Runs the door interlude animation
    *@param sourceTag - the source room where the player was when the door was opened
    */
    public void Run(string sourceTag)
    {
        m_SourceTag = sourceTag;

        // notify that animation is running
        m_IsRunning = true;

        // notify that the player started to open the door
        OnPlayerOpeningDoor?.Invoke(this, m_SourceTag);

        // start the animations
        m_DoorAnimator.SetBool("startAnim", true);
        m_CameraAnimator.SetBool("startAnim", true);
    }

    /**
    * Called when walking animation is starting
    *@param sender - event sender
    */
    public void OnStartWalk(object sender)
    {
        m_FootSteps.Play();
    }

    /**
    * Called when door is reached
    *@param sender - event sender
    */
    public void OnDoorReached(object sender)
    {
        m_FootSteps.Stop();
        m_DoorOpening.Play();
    }

    /**
    * Called when door is opened
    *@param sender - event sender
    */
    public void OnDoorOpened(object sender)
    {
        m_FootSteps.Play();
    }

    /**
    * Called when walking animation is stopped
    *@param sender - event sender
    */
    public void OnStopWalk(object sender)
    {
        // stop the sound
        m_FootSteps.Stop();

        // reset the start animation parameters
        m_DoorAnimator.SetBool("startAnim", false);
        m_CameraAnimator.SetBool("startAnim", false);

        // reset the animations to their initial state
        m_DoorAnimator.Rebind();
        m_CameraAnimator.Rebind();

        // notify that the player entered in a new room
        OnPlayerEnteredNewRoom?.Invoke(this, m_SourceTag);

        // notify that animation stopped
        m_IsRunning = false;
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

        // get the interlude camera
        m_Camera = m_InterludeScene.GetComponentInChildren<Camera>();
        Debug.Assert(m_Camera);

        // get the camera animation events
        m_CameraAnimEvents = m_InterludeScene.GetComponentInChildren<GS_CameraAnimEvents>();
        Debug.Assert(m_CameraAnimEvents);
        m_CameraAnimEvents.OnStartWalk   = OnStartWalk;
        m_CameraAnimEvents.OnDoorReached = OnDoorReached;
        m_CameraAnimEvents.OnDoorOpened  = OnDoorOpened;
        m_CameraAnimEvents.OnStopWalk    = OnStopWalk;

        // get the children animators (interlude should own 2 animations)
        Component[] animators = GetComponentsInChildren<Animator>();
        Debug.Assert(animators.Length == 2);

        // get the door animator
        m_DoorAnimator = animators[0] as Animator;
        Debug.Assert(m_DoorAnimator);

        // get the camera animator
        m_CameraAnimator = animators[1] as Animator;
        Debug.Assert(m_CameraAnimator);

        // get the children audio sources (interlude should own 2 audio sources)
        Component[] components = GetComponentsInChildren<AudioSource>();
        Debug.Assert(components.Length == 2);

        // get the door opening source
        m_DoorOpening = components[0] as AudioSource;
        Debug.Assert(m_DoorOpening);

        // get the footsteps audio source
        m_FootSteps = components[1] as AudioSource;
        Debug.Assert(m_FootSteps);
    }

    #endregion
}
