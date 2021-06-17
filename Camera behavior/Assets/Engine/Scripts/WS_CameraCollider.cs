using System.Collections.Generic;
using UnityEngine;

/**
* Camera collider
*@author Jean-Milost Reymond
*/
public class WS_CameraCollider : MonoBehaviour
{
    /**
    * Detection type enumeration
    */
    public enum IEDetectionType
    {
        IE_DT_PlayerToCamera,
        IE_DT_CameraToPlayer,
        IE_DT_Both
    };

    /**
    * Game object comparer, to use in the game object sorted set
    */
    public class IGameObjectComparer : IComparer<GameObject>
    {
        /**
        * Compare two game objects
        *@param first - first object to compare to
        *@param second - second object to compare with
        *@return 0 if objects are equals, less than 0 if first is less than second, or higher than 0 if first is greater than second
        */
        public int Compare(GameObject first, GameObject second)
        {
            if (!first)
                return 0;

            if (!second)
                return 0;

            return first.name.CompareTo(second.name);
        }
    }

    /**
    * Resolves the collision
    *@param sender - event sender
    *@param distance - closest distance from the target center
    *@param minDistance - minimum possible distance from the target center
    *@param maxDistance - maximum possible distance from the target center
    *@param prevDetectedObjects - detected objects list on the previous collision detection
    */
    public delegate void OnBeforeCheckCollisionEvent(object                sender,
                                                     float                 minDistance,
                                                     float                 maxDistance,
                                                     GameObject            camera,
                                                     GameObject            target,
                                                     SortedSet<GameObject> prevDetectedObjects);

    /**
    * Resolves the collision
    *@param sender - event sender
    *@param distance - closest distance from the target center
    *@param minDistance - minimum possible distance from the target center
    *@param maxDistance - maximum possible distance from the target center
    *@param detectedObjects - detected objects list
    */
    public delegate void OnResolveCollisionEvent(object                sender,
                                                 float                 distance,
                                                 float                 minDistance,
                                                 float                 maxDistance,
                                                 Vector3               proposedPos,
                                                 GameObject            camera,
                                                 GameObject            target,
                                                 SortedSet<GameObject> detectedObjects);

    // the layers that will be affected by collision
    [Tooltip("The layers that will be affected by collision")]
    public LayerMask m_CameraOcclusion;

    // min distance
    [Tooltip("The minimum distance the camera may reach")]
    public float m_MinDistance = 1.0f;

    // max distance
    [Tooltip("The maximum distance the camera may reach, if Auto Max Dir option is deactivated")]
    public float m_MaxDistance = 4.0f;

    // determine if the detection will happens from the player position to the camera position, or from camera to player, or both
    [Tooltip("determine if the detection will happens from the player position to the camera position, or from camera to player, or both")]
    public IEDetectionType m_DetectionType = IEDetectionType.IE_DT_PlayerToCamera;

    // whether the camera should follow the target or the player collider
    [Tooltip("If true, the camera will follow the camera target instead of the player collider")]
    public bool m_FollowTarget = false;

    // auto max distance
    [Tooltip("If true, the camera maximum distance will be calculated automatically from the current camera position")]
    public bool m_AutoMax = true;

    private          GameObject            m_Player;
    private          GameObject            m_CameraTarget;
    private          CapsuleCollider       m_PlayerCollider;
    private          SphereCollider        m_CameraCollider;
    private readonly SortedSet<GameObject> m_DetectedObjects = new SortedSet<GameObject>(new IGameObjectComparer());
    private          Vector3               m_Dir;
    private          Vector3               m_CamTargetDir;
    private          float                 m_Distance;
    private          float                 m_CamTargetDistance;
    private          float                 m_CamTargetMaxDistance;

    /**
    * Gets or sets the OnBeforeCheckCollision event
    */
    public OnBeforeCheckCollisionEvent OnBeforeCheckCollision { get; set; }

    /**
    * Gets or sets the OnResolveCollision event
    */
    public OnResolveCollisionEvent OnResolveCollision { get; set; }

    /**
    * Initializes the script (called only once in the game lifecycle)
    */
    void Awake()
    {
        // get the camera direction and starting distance
        m_Dir      = transform.localPosition.normalized;
        m_Distance = transform.localPosition.magnitude;

        // if auto max is enabled, the max distance will be deduced from the current camera position
        if (m_AutoMax)
            m_MaxDistance = m_Distance;
    }

