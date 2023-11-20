using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    [SerializeField] private bool m_isTop = false;
    [SerializeField] private List<AudioClip> m_audioClips;

    public bool IsTop => m_isTop;

    private void OnCollisionEnter2D(Collision2D other)
    {
        var _gameObject = other.gameObject;
        if(_gameObject.CompareTag("Ball"))
        {
            _gameObject.GetComponent<Ball>().OnBounce(other.GetContact(0).normal);
            AudioPlayer.Instance.PlayRandomClip(m_audioClips);
        }

        if(_gameObject.CompareTag("Paddle"))
        {
            _gameObject.GetComponent<PaddleController>().ToggleLock(m_isTop, true);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        var _gameObject = other.gameObject;
        if(_gameObject.CompareTag("Paddle"))
        {
            _gameObject.GetComponent<PaddleController>().ToggleLock(m_isTop, false);
        }
    }
}
