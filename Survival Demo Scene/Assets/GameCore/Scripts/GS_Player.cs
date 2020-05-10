using UnityEngine;

/**
* This class manages the main character gestures and events
* @author Jean-Milost Reymond
*/
public class GS_Player : MonoBehaviour
{
    #region Public delegates

    /**
    * Called while the character stays inside a trigger zone
    *@param sender - event sender
    *@param playerController - player controller
    *@param collider - trigger zone collider in which the character is staying
    */
    public delegate void OnTriggerInsideEvent(object sender, CharacterController playerController, Collider collider);

    #endregion

    #region Public members

    public float m_MoveSpeed     = 1.95f;
    public float m_RotationSpeed = 250.0f;
    public float m_Gravity       = 9.81f;

    #endregion

    #region Private members

    private GameObject          m_Interlude;
    private CharacterController m_CharacterController;
    private Animator            m_Animator;
    private AudioSource         m_FootSteps;
    private GS_Interlude        m_InterludeManager;
    private Vector3             m_MoveDirection = Vector3.zero;
    private float               m_Angle         = 90.0f;

    #endregion

    #region Public properties

    /**
    * Gets or sets the OnTriggerInside event
    */
    public OnTriggerInsideEvent OnTriggerInside { get; set; }

    #endregion

    #region Public functions

    /**
    * Sets the player position and direction
    *@param pos - the player position
    *@param dir - the player direction
    */
    public void SetPlayerPosAndDir(Vector3 pos, Vector3 dir)
    {
        m_CharacterController.enabled               = false;
        m_CharacterController.transform.position    = pos;
        m_CharacterController.transform.eulerAngles = dir;
        m_CharacterController.enabled               = true;
    }

    #endregion

    #region Private functions

    /**
    * Starts the script
    */
    void Start()
    {
        // get the door interlude object
        m_Interlude = GameObject.Find("Interlude");
        Debug.Assert(m_Interlude);

        // get the door interlude script
        m_InterludeManager = m_Interlude.GetComponentInChildren<GS_Interlude>();
        Debug.Assert(m_InterludeManager);

        // get the player children objects
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator            = GetComponent<Animator>();
        m_FootSteps           = GetComponentInChildren<AudioSource>();

        Debug.Assert(m_CharacterController);
        Debug.Assert(m_Animator);
        Debug.Assert(m_FootSteps);
    }

    /**
    * Updates the scene (synchronous, once per frame)
    */
    void Update()
    {
        MoveCharacter();
    }

    /**
    * Called while the character stays inside a trigger zone
    *@param other - trigger zone collider in which the character is staying
    */
    void OnTriggerStay(Collider other)
    {
        OnTriggerInside?.Invoke(this, m_CharacterController, other);
    }

    /**
    * Moves the character
    */
    void MoveCharacter()
    {
        Vector2 axis;

        // get axis moved by the player (A, W, S, D or left/top/right/bottom arrows). Limit the vertical axis to a value
        // between 0 and 1, thus the player may only move forward
        if (!m_InterludeManager.IsRunning)
            axis = new Vector2(Input.GetAxis("Horizontal"), Mathf.Clamp01(Input.GetAxis("Vertical")));
        else
            axis = new Vector2(0.0f, 0.0f);

        // calculate angle at which player is looking
        m_Angle = (m_Angle + (m_RotationSpeed * axis.x * Time.deltaTime)) % 360.0f;

        // convert angle to radians
        float angleRad = m_Angle * Mathf.Deg2Rad;

        // calculate the movement direction to apply to the character controller. NOTE time is applied twice on the y axis
        // because the gravity is an acceleration, not just a velocity
        m_MoveDirection = new Vector3(Mathf.Sin(angleRad) * axis.y, 
                                      m_CharacterController.isGrounded == false ? -m_Gravity * Time.deltaTime : 0.0f,
                                      Mathf.Cos(angleRad) * axis.y);

        // move the player
        m_CharacterController.Move(m_MoveDirection * m_MoveSpeed * Time.deltaTime);

        // rotate the player
        transform.rotation = Quaternion.Euler(0, m_Angle, 0);

        bool isWalking = (axis.y != 0.0f);

        // show walking animation if the player is currently walking
        m_Animator.SetBool("isWalking", isWalking);

        // play or stop the walking footsteps sound
        if (isWalking)
        {
            if (!m_FootSteps.isPlaying)
                m_FootSteps.Play();
        }
        else
            m_FootSteps.Stop();
    }

    #endregion
}
