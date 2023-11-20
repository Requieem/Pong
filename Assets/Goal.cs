using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_score;
    [SerializeField] private PaddleController m_paddle;

    // Start is called before the first frame update
    private void Start()
    {
        m_score.text = "0";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Ball"))
        {
            m_score.text = (int.Parse(m_score.text) + 1).ToString();
            other.GetComponent<Ball>().ResetState(m_paddle);
        }
    }
}
