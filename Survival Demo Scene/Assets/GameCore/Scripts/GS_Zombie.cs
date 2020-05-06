using UnityEngine;
using UnityEngine.AI;

public class GS_Zombie : MonoBehaviour
{
    public Transform m_Target;

    private Animator     m_Animator;
    private NavMeshAgent m_Agent;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(m_Target);

        m_Animator = GetComponent<Animator>();
        Debug.Assert(m_Animator);

        m_Agent = GetComponent<NavMeshAgent>();
        Debug.Assert(m_Agent);
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.SetBool("isMoving", true);
        m_Agent.SetDestination(m_Target.position);
    }
}
