using UnityEngine;

/**
* This class manages the opening door interlude events
* @author Jean-Milost Reymond
*/
public class GS_CameraAnimEvents : MonoBehaviour
{
    #region Public delegates

    /**
    * Called when walking animation is starting
    *@param sender - event sender
    */
    public delegate void OnStartWalkEvent(object sender);

    /**
    * Called when the door is reached
    *@param sender - event sender
    */
    public delegate void OnDoorReachedEvent(object sender);

    /**
    * Called when door is opened
    *@param sender - event sender
    */
    public delegate void OnDoorOpenedEvent(object sender);

    /**
    * Called when walking animation is stopped
    *@param sender - event sender
    */
    public delegate void OnStopWalkEvent(object sender);

    #endregion

    #region Public properties

    /**
    * Gets or sets the OnStartWalk event
    */
    public OnStartWalkEvent OnStartWalk { get; set; }

    /**
    * Gets or sets the OnDoorReached event
    */
    public OnDoorReachedEvent OnDoorReached { get; set; }

    /**
    * Gets or sets the OnDoorOpened event
    */
    public OnDoorOpenedEvent OnDoorOpened { get; set; }

    /**
    * Gets or sets the OnStopWalk event
    */
    public OnStopWalkEvent OnStopWalk { get; set; }

    #endregion

    #region Public functions

    /**
    * Called when walking animation is starting
    */
    public void StartWalk()
    {
        OnStartWalk?.Invoke(this);
    }

    /**
    * Called when the door is reached
    */
    public void DoorReached()
    {
        OnDoorReached?.Invoke(this);
    }

    /**
    * Called when the door is opened
    */
    public void DoorOpened()
    {
        OnDoorOpened?.Invoke(this);
    }

    /**
    * Called when walking animation is stopped
    */
    public void StopWalk()
    {
        OnStopWalk?.Invoke(this);
    }

    #endregion
}
