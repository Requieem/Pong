using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Configuration")]

    [SerializeField] private SharedFloat m_speed;
    [SerializeField] private float m_speedMultiplier = 1.1f;

    private float m_currentSpeed = 0f;

    private Vector2 m_direction = Vector2.zero;
    private Coroutine m_moving;

    public Vector2 Direction => m_direction;

    private void OnEnable()
    {
        m_currentSpeed = m_speed.Value;
    }

    public void OnBounce(Vector2 normal)
    {
        m_currentSpeed *= m_speedMultiplier;
        OnMove(Vector2.Reflect(m_direction, normal));
    }

    public void OnPaddle(Vector2 direction)
    {
        m_currentSpeed *= m_speedMultiplier;
        OnMove(direction);
    }

    public void OnMove(Vector2 direction)
    {
        m_direction = direction;

        if(m_moving == null)
        {
            m_moving = StartCoroutine(DoMove());
        }
    }

    private void OnStop()
    {
        if(m_moving != null)
        {
            StopCoroutine(m_moving);
            m_moving = null;
            m_direction = Vector2.zero;
        }
    }

    private IEnumerator DoMove()
    {
        while(m_direction != Vector2.zero)
        {
            transform.Translate(m_direction * m_currentSpeed * Time.deltaTime);
            yield return null;
        }

        m_moving = null;
    }

    public void ResetState(PaddleController controller)
    {
        OnStop();
        m_currentSpeed = m_speed.Value;
        controller.Ball = this;
        controller.HasBall = true;
    }
}
