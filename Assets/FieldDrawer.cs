using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class FieldDrawer : MonoBehaviour
{
    [SerializeField][Range(3f, 10f)] private int m_sides = 4;
    [SerializeField] private float m_radius = 5f;
    [SerializeField] private float m_thickness = 0.1f;
    [SerializeField] private float m_speed = 1f;
    [SerializeField] private LineRenderer m_computeRenderer;
    [SerializeField] private LineRenderer m_backupRenderer;
    [SerializeField] private LineRenderer m_showRenderer;
    [SerializeField] private UnityEvent<int> m_sidesSet;

    private int m_lastDrawnSides = 0;
    private float m_lastDrawnRadius = 0;
    private float m_lastDrawnThickness = 0f;

    private void Update()
    {
        if(m_lastDrawnSides != m_sides || m_lastDrawnRadius != m_radius)
        {

            DrawLooped();
        }

        if(m_lastDrawnThickness != m_thickness)
        {
            m_lastDrawnThickness = m_thickness;
            m_computeRenderer.widthCurve = new AnimationCurve(new Keyframe(0, m_thickness), new Keyframe(1, m_thickness));
        }
    }

    private void DrawLooped()
    {
        Vector3[] _positions = new Vector3[m_computeRenderer.positionCount];
        int _positionCount = m_computeRenderer.positionCount;

        if(_positionCount != 0)
        {
            for(int _i = 0; _i < _positionCount; _i++)
            {
                _positions[_i] = m_computeRenderer.GetPosition(_i);
            }
            m_backupRenderer.positionCount = _positionCount;
            m_backupRenderer.useWorldSpace = false;
            m_backupRenderer.widthCurve = new AnimationCurve(new Keyframe(0, m_thickness), new Keyframe(1, m_thickness));
            m_backupRenderer.SetPositions(_positions);
        }

        m_computeRenderer.positionCount = m_sides + 1;
        m_computeRenderer.useWorldSpace = false;
        m_computeRenderer.widthCurve = new AnimationCurve(new Keyframe(0, m_thickness), new Keyframe(1, m_thickness));

        float _angle = 0f;
        _positions = new Vector3[m_sides + 1];
        for(int _i = 0; _i < m_sides + 1; _i++)
        {
            Vector3 _pos = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle), 0) * m_radius;
            _positions[_i] = _pos;
            m_computeRenderer.SetPosition(_i, _pos);
            _angle += Mathf.PI * 2f / m_sides;
            m_lastDrawnSides = m_sides;
            m_lastDrawnRadius = m_radius;
        }

        if(m_backupRenderer.positionCount == 0)
        {
            m_backupRenderer.positionCount = m_sides + 2;
            m_backupRenderer.useWorldSpace = false;
            m_backupRenderer.widthCurve = new AnimationCurve(new Keyframe(0, m_thickness), new Keyframe(1, m_thickness));
            m_backupRenderer.SetPositions(_positions);
        }

        StopAllCoroutines();
        StartCoroutine(SmoothDraw());
    }

    private IEnumerator SmoothDraw()
    {
        int _backupPositionCount = m_backupRenderer.positionCount;
        var _maxPositions = Mathf.Max(m_computeRenderer.positionCount, _backupPositionCount);

        Vector3[] _positions = new Vector3[_maxPositions];
        m_showRenderer.positionCount = _maxPositions;
        m_showRenderer.useWorldSpace = false;
        m_showRenderer.widthCurve = new AnimationCurve(new Keyframe(0, m_thickness), new Keyframe(1, m_thickness));

        for(int _i = 0; _i < _maxPositions; _i++)
        {
            Vector3 _pos;
            if(_i >= _backupPositionCount)
            {
                _pos = m_backupRenderer.GetPosition(_backupPositionCount - 1);
            }
            else
            {
                _pos = m_backupRenderer.GetPosition(_i);
            }

            _positions[_i] = _pos;
        }

        m_showRenderer.SetPositions(_positions);
        bool _toSmooth = true;
        var _maxIterations = 200;
        m_sidesSet.Invoke(m_sides - 3);

        while(_toSmooth && _maxIterations > 0)
        {
            for(int _i = 0; _i < _maxPositions; _i++)
            {
                if(_i >= m_computeRenderer.positionCount)
                {
                    _positions[_i] = _positions[_i] = Vector3.Lerp(m_showRenderer.GetPosition(_i), m_computeRenderer.GetPosition(_i - 1), m_speed * Time.deltaTime);
                }
                else
                {
                    _positions[_i] = Vector3.Lerp(m_showRenderer.GetPosition(_i), m_computeRenderer.GetPosition(_i), m_speed * Time.deltaTime);
                }

                m_showRenderer.SetPosition(_i, _positions[_i]);
            }

            _toSmooth = _positions.Take(_maxPositions).Where((pos, i) => i < m_computeRenderer.positionCount ? pos != m_computeRenderer.GetPosition(i) : pos != m_computeRenderer.GetPosition(i - 1)).Any();
            _maxIterations--;

            yield return new WaitForEndOfFrame();
        }

        m_showRenderer.positionCount = m_computeRenderer.positionCount;
        for(int _i = 0; _i < m_showRenderer.positionCount - 1; _i++)
        {
            m_showRenderer.SetPosition(_i, m_computeRenderer.GetPosition(_i));
        }

        m_showRenderer.SetPosition(m_showRenderer.positionCount - 1, m_computeRenderer.GetPosition(0));

        Debug.Log("Smoothed in " + (200 - _maxIterations) + " iterations");
    }
}