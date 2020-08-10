using System;
using UnityEngine;

/**
* Provides a player controller
*@author Jean-Milost Reymond
*/
public class WS_PlayerController : MonoBehaviour
{
    /**
    * Sound controller, contains the sounds the player may emit while he's moving
    */
    public class ISoundController : WS_SoundController
    {
        /**
        * Audio source type
        *@note The item sorting and count should match with the m_AudioSources content
        */
        public enum IEType
        {
            IE_T_FootSteps_Concrete = 0
        };
    }

    /**
    * Player rotation controller, contains the rotations the player may perform
    */
    [Serializable]
    public class IRotationController
    {
        /**
        * Head rotation values
        */
        [Serializable]
        public class IHead
        {
            // sensitivity
            [Tooltip("Head sensitivity")]
            public float m_Sensitivity = 2.0f;

            // minimum angle
            [Tooltip("The minimum angle value the head can look at")]
            public float m_MinAngle = -45.0f;

            // maximum angle
            [Tooltip("The maximum angle value the head can look at")]
            public float m_MaxAngle = 89.5f;

            // do clamp rotation
            [Tooltip("If true, the head rotation will be clamped")]
            public bool m_DoClampRotation = true;

            private Quaternion m_Rotation = new Quaternion();

            /**
            * Gets or sets the rotation
            */
            public Quaternion Rotation
            {
                get
                {
                    return m_Rotation;
                }

                set
                {
                    // limit the head rotation inside the human body acceptable limits
                    if (m_DoClampRotation)
                    {
                        m_Rotation = ClampRotation(value);
                        return;
                    }

                    m_Rotation = value;
                }
            }

            /**
            * Initializes the class
            *@param q - quaternion containing the initial rotation
            */
            public void Init(Quaternion q)
            {
                m_Rotation = q;
            }

            /**
            * Keeps the head rotation inside human body acceptable limits
            *@param q - quaternion containing the head rotation to limit
            *@return limited head rotation
            */
            Quaternion ClampRotation(Quaternion q)
            {
                q.x /= q.w;
                q.y /= q.w;
                q.z /= q.w;
                q.w  = 1.0f;

                float angle = Mathf.Clamp(2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x), m_MinAngle, m_MaxAngle);
                q.x         = Mathf.Tan(0.5f * Mathf.Deg2Rad * angle);

                return q;
            }
        }

        /**
        * Body rotation values
        */
        [Serializable]
        public class IBody
        {
            // sensitivity
            [Tooltip("Body sensitivity")]
            public float m_Sensitivity = 2.0f;

            /**
            * Gets or sets the rotation
            */
            public Quaternion Rotation { get; set; }

            /**
            * Initializes the class
            *@param q - quaternion containing the initial rotation
            */
            public void Init(Quaternion q)
            {
                Rotation = q;
            }
        }

        public IHead m_Head = new IHead();
        public IBody m_Body = new IBody();

        // smooth
        [Tooltip("If true, the rotations will be smooth")]
        public bool m_Smooth = true;

        // smooth time
        [Tooltip("The smooth time")]
        public float m_SmoothTime = 18.0f;

        /**
        * Initializes the class
        *@param head - initial head rotation
        *@param body - initial body rotation
        */
        public void Init(Transform head, Transform body)
        {
            // initialize the player rotations
            m_Head.Init(head.localRotation);
            m_Body.Init(body.localRotation);
        }

