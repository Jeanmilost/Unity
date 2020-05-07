using UnityEngine;
using UnityEngine.AI;

public class GS_Zombie : MonoBehaviour
{
    public Transform   m_Target;

    private Animator     m_Animator;
    private NavMeshAgent m_Agent;
    private AudioSource  m_Breath;
    private AudioSource  m_FootSteps;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_Target);

        m_Animator = GetComponent<Animator>();
        Debug.Assert(m_Animator);

        m_Agent = GetComponent<NavMeshAgent>();
        Debug.Assert(m_Agent);

        // get the children audio sources (zombie should own 2 sounds)
        Component[] components = GetComponentsInChildren<AudioSource>();
        Debug.Assert(components.Length == 2);

        // get the key lock audio source
        m_Breath = components[0] as AudioSource;
        Debug.Assert(m_Breath);

        // get the key lock audio source
        m_FootSteps = components[1] as AudioSource;
        Debug.Assert(m_FootSteps);
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.SetBool("isMoving", true);
        m_Agent.SetDestination(m_Target.position);
    }
}
