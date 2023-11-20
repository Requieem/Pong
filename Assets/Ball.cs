using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Configuration")]

    [SerializeField] private SharedFloat m_speed;

    private Vector2 m_direction = Vector2.zero;
    private Coroutine m_moving;

    public void OnBounce()
    {
        m_direction = Vector2.Reflect(m_direction, Vector2.up);
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
            transform.Translate(m_direction * m_speed.Value * Time.deltaTime);
            yield return null;
        }

        m_moving = null;
    }

    public void ResetState(PaddleController controller)
    {
        OnStop();
        controller.Ball = this;
    }
}
