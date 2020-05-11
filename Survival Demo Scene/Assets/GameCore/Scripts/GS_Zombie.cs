using UnityEngine;
using UnityEngine.AI;

/**
* This class manages the zombie character AI, gestures and events
* @author Jean-Milost Reymond
*/
public class GS_Zombie : MonoBehaviour
{
    #region Public delegates

    /**
    * Called when the first attack is starting
    *@param sender - event sender
    */
    public delegate void OnStartFirstAttackEvent(object sender);

    /**
    * Called when the first attack hit point is reached
    *@param sender - event sender
    *@param zombiePos - the zombie position
    */
    public delegate void OnFirstAttackHitEvent(object sender, Transform zombiePos);

    /**
    * Called when the second attack is starting
    *@param sender - event sender
    */
    public delegate void OnStartSecondAttackEvent(object sender);

    /**
    * Called when the second attack hit point is reached
    *@param sender - event sender
    *@param zombiePos - the zombie position
    */
    public delegate void OnSecondAttackHitEvent(object sender, Transform zombiePos);

    #endregion

    #region Public members

    public enum IEMachineState
    {
        IE_MS_Idle,
        IE_MS_Chasing,
        IE_MS_Attacking
    };

    public Transform m_Target;

    #endregion

    #region Private members

    private GameObject     m_InterludeScene;
    private Animator       m_Animator;
    private NavMeshAgent   m_Agent;
    private AudioSource    m_ArmBeatingAir;
    private AudioSource    m_Attacking;
    private AudioSource    m_Breath;
    private AudioSource    m_FootSteps;
    private GS_Interlude   m_Interlude;
    private IEMachineState m_MachineState = IEMachineState.IE_MS_Idle;
    private bool           m_MachineStateChanged;
    private bool           m_AttackStarted;

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

    /**
    * Gets or sets the StartFirstAttack event
    */
    public OnStartFirstAttackEvent OnStartFirstAttack { get; set; }

    /**
    * Gets or sets the FirstAttackHit event
    */
    public OnFirstAttackHitEvent OnFirstAttackHit { get; set; }

    /**
    * Gets or sets the StartSecondAttack event
    */
    public OnStartSecondAttackEvent OnStartSecondAttack { get; set; }

    /**
    * Gets or sets the SecondAttackHit event
    */
    public OnSecondAttackHitEvent OnSecondAttackHit { get; set; }

    #endregion

    #region Public functions

    /**
    * Called when first attack is starting
    *@param sender - event sender
    */
    public void StartFirstAttack()
    {
        m_ArmBeatingAir.Play();

        OnStartFirstAttack?.Invoke(this);
    }

    /**
    * Called when the first attack hit point is reached
    *@param sender - event sender
    *@param zombiePos - the zombie position
    */
    public void FirstAttackHit()
    {
        OnFirstAttackHit?.Invoke(this, transform);
    }

    /**
    * Called when the second attack is starting
    *@param sender - event sender
    */
    public void StartSecondAttack()
    {
        m_ArmBeatingAir.Play();

        OnStartSecondAttack?.Invoke(this);
    }

    /**
    * Called when the second attack hit point is reached
    *@param sender - event sender
    *@param zombiePos - the zombie position
    */
    public void SecondAttackHit()
    {
        OnSecondAttackHit?.Invoke(this, transform);
    }

    #endregion

    #region Private functions

    /**
    * Starts the script
    */
    void Start()
    {
        Debug.Assert(m_Target);

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

        // get the children audio sources (zombie should own 4 sounds)
        Component[] components = GetComponentsInChildren<AudioSource>();
        Debug.Assert(components.Length == 4);

        // get the arm beating air audio source
        m_ArmBeatingAir = components[0] as AudioSource;
        Debug.Assert(m_ArmBeatingAir);

        // get the attacking audio source
        m_Attacking = components[1] as AudioSource;
        Debug.Assert(m_Attacking);

        // get the breath audio source
        m_Breath = components[2] as AudioSource;
        Debug.Assert(m_Breath);

        // get the footsteps audio source
        m_FootSteps = components[3] as AudioSource;
        Debug.Assert(m_FootSteps);
    }

    /**
    * Updates the scene (synchronous, once per frame)
    */
    void Update()
    {
        IEMachineState machineState = MachineState;

        // if the zombie is close to the target, attack it
        if (!m_Interlude.IsRunning && Vector3.Distance(m_Target.position, transform.position) < 1.5f)
        {
            if (!m_AttackStarted)
            {
                m_AttackStarted       = true;
                m_MachineStateChanged = true;
            }

            machineState = IEMachineState.IE_MS_Attacking;
        }
        else
            m_AttackStarted = false;

        // execute the running action
        switch (machineState)
        {
            case IEMachineState.IE_MS_Chasing:   ExecuteChasing();   break;
            case IEMachineState.IE_MS_Attacking: ExecuteAttacking(); break;
            default:                             ExecuteIdle();      break;
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
        m_Animator.SetBool("isAttacking", false);

        // stop the footsteps sound
        if (m_FootSteps.isPlaying)
            m_FootSteps.Stop();

        // stop the attacking sound
        if (m_Attacking.isPlaying)
            m_Attacking.Stop();
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
        m_Animator.SetBool("isAttacking", false);

        // play the footsteps sound
        if (!m_FootSteps.isPlaying)
            m_FootSteps.Play();
    }

    /**
    * Executes the attacking action
    */
    void ExecuteAttacking()
    {
        if (m_MachineStateChanged)
            m_Attacking.Play();

        // stop the zombie on its current position
        m_Agent.SetDestination(transform.position);
        transform.LookAt(m_Target);

        // run the attacking animation
        m_Animator.SetBool("isMoving", false);
        m_Animator.SetBool("isAttacking", true);

        // stop the footsteps sound
        if (m_FootSteps.isPlaying)
            m_FootSteps.Stop();
    }

    #endregion
}
