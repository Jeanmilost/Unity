using UnityEngine;

/**
* This class manages the opening door interlude
* @author Jean-Milost Reymond
*/
public class GS_Interlude : MonoBehaviour
{
    #region Private members

    private GameObject  m_InterludeScene;
    private GameObject  m_Door;
    private Camera      m_Camera;
    private Animator    m_DoorAnimator;
    private Animator    m_CameraAnimator;
    private AudioSource m_DoorOpening;
    private AudioSource m_FootSteps;
    private bool        m_Running;

    #endregion

    #region Public properties

    /**
    * Gets if the interlude is running
    */
    public bool IsRunning { get; } = false;
    public bool Running   { get => m_Running; }

    #endregion

    #region Public functions

    /**
    * Runs the door interlude animation
    */
    public void Run()
    {
        // notify that animation is running
        m_Running = true;

        // show the scene
        m_Camera.enabled = true;

        // start the animations
        m_DoorAnimator.SetBool("startAnim", true);
        m_CameraAnimator.SetBool("startAnim", true);
    }

    /**
    * Called when walking animation is starting
    */
    public void OnStartWalk()
    {
        m_FootSteps.Play();
    }

    /**
    * Called when door is reached
    */
    public void OnDoorReached()
    {
        m_FootSteps.Stop();
        m_DoorOpening.Play();
    }

    /**
    * Called when door is opened
    */
    public void OnDoorOpened()
    {
        m_FootSteps.Play();
    }

    /**
    * Called when walking animation is stopped
    */
    public void OnStopWalk()
    {
        // stop the sound
        m_FootSteps.Stop();

        // disable the camera
        m_Camera.enabled = false;

        // reset the start animation parameters
        m_DoorAnimator.SetBool("startAnim", false);
        m_CameraAnimator.SetBool("startAnim", false);

        // reset the animations to their initial state
        m_DoorAnimator.Rebind();
        m_CameraAnimator.Rebind();

        // notify that animation stopped
        m_Running = false;
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
