using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* This class manages the opening door interlude
* @author Jean-Milost Reymond
*/
public class GS_Interlude : MonoBehaviour
{
    #region Public members
    #endregion

    #region Private members

    private GameObject m_InterludeScene;
    private GameObject m_Door;
    private Camera     m_Camera;
    private Animator   m_DoorAnimator;
    private Animator   m_CameraAnimator;
    private bool       m_Running;

    #endregion

    #region Public properties

    /**
    * Gets if the interlude is running
    */
    public bool IsRunning { get; } = false;

    #endregion

    public void Run()
    {
        m_Running = true;

        m_Camera.enabled = true;
        m_DoorAnimator.SetBool("startAnim", true);
        m_CameraAnimator.SetBool("startAnim", true);
    }

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

        // get the key lock audio source
        m_DoorAnimator = animators[0] as Animator;
        Debug.Assert(m_DoorAnimator);

        // get the key lock audio source
        m_CameraAnimator = animators[1] as Animator;
        Debug.Assert(m_CameraAnimator);
    }

    /**
    * Updates the scene (synchronous, once per frame)
    */
    void Update()
    {}

    #endregion
}
