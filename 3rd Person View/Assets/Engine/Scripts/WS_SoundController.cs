using UnityEngine;

/**
* Sound controller, contains the sounds an object may emit depending on its actions
*/
public class WS_SoundController
{
    public AudioSource[] m_AudioSources;

    /**
    * Initializes the class
    *@param audioSources - audio sources the player contains
    */
    public void Init(Component[] audioSources)
    {
        m_AudioSources = new AudioSource[audioSources.Length];

        // get the children audio sources
        for (int i = 0; i < audioSources.Length; ++i)
            // get the audio source
            m_AudioSources[i] = audioSources[i] as AudioSource;
    }

    /**
    * Checks if a sound can be played
    *@param key - the sound key to check
    *@return true if the sound can be played, otherwise false
    */
    public bool CanPlay(int key)
    {
        // is type out of bounds
        if (m_AudioSources.Length <= key)
            return false;

        // check the sound content
        return (m_AudioSources[key] != null);
    }

    /**
    * Plays a sound
    *@param key - the sound key to play
    */
    public void Play(int key)
    {
        // can play the sound?
        if (!CanPlay(key))
            return;

        // is sound already playing?
        if (m_AudioSources[key].isPlaying)
            return;

        // play the sound
        m_AudioSources[key].Play();
    }

    /**
    * Stops to play a sound
    *@param type - the sound key to play
    */
    public void Stop(int key)
    {
        // can play the sound?
        if (!CanPlay(key))
            return;

        // is sound already playing?
        if (!m_AudioSources[key].isPlaying)
            return;

        // play the sound
        m_AudioSources[key].Stop();
    }

    /**
    * Gets the sound
    *@param type - the sound to get
    *@return the sound, null if not found or on error
    */
    public AudioSource GetSound(int key)
    {
        // is type out of bounds
        if (m_AudioSources.Length <= key)
            return null;

        // play the sound
        return m_AudioSources[key];
    }
}
