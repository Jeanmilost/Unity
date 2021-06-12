using UnityEngine;

/**
* Provides a kick animation controller
*@author Jean-Milost Reymond
*/
public class WS_KickAnimController : MonoBehaviour
{
    /**
    * Called when the player kicks the ball
    *@param sender - event sender
    */
    public delegate void OnKickBallEvent(object sender);

    /**
    * Called when the kicks animation ends
    *@param sender - event sender
    */
    public delegate void OnKickEndEvent(object sender);

    /**
    * Gets or sets the OnKickBall event
    */
    public OnKickBallEvent OnKickBall { get; set; }

    /**
    * Gets or sets the OnKickEnd event
    */
    public OnKickBallEvent OnKickEnd { get; set; }

    /**
    * Called when the player kicks the ball
    */
    public void KickBall()
    {
        OnKickBall?.Invoke(this);
    }

    /**
    * Called when the player kick animation ends
    */
    public void KickEnd()
    {
        OnKickEnd?.Invoke(this);
    }
}