    /**
    * Starts the script
    */
    void Start()
    {
        m_CameraCollider = GetComponent<SphereCollider>();
        Debug.Assert(m_CameraCollider);

        // get the child player camera
        m_Player = GameObject.Find("Player");
        Debug.Assert(m_Player);

        // get the camera target
        m_CameraTarget = GameObject.Find("CameraTarget");
        Debug.Assert(m_CameraTarget);

        // get the player collider
        m_PlayerCollider = m_Player.GetComponent<CapsuleCollider>();
        Debug.Assert(m_PlayerCollider);

        // get the camera direction and starting distance
        if (m_FollowTarget)
        {
            m_CamTargetDir      = (m_CameraTarget.transform.localPosition - transform.localPosition).normalized;
            m_CamTargetDistance = (m_CameraTarget.transform.localPosition - transform.localPosition).magnitude;

            // if auto max is enabled, the max distance will be deduced from the current camera position
            if (m_AutoMax)
                m_CamTargetMaxDistance = m_CamTargetDistance;
            else
                m_CamTargetMaxDistance = m_MaxDistance;
        }
    }

    /**
    * Post-updates the scene (synchronous, once per frame)
    */
    void LateUpdate()
    {
        float   maxDistance = m_FollowTarget ? m_CamTargetMaxDistance : m_MaxDistance;
        Vector3 dir         = m_FollowTarget ? m_CamTargetDir         : m_Dir;

        // notify that a new collision detection begins
        OnBeforeCheckCollision?.Invoke(this, m_MinDistance, maxDistance, transform.gameObject, m_Player, m_DetectedObjects);

        // clear the previous detection
        m_DetectedObjects.Clear();

        // get the target position the camera should reach
        Vector3 targetPosition = m_FollowTarget ? m_CameraTarget.transform.position : m_PlayerCollider.transform.position;

        // calculate the default camera position
        if (m_FollowTarget)
            transform.position =
                    transform.parent.TransformPoint
                            (new Vector3(m_CameraTarget.transform.localPosition.x - (m_CamTargetMaxDistance * m_CamTargetDir.x),
                                         m_CameraTarget.transform.localPosition.y - (m_CamTargetMaxDistance * m_CamTargetDir.y),
                                         m_CameraTarget.transform.localPosition.z - (m_CamTargetMaxDistance * m_CamTargetDir.z)));
        else
            transform.position = transform.parent.TransformPoint(m_Dir * m_MaxDistance);

        // calculate the distance between the camera and its target
        CalculateDistance(transform.position, targetPosition, maxDistance);

        Vector3 proposedPos;

        // recalculate the new camera position, if needed
        if (m_Distance != maxDistance)
        {
            // calculate the proposed position
            if (m_FollowTarget)
                proposedPos =
                        transform.parent.TransformPoint
                                (new Vector3(m_CameraTarget.transform.localPosition.x - (m_Distance * m_CamTargetDir.x),
                                             m_CameraTarget.transform.localPosition.y - (m_Distance * m_CamTargetDir.y),
                                             m_CameraTarget.transform.localPosition.z - (m_Distance * m_CamTargetDir.z)));
            else
                proposedPos = transform.parent.TransformPoint(dir * m_Distance);
        }
        else
            proposedPos = transform.position;

        // resolve the collision
        OnResolveCollision?.Invoke(this, m_Distance, m_MinDistance, m_MaxDistance, proposedPos, transform.gameObject, m_Player, m_DetectedObjects);
    }

    /**
    * Calculates the distance where the camera should be set from target position
    *@param source - camera source position
    *@param destination - target destination to reach
    *@param maxDistance - maximum distance the camera may reach from the target position
    */
    void CalculateDistance(Vector3 source, Vector3 destination, float maxDistance)
    {
        m_Distance = maxDistance;

        // detect from camera to player
        if (m_DetectionType == IEDetectionType.IE_DT_CameraToPlayer || m_DetectionType == IEDetectionType.IE_DT_Both)
        {
            // detect objects hit by the camera to player ray
            Vector3      delta = (destination - source);
            RaycastHit[] hits  = Physics.RaycastAll(source, delta.normalized, delta.magnitude, m_CameraOcclusion);

            // iterate through hit objects
            for (int i = 0; i < hits.Length; ++i)
            {
                // calculate a position closer to the target
                m_Distance = Mathf.Min(m_Distance, Mathf.Clamp(hits[i].distance * 0.9f, m_MinDistance, maxDistance));

                // add the detected object to list, if still not exists
                m_DetectedObjects.Add(hits[i].transform.gameObject);
            }
        }

        // detect from player to camera
        if (m_DetectionType == IEDetectionType.IE_DT_PlayerToCamera || m_DetectionType == IEDetectionType.IE_DT_Both)
        {
            // detect objects hit by the player to camera ray
            Vector3      delta = (source - destination);
            RaycastHit[] hits  = Physics.RaycastAll(destination, delta.normalized, delta.magnitude, m_CameraOcclusion);

            // iterate through hit objects
            for (int i = 0; i < hits.Length; ++i)
            {
                // calculate a position closer to the target
                m_Distance = Mathf.Min(m_Distance, Mathf.Clamp(hits[i].distance * 0.9f, m_MinDistance, maxDistance));

                // add the detected object to list, if still not exists
                m_DetectedObjects.Add(hits[i].transform.gameObject);
            }
        }
    }
}
