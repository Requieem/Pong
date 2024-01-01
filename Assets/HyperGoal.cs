using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HyperGoal : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_score;
    [SerializeField] private PaddleController m_paddle;
    [SerializeField] private List<AudioClip> m_audioClips;
    [SerializeField] private GameObject m_paddlePrefab;
    [SerializeField] private BoxCollider2D m_collider;
    [SerializeField] private LineRenderer m_lineRenderer;

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

        if(m_lineRenderer == null)
        {
            m_lineRenderer = gameObject.AddComponent<LineRenderer>();
            m_lineRenderer.colorGradient = new Gradient()
            {
                colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(Random.ColorHSV(), 0),
                    new GradientColorKey(Random.ColorHSV(), 1)
                }
            };

            m_lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            m_lineRenderer.useWorldSpace = false;
            m_lineRenderer.positionCount = 2;
            m_lineRenderer.startWidth = 0.1f;
            m_lineRenderer.endWidth = 0.1f;
        }

        var _position = sideDefinition.Middle;
        transform.position = _position;

        var _rotation = sideDefinition.Rotation;
        transform.rotation = _rotation;

        var _distance = sideDefinition.Distance;

        m_collider.size = new Vector2(0.3f, _distance);
        m_collider.offset = new Vector2(-0.3f, 0);
        m_lineRenderer.SetPositions(new Vector3[] { new Vector3(-0.3f, -_distance / 2f, 0), new Vector3(-0.3f, _distance / 2f, 0) });

        if(m_paddle == null)
        {
            m_paddle = Instantiate(m_paddlePrefab, _position, _rotation).GetComponent<PaddleController>();
            m_paddle.transform.SetParent(transform);
        }
    }

    public void OnDestroy()
    {
        if(m_paddle != null)
            Destroy(m_paddle.gameObject);
    }
}
