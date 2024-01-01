using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class FieldDrawer : MonoBehaviour
{
    [Serializable]
    public struct SideDefinition
    {
        [SerializeField] private Vector3 m_start;
        [SerializeField] private Vector3 m_end;
        [SerializeField] private Transform m_transform;

        public Vector3 Start => m_transform.TransformPoint(m_start);
        public Vector3 End => m_transform.TransformPoint(m_end);
        public Vector3 Middle => (Start + End) / 2f;
        public Vector3 Normal => Vector3.Cross(End - Start, Vector3.forward).normalized;
        public float Angle => Vector3.SignedAngle(Vector3.right, Normal, Vector3.forward);
        public Quaternion Rotation => Quaternion.AngleAxis(Angle, Vector3.forward);
        public float Distance => Vector3.Distance(Start, End) * 0.95f;

        public SideDefinition(Vector3 start, Vector3 end, Transform transform)
        {
            m_start = start;
            m_end = end;
            m_transform = transform;
        }
    }

    [SerializeField] private bool m_showDebug = false;
    [SerializeField] private bool m_showComputeDebug = false;
    [SerializeField] private bool m_showBackupDebug = false;
    [SerializeField][Range(3, 10)] private int m_sides = 4;
    [SerializeField] private float m_radius = 5f;
    [SerializeField] private float m_smoothSpeed = 1f;
    [SerializeField] private float m_maxIterations = 200f;
    [SerializeField] private UnityEvent<int> m_sidesSet;
    [SerializeField] private HyperGoal[] m_goals = new HyperGoal[0];
    [SerializeField] private Vector3[] m_compute = new Vector3[0];
    [SerializeField] private Vector3[] m_backup = new Vector3[0];
    [SerializeField] private Vector3[] m_show = new Vector3[0];
    [SerializeField] private SideDefinition[] m_sidesDefinition = new SideDefinition[0];
    [SerializeField] private HyperGoal m_goalPrefab;

    private int m_lastDrawnSides = 0;
    private float m_lastDrawnRadius = 0;

    private bool m_isMoving = false;
    private bool m_isRotating = false;
    private bool m_isScaling = false;
    private bool m_isSmoothing = false;

    private void OnEnable()
    {
        m_sidesDefinition = new SideDefinition[0];
        m_goals = new HyperGoal[0];
    }

    private void OnDisable()
    {
        m_sidesDefinition = new SideDefinition[0];
        m_goals = new HyperGoal[0];
    }

    private void Update()
    {
        if(m_lastDrawnSides != m_sides || m_lastDrawnRadius != m_radius)
        {
            Draw();
        }
    }

    public void FinishMoving()
    {
        m_isMoving = false;
        SpawnIfFinished();
    }

    public void FinishRotating()
    {
        m_isRotating = false;
        SpawnIfFinished();
    }

    public void FinishScaling()
    {
        m_isScaling = false;
        SpawnIfFinished();
    }

    public void FinishSmoothing()
    {
        m_isSmoothing = false;
        SpawnIfFinished();
    }

    private void SpawnIfFinished()
    {
        if(!m_isMoving && !m_isRotating && !m_isScaling && !m_isSmoothing)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        if(Application.isPlaying)
        {
            DespawnGoals();
            m_goals = SpawnGoals(m_sidesDefinition);
        }
    }

    private void Draw()
    {
        int _length = m_compute.Length;
        Vector3[] _positions = new Vector3[_length];

        if(_length != 0)
        {
            for(int _i = 0; _i < _length; _i++)
            {
                _positions[_i] = m_compute[_i];
            }

            m_backup = _positions;
        }

        m_compute = m_compute.ChangeLength(m_sides + 1);
        float _angle = 0f;
        for(int _i = 0; _i < m_sides + 1; _i++)
        {
            Vector3 _pos = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle), 0) * m_radius;
            m_compute[_i] = _pos;
            _angle += Mathf.PI * 2f / m_sides;
            m_lastDrawnSides = m_sides;
            m_lastDrawnRadius = m_radius;
        }

        if(m_backup.Length == 0)
        {
            m_backup = m_backup.ChangeLength(m_sides + 2);
        }

        StopAllCoroutines();
        StartCoroutine(SmoothDraw());
    }

    private IEnumerator SmoothDraw()
    {
        int _backupPositionCount = m_backup.Length;
        var _maxPositions = Mathf.Max(m_compute.Length, _backupPositionCount);


        Vector3[] _positions = new Vector3[_maxPositions];
        m_show = m_show.ChangeLength(_maxPositions);

        for(int _i = 0; _i < _maxPositions; _i++)
        {
            Vector3 _pos;
            if(_i >= _backupPositionCount)
            {
                _pos = m_backup[_backupPositionCount - 1];
            }
            else
            {
                _pos = m_backup[_i];
            }

            _positions[_i] = _pos;
        }

        bool _toSmooth = true;
        var _maxIterations = m_maxIterations;
        m_show = _positions;
        m_sidesSet.Invoke(m_sides - 3);

        if(Application.isPlaying)
        {
            m_isSmoothing = true;
            m_isMoving = true;
            m_isRotating = true;
            m_isScaling = true;
            m_sidesDefinition = new SideDefinition[_maxPositions - 1];

            while(_toSmooth && _maxIterations > 0)
            {
                for(int _i = 0; _i < _maxPositions; _i++)
                {
                    if(_i >= m_compute.Length)
                    {
                        _positions[_i] = _positions[_i] = Vector3.Lerp(m_show[_i], m_compute[_i - 1], m_smoothSpeed * Time.deltaTime);
                    }
                    else
                    {
                        _positions[_i] = Vector3.Lerp(m_show[_i], m_compute[_i], m_smoothSpeed * Time.deltaTime);
                    }

                    m_show[_i] = _positions[_i];
                    ;
                }

                for(int _i = 0; _i < _maxPositions - 1; _i++)
                {
                    var _start = m_show[_i];
                    var _end = m_show[_i + 1];
                    m_sidesDefinition[_i] = new SideDefinition(_start, _end, transform);
                }

                if(Application.isPlaying && m_sidesDefinition.Length != m_goals.Length)
                {
                    m_goals = SpawnGoals(m_sidesDefinition);
                }

                for(int _i = 0; _i < m_goals.Length; _i++)
                {
                    if(_i >= m_sidesDefinition.Length)
                        m_goals[_i].Setup(m_sidesDefinition[_i - 1]);
                    else
                        m_goals[_i].Setup(m_sidesDefinition[_i]);
                }

                _toSmooth = _positions.Take(_maxPositions).Where((pos, i) => i < m_compute.Length ? pos != m_compute[i] : pos != m_compute[i - 1]).Any();
                _maxIterations--;

                yield return new WaitForEndOfFrame();
            }
        }

        m_show = m_show.ChangeLength(m_compute.Length);
        for(int _i = 0; _i < m_show.Length - 1; _i++)
        {
            m_show[_i] = m_compute[_i];
        }

        m_show[m_show.Length - 1] = m_compute[0];

        m_sidesDefinition = new SideDefinition[m_sides];
        for(int _i = 0; _i < m_sides; _i++)
        {
            var _start = m_show[_i];
            var _end = m_show[_i + 1];
            m_sidesDefinition[_i] = new SideDefinition(_start, _end, transform);
        }

        if(Application.isPlaying)
        {
            DespawnGoals();
            m_goals = SpawnGoals(m_sidesDefinition);
        }

        for(int _i = 0; _i < m_goals.Length; _i++)
        {
            m_goals[_i].Setup(m_sidesDefinition[_i]);
        }

        FinishSmoothing();
        Debug.Log("Smoothed in " + (m_maxIterations - _maxIterations) + " iterations");
    }

    private void DebugPolygon(Vector3[] arr, Color gizmoColor, bool drawAdditionalData = false)
    {
        for(int _i = 0; _i < arr.Length - 1; _i++)
        {
            Gizmos.color = gizmoColor;
            var _start = arr[_i];
            var _end = arr[_i + 1];
            var _localStart = transform.TransformPoint(_start);
            var _localEnd = transform.TransformPoint(_end);

            Gizmos.DrawLine(_localStart, _localEnd);

            if(!drawAdditionalData)
                continue;

            var _localMiddle = (_localStart + _localEnd) / 2f;
            var _localNormal = Vector3.Cross(_localEnd - _localStart, transform.forward).normalized;
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(_localStart, 0.15f);
            Gizmos.DrawSphere(_localEnd, 0.15f);
            Gizmos.DrawCube(_localMiddle, Vector3.one * 0.125f);
            Gizmos.DrawLine(_localMiddle, _localMiddle + _localNormal);
        }
    }

    private void DespawnGoals()
    {
        if(m_goals == null || m_goals.Length == 0)
            return;

        for(int _i = m_goals.Length - 1; _i >= m_sidesDefinition.Length; _i--)
        {
            Destroy(m_goals[_i].gameObject);
        }

        m_goals = m_goals.Take(m_sidesDefinition.Length).ToArray();
    }

    private HyperGoal[] SpawnGoals(SideDefinition[] sides)
    {
        HyperGoal[] _goals = sides.ToList().Where(e => !e.Equals(default)).Select((side, i) => i >= m_goals.Length ? SpawnGoal() : m_goals[i]).ToArray();

        for(int _i = 0; _i < _goals.Length; _i++)
        {
            _goals[_i].Setup(sides[_i]);
        }

        return _goals;
    }

    private HyperGoal SpawnGoal()
    {
        var _hyperGoal = Instantiate(m_goalPrefab);

        return _hyperGoal;
    }

    private void OnDrawGizmos()
    {
        if(m_show == null || m_show.Length == 0)
            return;

        if(m_showBackupDebug)
        {
            DebugPolygon(m_backup, Color.red);
        }

        if(m_showDebug)
        {
            DebugPolygon(m_show, Color.yellow, true);
        }

        if(m_showComputeDebug)
        {
            DebugPolygon(m_compute, Color.green);
        }
    }
}