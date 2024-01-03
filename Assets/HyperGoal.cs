using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HyperGoal : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_score;
    [SerializeField] private PaddleController m_paddle;
    [SerializeField] private List<AudioClip> m_audioClips;
    [SerializeField] private GameObject m_paddlePrefab;
    [SerializeField] private Sprite m_poleSprite;

    private BoxCollider2D m_collider;
    private LineRenderer m_lineRenderer;
    private SpriteRenderer m_startSprite;
    private SpriteRenderer m_endSprite;

    private bool m_lock = false;

    private void OnCollisionEnter2D(Collision2D other)
    {
        var _gameObject = other.gameObject;
        if(_gameObject.CompareTag("Ball"))
        {
            int _score = int.Parse(m_score.text) + 1;
            AudioPlayer.Instance.PlayRandomClip(m_audioClips);
            m_score.text = (int.Parse(m_score.text) + 1).ToString();
            _gameObject.GetComponent<Ball>().ResetState(m_paddle);
        }
    }

    public void Setup(FieldDrawer.SideDefinition sideDefinition)
    {
        if(m_collider == null)
            m_collider = gameObject.AddComponent<BoxCollider2D>();

        var _startColor = sideDefinition.Index > 0 ? Random.ColorHSV(valueMin: 0.35f, saturationMin: 0.2f, saturationMax: 0.3f, valueMax: 0.75f, alphaMax: 1f, alphaMin: 1f, hueMin: 0f, hueMax: 1f) : Color.white;
        var _distance = sideDefinition.Distance;
        var _rotation = sideDefinition.Rotation;
        var _zOffset = new Vector3(0f, 0f, sideDefinition.Index * 0.01f);
        var _position = sideDefinition.Middle;

        transform.SetPositionAndRotation(_position, _rotation);
        CreateSprite(out m_startSprite, m_startSprite, sideDefinition.Start - (sideDefinition.Normal * 0.3f) + _zOffset, _startColor);
        CreateSprite(out m_endSprite, m_endSprite, sideDefinition.End - (sideDefinition.Normal * 0.3f) + _zOffset, _startColor);
        CreateCollider(out m_collider, m_collider, new Vector2(0.1f, _distance), new Vector2(-0.3f, 0));
        CreateLineRenderer(out m_lineRenderer, m_lineRenderer, new Vector3(-0.3f, -_distance / 2f, 0), new Vector3(-0.3f, _distance / 2f, 0), _startColor);

        if(m_lock)
            return;

        CreatePaddle(out m_paddle, m_paddle, _startColor);
    }

    private void CreateCollider(out BoxCollider2D assigned, BoxCollider2D original, Vector2 size, Vector2 offset)
    {
        assigned = original;

        if(assigned == null)
        {
            assigned = gameObject.AddComponent<BoxCollider2D>();
        }

        assigned.size = size;
        assigned.offset = offset;
    }

    private void CreateLineRenderer(out LineRenderer assigned, LineRenderer original, Vector3 start, Vector3 end, Color color)
    {
        assigned = original;

        if(assigned == null)
        {
            assigned = gameObject.AddComponent<LineRenderer>();
            assigned.material = new Material(Shader.Find("Sprites/Default"));
            assigned.useWorldSpace = false;
            assigned.positionCount = 2;
            assigned.startWidth = 0.1f;
            assigned.endWidth = 0.1f;
            assigned.colorGradient = new Gradient()
            {
                colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(color, 0),
                    new GradientColorKey(color, 1)
                }
            };
        }

        assigned.SetPositions(new Vector3[] { start, end });
    }

    private void CreateSprite(out SpriteRenderer assigned, SpriteRenderer original, Vector3 position, Color color)
    {
        assigned = original;

        if(assigned == null)
        {
            assigned = new GameObject("Sprite").AddComponent<SpriteRenderer>();
            assigned.sprite = m_poleSprite;
            assigned.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
            assigned.gameObject.AddComponent<CircleCollider2D>();
            assigned.color = color;
        }

        assigned.transform.position = position;
    }

    private void CreatePaddle(out PaddleController assigned, PaddleController original, Color color)
    {
        assigned = original;

        if(assigned == null)
        {
            assigned = Instantiate(m_paddlePrefab, transform.position, transform.rotation).GetComponent<PaddleController>();
            assigned.transform.SetParent(transform);
            assigned.GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void RemovePaddle(bool doLock = false)
    {
        if(m_paddle != null)
            Destroy(m_paddle.gameObject);

        m_lock = doLock;
        m_paddle = null;
    }

    public void RemoveSprite()
    {
        if(m_startSprite != null)
            Destroy(m_startSprite.gameObject);
        m_startSprite = null;

        if(m_endSprite != null)
            Destroy(m_endSprite.gameObject);
        m_endSprite = null;
    }

    public void RestorePaddle()
    {
        if(m_paddle == null)
        {
            m_paddle = Instantiate(m_paddlePrefab, transform.position, transform.rotation).GetComponent<PaddleController>();
            m_paddle.transform.SetParent(transform);
            m_paddle.GetComponent<SpriteRenderer>().color = m_startSprite.color;
            m_lock = false;
        }
    }

    public void OnDestroy()
    {
        RemovePaddle();
        RemoveSprite();
    }
}
