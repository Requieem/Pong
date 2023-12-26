using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int m_winScore = 9;
    [SerializeField] private PaddleController m_rightPaddle;
    [SerializeField] private List<Goal> m_goals;
    [SerializeField] private GameObject m_menu;
    [SerializeField] private TextMeshProUGUI m_title;
    [SerializeField] private string m_titleText = "PONG\r\nAn Atari Classic";
    [SerializeField] private InputActionAsset m_inputActionAsset;

    public void Start()
    {
        m_goals.ForEach(goal => goal.ResetState());
        m_goals.ForEach(goal => goal.UpdateWinScore(m_winScore));
        m_menu.SetActive(true);
        m_title.text = m_titleText;
        m_inputActionAsset.Disable();
    }

    public void SetWinScore(float value)
    {
        m_winScore = (int)value;
    }

    public void StartVS()
    {
        m_rightPaddle.IsAI = false;
        DoStart();
    }

    public void StartAI()
    {
        m_rightPaddle.IsAI = true;
        DoStart();
    }

    private void DoStart()
    {
        m_goals.ForEach(goal => goal.ResetState());
        m_goals.ForEach(goal => goal.UpdateWinScore(m_winScore));
        m_menu.SetActive(false);
        m_inputActionAsset.Enable();
    }

    public void Win(PaddleController paddle)
    {
        m_inputActionAsset.Disable();
        m_title.text = paddle.PlayerName + " Wins!";
        m_menu.SetActive(true);
    }
}
