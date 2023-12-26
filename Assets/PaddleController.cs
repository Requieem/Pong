using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaddleController : MonoBehaviour
{
    [Header("Configuration")]

    [SerializeField] private string m_playerName;
    [SerializeField] private InputActionAsset m_inputActionAsset;
    [SerializeField] private SharedFloat m_speed;
    [SerializeField] private SharedFloat m_ballOffset;
    [SerializeField][Range(1, 2)] private int m_playerNumber;
    [SerializeField] private List<AudioClip> m_audioClips;
    [SerializeField] private List<AudioClip> m_shootClips;
    [SerializeField] private bool m_invert = false;
    [SerializeField] private bool m_hasBall = false;
    [SerializeField] private bool m_isAI = false;

    [Header("Initial State")]
    [SerializeField] private Ball m_ball;

    [field: SerializeField] private bool LockUp { get; set; } = false;
    [field: SerializeField] private bool LockDown { get; set; } = false;
    public bool HasBall { get => m_hasBall; set => m_hasBall = value; }
    public bool IsAI { get => m_isAI; set => m_isAI = value; }
    public string PlayerName => m_playerName;
    private bool CanStart => m_hasBall && m_ball != null;
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

        MoveAction = _actionMap.FindAction("Move");
        StartAction = _actionMap.FindAction("Start");
        StopAction = _actionMap.FindAction("Stop");

        MoveAction.performed += OnMove;
        StopAction.performed += OnStop;
        StartAction.performed += OnStart;
    }

    private void OnMove(InputAction.CallbackContext performed)
    {
        OnMove(performed.ReadValue<Vector2>());
    }

    private void OnMove(Vector2 direction)
    {
        m_direction = direction;
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
        if(!CanStart)
            return;

        m_ball.OnMove(BallDirection);
        m_hasBall = false;
        AudioPlayer.Instance.PlayRandomClip(m_shootClips);
    }

    private IEnumerator DoMove()
    {
        while(m_direction != Vector2.zero)
        {
            if(!m_isAI)
            {
                m_direction = MoveAction.ReadValue<Vector2>();
            }

            var _direction = m_direction;

            if(LockUp && _direction.y > 0)
                _direction.y = 0;
            if(LockDown && _direction.y < 0)
                _direction.y = 0;

            transform.Translate(m_speed.Value * Time.deltaTime * _direction);
            if(CanStart)
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
            _ball.OnPaddle((collision.transform.position - transform.position).normalized);
            AudioPlayer.Instance.PlayRandomClip(m_audioClips);
        }
    }

    public void ToggleLock(bool dir, bool value)
    {
        if(dir)
            LockUp = value;
        else
            LockDown = value;
    }

    private void FixedUpdate()
    {
        if(!m_isAI)
            return;

        var _direction = Ball.Direction;
        var _layerMask = LayerMask.GetMask("Border");
        var _point = Ball.transform.position;
        var _hit = Physics2D.Raycast(_point, _direction, 100f, _layerMask);
        var _collider = _hit.collider;
        var _border = _collider?.GetComponent<Border>();

        var _count = 0;

        while(_collider != null && _border != null && _count < 10)
        {
            _direction = Vector2.Reflect(_direction, Vector2.up);
            _point = _hit.point + (_hit.normal * 0.025f);

            _hit = Physics2D.Raycast(_point, _direction, 100f, _layerMask);

            _collider = _hit.collider;
            _border = _collider?.GetComponent<Border>();

            _count++;
        }

        var _intersects = LineIntersection.TryGetIntersection(_point, _direction, transform.position, transform.up, out var _intersection);

        if(_intersects)
        {
            var _nextDirection = (_intersection - transform.position).normalized;
            var _nextDistance = Vector3.Distance(_intersection, transform.position);

            _hit = Physics2D.Raycast(transform.position, _nextDirection, _nextDistance, _layerMask);

            if(_nextDistance < 0.1f || _hit.collider != null)
                _nextDirection = Vector3.zero;

            Debug.Log("AI is moving to " + _nextDirection.ToString());
            OnMove(new Vector2(_nextDirection.x, _nextDirection.y));
        }
    }

    private void OnDrawGizmos()
    {
        if(!m_isAI)
            return;

        var _direction = Ball.Direction;
        var _layerMask = LayerMask.GetMask("Border");
        var _point = Ball.transform.position;
        var _hit = Physics2D.Raycast(_point, _direction, 100f, _layerMask);
        var _collider = _hit.collider;
        var _border = _collider?.GetComponent<Border>();

        var _count = 0;

        while(_collider != null && _border != null && _count < 10)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_point, _point + (new Vector3(_direction.x, _direction.y) * _hit.distance));

            _direction = Vector2.Reflect(_direction, Vector2.up);
            _point = _hit.point + (_hit.normal * 0.025f);

            _hit = Physics2D.Raycast(_point, _direction, 100f, _layerMask);

            _collider = _hit.collider;
            _border = _collider?.GetComponent<Border>();

            _count++;
        }

        var _intersects = LineIntersection.TryGetIntersection(_point, _direction, transform.position, transform.up, out var _intersection);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (transform.up * 100f));
        Gizmos.DrawLine(transform.position, transform.position - (transform.up * 100f));

        if(_intersects)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_point, _intersection);
            var _nextDirection = (_intersection - transform.position).normalized;
            Gizmos.DrawLine(transform.position, transform.position + (_nextDirection * Vector3.Distance(_intersection, transform.position)));
            Gizmos.DrawSphere(_intersection, 0.25f);
        }
    }

}