        /**
        * Rotates the character and camera
        *@param head - object transformation representing the head
        *@param body - object transformation representing the body
        *@param axis - rotation to apply on the head or the body, in percent and between -1.0f and 1.0f.
        *              The X value represents the body, while the Y value represents the head
        */
        public void Rotate(Transform head, Transform body, Vector2 axis)
        {
            // no rotation?
            if (axis == Vector2.zero)
                return;

            // calculate the movements amplitude
            float headAmplitude = axis.y * m_Head.m_Sensitivity;
            float bodyAmplitude = axis.x * m_Body.m_Sensitivity;

            // calculate the resulting rotation
            m_Body.Rotation *= Quaternion.Euler(0.0f, bodyAmplitude, 0.0f);
            m_Head.Rotation *= Quaternion.Euler(-headAmplitude, 0.0f, 0.0f);

            // do apply smooth rotation?
            if (m_Smooth)
            {
                // calculate and apply smooth rotation
                head.localRotation = Quaternion.Slerp(head.localRotation, m_Head.Rotation, m_SmoothTime * Time.deltaTime);
                body.localRotation = Quaternion.Slerp(body.localRotation, m_Body.Rotation, m_SmoothTime * Time.deltaTime);
            }
            else
            {
                // apply rotation
                head.localRotation = m_Head.Rotation;
                body.localRotation = m_Body.Rotation;
            }
        }
    }

    /**
    * Player translation controller, contains the translations the player may perform
    */
    [Serializable]
    public class ITranslationController
    {
        // forward speed
        [Tooltip("The walking forward speed")]
        public float m_ForwardSpeed = 8.0f;

        // backward speed
        [Tooltip("The walking backward speed")]
        public float m_BackwardSpeed = 4.0f;

        // sideway speed
        [Tooltip("The walking sideway speed")]
        public float m_SidewaySpeed = 4.0f;

        // run multiplier
        [Tooltip("The run multiplier")]
        public float m_RunMultiplier = 2.0f;

        // jump force
        [Tooltip("The jump force")]
        public float m_JumpForce = 50.0f;

        // slope curve
        [Tooltip("The slope curve")]
        public AnimationCurve m_SlopeCurve = new AnimationCurve(new Keyframe(-90.0f, 1.0f),
                                                                new Keyframe(0.0f, 1.0f),
                                                                new Keyframe(90.0f, 0.0f));

        [HideInInspector] public float m_CurrentTargetSpeed = 8.0f;

        /**
        * Gets if player is running
        */
        public bool Running { get; private set; }

        /**
        * Calculates and updates the target speed
        *@param dir - target direction, in percent and between -1.0f and 1.0f. The X value represents the left/right
        *             side direction, while the Y value represents the forward/backward direction
        *@param running - if true, player is running
        */
        public void UpdateTargetSpeed(Vector2 dir, bool running)
        {
            // no direction?
            if (dir == Vector2.zero)
                return;

            // walking sideways?
            if (dir.x > 0.0f || dir.x < 0.0f)
                m_CurrentTargetSpeed = m_SidewaySpeed;

            // going backward or forward?
            if (dir.y < 0.0f)
                // backwards
                m_CurrentTargetSpeed = m_BackwardSpeed;
            else
            if (dir.y > 0.0f)
                // forwards. Handled last as if strafing and moving forward at the same time forwards speed should take precedence
                m_CurrentTargetSpeed = m_ForwardSpeed;

            Running = running;

            // is running?
            if (Running)
                m_CurrentTargetSpeed *= m_RunMultiplier;
        }
    }

    /**
    * Player model animation controller
    */
    public class IAnimationController
    {
        /**
        * Animation states
        */
        public enum IEAnimState
        {
            IE_AS_Idle,
            IE_AS_Running,
            IE_AS_BackwardRunning,
            IE_AS_Jumping,
            IE_AS_WalkingLeft,
            IE_AS_WalkingRight
        };

        private GameObject  m_Model;
        private Animator    m_Animator;
        private IEAnimState m_AnimState = IEAnimState.IE_AS_Idle;

        /**
        * Gets or sets the animation state
        */
        public IEAnimState AnimState
        {
            get
            {
                return m_AnimState;
            }

            set
            {
                m_AnimState = value;

                if (!m_Animator)
                    return;

                // apply the new state to the animator
                switch (m_AnimState)
                {
                    case IEAnimState.IE_AS_Idle:
                        m_Animator.SetBool("isJumping",         false);
                        m_Animator.SetBool("isRunningBackward", false);
                        m_Animator.SetBool("isRunning",         false);
                        m_Animator.SetBool("isWalkingLeft",     false);
                        m_Animator.SetBool("isWalkingRight",    false);
                        return;

                    case IEAnimState.IE_AS_Running:
                        m_Animator.SetBool("isJumping",         false);
                        m_Animator.SetBool("isRunningBackward", false);
                        m_Animator.SetBool("isRunning",         true);
                        m_Animator.SetBool("isWalkingLeft",     false);
                        m_Animator.SetBool("isWalkingRight",    false);
                        return;

                    case IEAnimState.IE_AS_BackwardRunning:
                        m_Animator.SetBool("isJumping",         false);
                        m_Animator.SetBool("isRunningBackward", true);
                        m_Animator.SetBool("isRunning",         false);
                        m_Animator.SetBool("isWalkingLeft",     false);
                        m_Animator.SetBool("isWalkingRight",    false);
                        return;

                    case IEAnimState.IE_AS_Jumping:
                        m_Animator.SetBool("isJumping",         true);
                        m_Animator.SetBool("isRunningBackward", false);
                        m_Animator.SetBool("isRunning",         false);
                        m_Animator.SetBool("isWalkingLeft",     false);
                        m_Animator.SetBool("isWalkingRight",    false);
                        return;

                    case IEAnimState.IE_AS_WalkingLeft:
                        m_Animator.SetBool("isJumping",         false);
                        m_Animator.SetBool("isRunningBackward", false);
                        m_Animator.SetBool("isRunning",         false);
                        m_Animator.SetBool("isWalkingLeft",     true);
                        m_Animator.SetBool("isWalkingRight",    false);
                        return;

                    case IEAnimState.IE_AS_WalkingRight:
                        m_Animator.SetBool("isJumping",         false);
                        m_Animator.SetBool("isRunningBackward", false);
                        m_Animator.SetBool("isRunning",         false);
                        m_Animator.SetBool("isWalkingLeft",     false);
                        m_Animator.SetBool("isWalkingRight",    true);
                        return;
                }
            }
        }

        /**
        * Initializes the class
        *@param modelName - the model name to animate
        */
        public bool Init(string modelName)
        {
            // get the player model
            m_Model = GameObject.Find(modelName);

            // found it?
            if (!m_Model)
                return false;

            // get the model animator
            m_Animator = m_Model.GetComponent<Animator>();

            if (!m_Animator)
                return false;

            return true;
        }
    }

    /**
    * Gets the sounds to play while player is moving
    *@param sender - event sender
    *@param ambient - ambient name for which the sound should be get
    *@param sounds - sounds to play while player is moving
    *@note The sounds should remain available as long as they are played while the player moves
    */
    public delegate void OnGetSoundsEvent(object sender, string ambient, ISoundController sounds);

    /**
    * Gets the rotation gesture to apply to the player
    *@param sender - event sender
    *@return the rotation gesture to apply on the head or the body, in percent and between -1.0f and 1.0f.
    *        The X value represents the body, while the Y value represents the head
    */
    public delegate Vector2 OnGetRotationGestureEvent(object sender);

    /**
    * Gets the translation gesture to apply to the player
    *@param sender - event sender
    *@return the translation gesture to apply on the head or the body, in percent and between -1.0f and 1.0f.
    *        The X value represents the body, while the Y value represents the head
    */
    public delegate Vector2 OnGetTranslationGestureEvent(object sender);

    /**
    * Gets if the player should jump
    *@param sender - event sender
    *@return true if player should jump, otherwise false
    */
    public delegate bool OnGetJumpStatusEvent(object sender);

    /**
    * Gets if the player should run
    *@param sender - event sender
    *@return true if player should run, otherwise false
    */
    public delegate bool OnGetRunStatusEvent(object sender);

    /**
    * Gets if the game is paused
    *@param sender - event sender
    *@return true if game is paused, otherwise false
    */
    public delegate bool OnGetPausedStatusEvent(object sender);

    /**
    * Notifies that player entered in collision with something
    *@param sender - event sender
    *@param rigidBody - player rigid body
    *@param collider - player collider
    *@param collision - collision info
    */
    public delegate void OnCollisionEvent(object sender, Rigidbody rigidBody, Collider collider, Collision collision);

    public IRotationController    m_RotationController    = new IRotationController();
    public ITranslationController m_TranslationController = new ITranslationController();

    // slow down rate
    [Tooltip("Slowdown rate, applied when player is no longer moving")]
    public float m_SlowDownRate = 20.0f;

    // shell offset
    [Tooltip("Shell offset, reduce the player radius by that value to avoid getting stuck in walls (normally set to 0.1f)")]
    public float m_ShellOffset = 0.1f;

    // ground check epsilon distance
    [Tooltip("Ground check epsilon distance, used to check if the controller is grounded (normally set to 0.1f)")]
    public float m_GroundCheckDistance = 0.1f;

    // ground check helper distance
    [Tooltip("Stick to ground helper distance, used to guarantee a minimum distance when the ground test is performed")]
    public float m_StickToGroundHelperDistance = 0.6f;

    // air control
    [Tooltip("If true, the player may continue to move even while he's jumping")]
    public bool m_AirControl;

    private readonly IAnimationController m_AnimationController = new IAnimationController();
    private readonly ISoundController     m_SoundController     = new ISoundController();
    private          Vector3              m_GroundContactNormal;
    private          bool                 m_Running;
    private          bool                 m_BackwardRunning;
    private          bool                 m_WalkingLeft;
    private          bool                 m_WalkingRight;
    private          bool                 m_Jump;
    private          bool                 m_PreviouslyGrounded;

    /**
    * Gets the player camera
    */
    public GameObject Camera { get; private set; }

    /**
    * Gets the player rigid body
    */
    public Rigidbody RigidBody { get; private set; }

    /**
    * Gets the player collider
    */
    public CapsuleCollider Collider { get; private set; }

    /**
    * Gets the player velocity
    */
    public Vector3 Velocity
    {
        get
        {
            return RigidBody.velocity;
        }
    }

    /**
    * Gets if the player lies on the ground
    */
    public bool IsGrounded { get; private set; }

    /**
    * Gets if the player is jumping
    */
    public bool Jumping { get; private set; }

    /**
    * Gets if the player is running
    */
    public bool Running
    {
        get
        {
            return m_TranslationController.Running;
        }
    }

    /**
    * Gets or sets if the view is enabled
    */
    public bool Enabled { get; set; } = true;

    /**
    * Gets or sets the OnGetSounds event
    */
    public OnGetSoundsEvent OnGetSounds { get; set; }

    /**
    * Gets or sets the OnGetRotationGesture event
    */
    public OnGetRotationGestureEvent OnGetRotationGesture { get; set; }

    /**
    * Gets or sets the OnGetTranslationGesture event
    */
    public OnGetTranslationGestureEvent OnGetTranslationGesture { get; set; }

    /**
    * Gets or sets the OnGetJumpStatus event
    */
    public OnGetJumpStatusEvent OnGetJumpStatus { get; set; }

    /**
    * Gets or sets the OnGetRunStatus event
    */
    public OnGetRunStatusEvent OnGetRunStatus { get; set; }

    /**
    * Gets or sets the OnGetPausedStatus event
    */
    public OnGetPausedStatusEvent OnGetPausedStatus { get; set; }

    /**
    * Gets or sets the OnCollision event
    */
    public OnCollisionEvent OnCollision { get; set; }

    /**
    * Starts the script
    */
    void Start()
    {
        // get the player components located around the script
        RigidBody = GetComponent<Rigidbody>();
        Collider  = GetComponent<CapsuleCollider>();

        Debug.Assert(RigidBody);
        Debug.Assert(Collider);

        // get the child player camera
        Camera = GameObject.Find("Camera");
        Debug.Assert(Camera);

        // initialize the player rotations
        m_RotationController.Init(Camera.transform, transform);

        // initialize the animation controller. NOTE if no model was found, the game may run without
        if (!m_AnimationController.Init("Yuna"))
            Debug.Log("No model found for player, the game will run without");

        // initialize the player sounds
        m_SoundController.Init(GetComponentsInChildren<AudioSource>());
    }

    /**
    * Updates the scene (synchronous, once per frame)
    */
    void Update()
    {
        // is view disabled?
        if (!Enabled)
            return;

        // is game paused?
        if (GetPausedStatus() && !m_Jump)
            return;

        // rotate the player in response to the mouse/touch events
        Rotate(GetRotationGesture());

        // check if the player should jump, and if not currently jumping, jump
        if (GetJumpStatus() && !m_Jump)
            m_Jump = true;
    }

    /**
    * Updates the scene (asynchronous, depends on frame rate)
    *@note The FixedUpdate() function should be used when applying forces, torques, or other physics-related functions,
    *      because it will be executed exactly in sync with the physics engine itself
    */
    void FixedUpdate()
    {
        // is view disabled?
        if (!Enabled)
            return;

        // is game paused?
        if (GetPausedStatus())
            return;

        CheckGround();

        Vector2 translation = GetTranslationGesture();

        // apply translation and jump effect to player
        Translate(translation);
        Jump(translation);
    }

    /**
    * Called when the player enters in a trigger zone
    *@param collider - trigger zone collider
    */
    void OnTriggerEnter(Collider collider)
    {
        // is view disabled?
        if (!Enabled)
            return;

        // get the sounds to play matching with the trigger zone
        OnGetSounds?.Invoke(this, collider.name, m_SoundController);
    }

    /**
    * Called when the player quits a trigger zone
    *@param collider - trigger zone collider
    */
    void OnTriggerExit(Collider collider)
    {
        // is view disabled?
        if (!Enabled)
            return;

        // get back the previous sounds to play
        OnGetSounds?.Invoke(this, "prev_ambient_sound", m_SoundController);
    }

    /**
    * Called when the player hits something
    *@param collision - collision infor
    */
    void OnCollisionEnter(Collision collision)
    {
        // is view disabled?
        if (!Enabled)
            return;

        // get back the previous sounds to play
        OnCollision?.Invoke(this, RigidBody, Collider, collision);
    }

    /**
    * Gets the translation gesture
    *@return the translation gesture
    */
    Vector2 GetTranslationGesture()
    {
        return OnGetTranslationGesture?.Invoke(this) ?? new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    /**
    * Gets the rotation gesture
    *@return the rotation gesture
    */
    Vector2 GetRotationGesture()
    {
        return OnGetRotationGesture?.Invoke(this) ?? new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    /**
    * Gets the jump status
    *@return the jump status
    */
    bool GetJumpStatus()
    {
        return OnGetJumpStatus?.Invoke(this) ?? (Input.GetAxis("Jump") != 0.0f);
    }

    /**
    * Gets the run status
    *@return the run status
    */
    bool GetRunStatus()
    {
        return OnGetRunStatus?.Invoke(this) ?? (Input.GetAxis("Run") != 0.0f);
    }

    /**
    * Gets the paused status
    *@return the paused status
    */
    bool GetPausedStatus()
    {
        return OnGetPausedStatus?.Invoke(this) ?? false;
    }

    /**
    * Translates the player
    *@param axis - translation axis, defined by the user inputs
    */
    void Translate(Vector2 axis)
    {
        // apply the translation
        m_TranslationController.UpdateTargetSpeed(axis, GetRunStatus());

        m_Running         = false;
        m_BackwardRunning = false;
        m_WalkingLeft     = false;
        m_WalkingRight    = false;
        bool stillPlaying = false;

        // is the input value large enough to move the player, and the player is lying on the ground or can be controlled while jumping?
        if ((Mathf.Abs(axis.x) > float.Epsilon || Mathf.Abs(axis.y) > float.Epsilon) && (m_AirControl || IsGrounded))
        {
            // always move along the camera forward as it is the direction that it being aimed by the player
            Vector3 desiredMove = Camera.transform.forward * axis.y + Camera.transform.right * axis.x;
            desiredMove         = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

            // calculate the desired move amplitude
            desiredMove.x *= m_TranslationController.m_CurrentTargetSpeed;
            desiredMove.y *= m_TranslationController.m_CurrentTargetSpeed;
            desiredMove.z *= m_TranslationController.m_CurrentTargetSpeed;

            // run the running animation
            if (IsGrounded)
            {
                // side walking
                if (axis.x > 0.0f)
                {
                    m_WalkingRight = true;

                    if (m_AnimationController.AnimState != IAnimationController.IEAnimState.IE_AS_WalkingRight)
                        m_AnimationController.AnimState = IAnimationController.IEAnimState.IE_AS_WalkingRight;
                }
                else
                if (axis.x < 0.0f)
                {
                    m_WalkingLeft = true;

                    if (m_AnimationController.AnimState != IAnimationController.IEAnimState.IE_AS_WalkingLeft)
                        m_AnimationController.AnimState = IAnimationController.IEAnimState.IE_AS_WalkingLeft;
                }

                // front or backward running
                if (axis.y > 0.0f)
                {
                    m_Running = true;

                    if (m_AnimationController.AnimState != IAnimationController.IEAnimState.IE_AS_Running)
                        m_AnimationController.AnimState = IAnimationController.IEAnimState.IE_AS_Running;
                }
                else
                if (axis.y < 0.0f)
                {
                    m_BackwardRunning = true;

                    if (m_AnimationController.AnimState != IAnimationController.IEAnimState.IE_AS_BackwardRunning)
                        m_AnimationController.AnimState = IAnimationController.IEAnimState.IE_AS_BackwardRunning;
                }
            }

            // do move the body?
            if (RigidBody.velocity.sqrMagnitude < (m_TranslationController.m_CurrentTargetSpeed * m_TranslationController.m_CurrentTargetSpeed))
            {
                // move the body by applying a force on it along the player direction
                RigidBody.AddForce(desiredMove * GetSlopeMultiplier(), ForceMode.Impulse);

                // play the footstep on concrete sound
                m_SoundController.Play((int)ISoundController.IEType.IE_T_FootSteps_Concrete);

                // keep the sound playing alive
                stillPlaying = true;
            }
        }

        // set animation to idle if no longer running
        if ((!m_Running         && m_AnimationController.AnimState == IAnimationController.IEAnimState.IE_AS_Running)         ||
            (!m_BackwardRunning && m_AnimationController.AnimState == IAnimationController.IEAnimState.IE_AS_BackwardRunning) ||
            (!m_WalkingLeft     && m_AnimationController.AnimState == IAnimationController.IEAnimState.IE_AS_WalkingLeft)     ||
            (!m_WalkingRight    && m_AnimationController.AnimState == IAnimationController.IEAnimState.IE_AS_WalkingRight))
            // check if grounded, in case the jump was not performed by the user (e.g when he falls or hit an obstacle)
            if (IsGrounded)
                m_AnimationController.AnimState = IAnimationController.IEAnimState.IE_AS_Idle;
            else
                m_AnimationController.AnimState = IAnimationController.IEAnimState.IE_AS_Jumping;

        // don't allow single file footsteps to run infinitely
        if (!stillPlaying)
            m_SoundController.Stop((int)ISoundController.IEType.IE_T_FootSteps_Concrete);
    }

    /**
    * Rotates the player
    *@param axis - rotation axis, defined by the user inputs
    */
    void Rotate(Vector2 axis)
    {
        // avoid the mouse looking if the game is effectively paused
        if (Mathf.Abs(Time.timeScale) < float.Epsilon)
            return;

        if (axis.x == 0.0f && axis.y == 0.0f)
            return;

        // get the rotation before it's changed
        float oldYRotation = transform.eulerAngles.y;

        // get the player rotation, and apply it to the head and body
        m_RotationController.Rotate(Camera.transform, transform, axis);

        // is player grounded, or can player modify his rotation on jumping?
        if (IsGrounded || m_AirControl)
        {
            // rotate the rigidbody velocity to match the new direction that the character is looking
            Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            RigidBody.velocity     = velRotation * RigidBody.velocity;
        }
    }

    /**
    * Applies the jump effect to the player
    *@param axis - translation axis, defined by the user inputs
    */
    void Jump(Vector2 axis)
    {
        // is body currently lying on the ground?
        if (IsGrounded)
        {
            RigidBody.drag = 5.0f;

            // not already jumping?
            if (m_Jump)
            {
                Jumping = true;

                // apply the jump force
                RigidBody.drag     = 0.0f;
                RigidBody.velocity = new Vector3(RigidBody.velocity.x, 0.0f, RigidBody.velocity.z);
                RigidBody.AddForce(new Vector3(0.0f, m_TranslationController.m_JumpForce, 0.0f), ForceMode.Impulse);

                // run the jump animation
                m_AnimationController.AnimState = IAnimationController.IEAnimState.IE_AS_Jumping;
            }

            // mitigate the jump effect when the player lies again on the ground, and if he isn't running
            if (!Jumping && Mathf.Abs(axis.x) < float.Epsilon && Mathf.Abs(axis.y) < float.Epsilon && RigidBody.velocity.magnitude < 1.0f)
                RigidBody.Sleep();
        }
        else
        {
            RigidBody.drag = 0.0f;

            // if not jumping, check if the player is falling down a slope
            if (m_PreviouslyGrounded && !Jumping)
                StickToGround();
        }

        m_Jump = false;
    }

    /**
    * Calculates and gets the slope multiplier to apply
    *@return the slope multiplier
    */
    float GetSlopeMultiplier()
    {
        return m_TranslationController.m_SlopeCurve.Evaluate(Vector3.Angle(m_GroundContactNormal, Vector3.up));
    }

    /**
    * Applies a velocity on the player body until it lies on the ground
    */
    void StickToGround()
    {
        // is player lying on the ground?
        if (Physics.SphereCast(transform.position,
                               Collider.radius * (1.0f - m_ShellOffset),
                               Vector3.down,
                               out RaycastHit hitInfo,
                               ((Collider.height / 2.0f) - Collider.radius) + m_StickToGroundHelperDistance,
                               Physics.AllLayers,
                               QueryTriggerInteraction.Ignore))
            // is slope higher or equals to 85°?
            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85.0f)
                // applies the gravity force to the rigid body
                RigidBody.velocity = Vector3.ProjectOnPlane(RigidBody.velocity, hitInfo.normal);
    }

    /**
    * Check if the body lies on the ground
    */
    void CheckGround()
    {
        m_PreviouslyGrounded = IsGrounded;

        // sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        if (Physics.SphereCast(transform.position,
                                Collider.radius * (1.0f - m_ShellOffset),
                                Vector3.down,
                                out RaycastHit hitInfo,
                                ((Collider.height / 2.0f) - Collider.radius) + m_GroundCheckDistance,
                                Physics.AllLayers,
                                QueryTriggerInteraction.Ignore))
        {
            IsGrounded            = true;
            m_GroundContactNormal = hitInfo.normal;
        }
        else
        {
            IsGrounded            = false;
            m_GroundContactNormal = Vector3.up;
        }

        // jump ended?
        if (!m_PreviouslyGrounded && IsGrounded && Jumping)
            Jumping = false;

        // reset the jump animation to idle or to the appropriate running animation if the jump ends
        if (!Jumping && IsGrounded  && m_AnimationController.AnimState == IAnimationController.IEAnimState.IE_AS_Jumping)
            if (m_Running)
                m_AnimationController.AnimState = IAnimationController.IEAnimState.IE_AS_Running;
            else
            if (m_BackwardRunning)
                m_AnimationController.AnimState = IAnimationController.IEAnimState.IE_AS_BackwardRunning;
            else
            if (m_WalkingLeft)
                m_AnimationController.AnimState = IAnimationController.IEAnimState.IE_AS_WalkingLeft;
            else
            if (m_WalkingRight)
                m_AnimationController.AnimState  = IAnimationController.IEAnimState.IE_AS_WalkingRight;
            else
                m_AnimationController.AnimState = IAnimationController.IEAnimState.IE_AS_Idle;
    }
}
