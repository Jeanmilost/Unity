using UnityEngine;
using UnityEngine.AI;

/**
* This class manages the zombie character AI, gestures and events
* @author Jean-Milost Reymond
*/
public class GS_Zombie : MonoBehaviour
{
    #region Public members

    public enum IEMachineState
    {
        IE_MS_Idle,
        IE_MS_Chasing
    };

    public Transform m_Target;

    #endregion

    #region Private members

    private GameObject     m_Room4;
    private GameObject     m_InterludeScene;
    private Camera         m_Camera4;
    private Animator       m_Animator;
    private NavMeshAgent   m_Agent;
    private AudioSource    m_Breath;
    private AudioSource    m_FootSteps;
    private GS_Interlude   m_Interlude;
    private IEMachineState m_MachineState = IEMachineState.IE_MS_Idle;
    private bool           m_MachineStateChanged;

    #endregion

    #region Public properties

    /**
    * Gets or sets the machine state
    */
    public IEMachineState MachineState
    {
        get
        {
            return m_MachineState;
        }
        
        set
        {
            m_MachineState        = value;
            m_MachineStateChanged = true;
        }
    }

    #endregion

    #region Private functions

    /**
    * Starts the script
    */
    void Start()
    {
        Debug.Assert(m_Target);

        // get the room 4
        m_Room4 = GameObject.Find("Room4");
        Debug.Assert(m_Room4);

        // get the room 4 camera
        m_Camera4 = m_Room4.GetComponentInChildren<Camera>();

        // get the interlude scene
        m_InterludeScene = GameObject.Find("Interlude");
        Debug.Assert(m_InterludeScene);

        // get the door interlude script
        m_Interlude = m_InterludeScene.GetComponentInChildren<GS_Interlude>();
        Debug.Assert(m_Interlude);

        m_Animator = GetComponent<Animator>();
        Debug.Assert(m_Animator);

        m_Agent = GetComponent<NavMeshAgent>();
        Debug.Assert(m_Agent);

        // get the children audio sources (zombie should own 2 sounds)
        Component[] components = GetComponentsInChildren<AudioSource>();
        Debug.Assert(components.Length == 2);

        // get the breath audio source
        m_Breath = components[0] as AudioSource;
        Debug.Assert(m_Breath);

        // get the footsteps audio source
        m_FootSteps = components[1] as AudioSource;
        Debug.Assert(m_FootSteps);
    }

    /**
    * Updates the scene (synchronous, once per frame)
    */
    void Update()
    {
        // execute the running action
        switch (m_MachineState)
        {
            case IEMachineState.IE_MS_Chasing: ExecuteChasing(); break;
            default:                           ExecuteIdle();    break;
        }

        // reset the machine state changed flag after having executed the current action at least once
        m_MachineStateChanged = false;
    }

    /**
    * Executes the idle action
    */
    void ExecuteIdle()
    {
        // stop the zombie on its current position
        m_Agent.SetDestination(transform.position);

        // run the idle animation
        m_Animator.SetBool("isMoving", false);

        // stop the footsteps sound
        if (m_FootSteps.isPlaying)
            m_FootSteps.Stop();
    }

    /**
    * Executes the chasing action
    */
    void ExecuteChasing()
    {
        if (m_MachineStateChanged)
            m_Breath.Play();

        // make the zombie to chase the player
        m_Agent.SetDestination(m_Target.position);

        // run the walking animation
        m_Animator.SetBool("isMoving", true);

        // play the footsteps sound
        if (!m_FootSteps.isPlaying)
            m_FootSteps.Play();
    }

    #endregion
}
