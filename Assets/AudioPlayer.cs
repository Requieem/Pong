using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance;

    public void OnEnable()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    [SerializeField] private AudioSource m_audioSource;
    public void PlayRandomClip(List<AudioClip> clips, float volume = 1)
    {
        var _clip = clips[Random.Range(0, clips.Count)];
        m_audioSource.PlayOneShot(_clip, volume);
    }
}
