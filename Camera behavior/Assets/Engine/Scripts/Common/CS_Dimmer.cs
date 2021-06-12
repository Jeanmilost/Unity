using UnityEngine;

/**
* Provides a generic value dimmer
*@author Jean-Milost Reymond
*/
public class CS_Dimmer
{
    private float m_Value;

    /**
    * Gets or sets the fader value
    */
    public float Value
    {
        get
        {
            return m_Value;
        }

        set
        {
            m_Value = Mathf.Clamp(value, 0.0f, 1.0f);
        }
    }

    /**
    * Initializes the dimmer
    *@param value - start value (between 0.0f and 1.0f)
    */
    public void Init(float value = 0.0f)
    {
        Value = value;
    }

    /**
    * Sets the value between a range. The range doesn't need to be between 0.0f and 1.0f
    *@param value - value to set
    *@param min - min range value
    *@param max - max range value
    */
    public void Set(float value, float min, float max)
    {
        Value = (value - min) / (max - min);
    }
}
