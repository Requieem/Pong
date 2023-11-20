using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_score;
    [SerializeField] private PaddleController m_paddle;
    [SerializeField] private List<AudioClip> m_audioClips;

    // Start is called before the first frame update
    private void Start()
    {
        m_score.text = "0";
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var _gameObject = other.gameObject;
        if(_gameObject.CompareTag("Ball"))
        {
            m_score.text = (int.Parse(m_score.text) + 1).ToString();
            _gameObject.GetComponent<Ball>().ResetState(m_paddle);
            AudioPlayer.Instance.PlayRandomClip(m_audioClips);
        }
    }
}
