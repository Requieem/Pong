using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Goal : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_score;
    [SerializeField] private PaddleController m_anchorPaddle;
    [SerializeField] private PaddleController m_ownerPaddle;
    [SerializeField] private List<AudioClip> m_audioClips;
    [SerializeField] private int m_winScore = 9;
    [SerializeField] private UnityEvent<PaddleController> m_onWin;

    // Start is called before the first frame update
    private void Start()
    {
        ResetState();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var _gameObject = other.gameObject;
        if(_gameObject.CompareTag("Ball"))
        {
            int _score = int.Parse(m_score.text) + 1;
            AudioPlayer.Instance.PlayRandomClip(m_audioClips);
            m_score.text = (int.Parse(m_score.text) + 1).ToString();
            _gameObject.GetComponent<Ball>().ResetState(m_anchorPaddle);

            if(_score >= m_winScore)
            {
                m_onWin.Invoke(m_ownerPaddle);
            }
        }
    }

    public void ResetState()
    {
        m_score.text = "0";
    }

    public void UpdateWinScore(int winScore)
    {
        m_winScore = winScore;
    }
}
