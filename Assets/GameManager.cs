using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private int m_winScore = 9;
    [SerializeField] private PaddleController m_rightPaddle;

    public void OnEnable()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetWinScore(float value)
    {
        m_winScore = (int)value;
    }

    public void StartVS()
    {

    }
}
