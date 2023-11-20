using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    [Header("Configuration")]

    [SerializeField] private InputActionAsset m_inputActionAsset;
    [SerializeField] private SharedFloat m_speed;
    [SerializeField] private SharedFloat m_ballOffset;
    [SerializeField][Range(1, 2)] private int m_playerNumber;
    [SerializeField] private bool m_invert = false;

    [Header("Initial State")]
    [SerializeField] private Ball m_ball;

    private bool HasBall => m_ball != null;
    private float BallOffset => !m_invert ? m_ballOffset.Value : -m_ballOffset.Value;
    private Vector3 BallDirection => !m_invert ? transform.right : -transform.right;
    public Ball Ball
    {
        get => m_ball;
        set
        {
            m_ball = value;
            m_ball.transform.position = transform.position + (transform.right * BallOffset);
        }
    }
    private InputAction MoveAction { get; set; }
    private InputAction StartAction { get; set; }
    private InputAction StopAction { get; set; }

    private Vector2 m_direction = Vector2.zero;
    private Coroutine m_moving;

    private void OnEnable()
    {
        var _actionMap = m_inputActionAsset.FindActionMap("Player" + m_playerNumber.ToString());
        Debug.Log("Player" + m_playerNumber.ToString());
        _actionMap.Enable();

        MoveAction = _actionMap.FindAction("Move");
        StartAction = _actionMap.FindAction("Start");
        StopAction = _actionMap.FindAction("Stop");

        MoveAction.performed += OnMove;
        StopAction.performed += OnStop;
        StartAction.performed += OnStart;
    }

    private void OnMove(InputAction.CallbackContext performed)
    {
        m_direction = performed.ReadValue<Vector2>();
        m_direction.x = 0;

        if(m_moving == null)
        {
            m_moving = StartCoroutine(DoMove());
        }
    }

    private void OnStop(InputAction.CallbackContext performed)
    {
        if(m_moving != null)
        {
            StopCoroutine(m_moving);
            m_moving = null;
            m_direction = Vector2.zero;
        }
    }

    private void OnStart(InputAction.CallbackContext performed)
    {
        if(!HasBall)
            return;

        m_ball.OnMove(BallDirection);
        m_ball = null;
    }

    private IEnumerator DoMove()
    {
        while(m_direction != Vector2.zero)
        {
            m_direction = MoveAction.ReadValue<Vector2>();
            transform.Translate(m_direction * m_speed.Value * Time.deltaTime);
            if(HasBall)
                m_ball.transform.position = transform.position + (transform.right * BallOffset);
            yield return null;
        }

        m_moving = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ball"))
        {
            var _ball = collision.gameObject.GetComponent<Ball>();
            _ball.OnMove((collision.transform.position - transform.position).normalized);
        }
    }
}
