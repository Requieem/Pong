using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FieldTweener : MonoBehaviour
{
    [Serializable]
    public struct TweenData
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }

    [SerializeField] private float m_duration = 1f;
    [SerializeField] private List<TweenData> m_tweenData = new();
    [SerializeField] private UnityEvent m_finishedMoving = new();
    [SerializeField] private UnityEvent m_finishedRotating = new();
    [SerializeField] private UnityEvent m_finishedScaling = new();

    public void Tween(int index)
    {
        if(index < 0 || index >= m_tweenData.Count)
        {
            Debug.LogError($"Index {index} is out of range");
            return;
        }

        TweenData _data = m_tweenData[index];

        var _moving = transform.DOMove(_data.Position, m_duration);
        _moving.onComplete += FinishMoving;

        var _rotating = transform.DORotate(_data.Rotation, m_duration);
        _rotating.onComplete += FinishRotating;

        var _scaling = transform.DOScale(_data.Scale, m_duration);
        _scaling.onComplete += FinishScaling;
    }

    public void FinishMoving()
    {
        m_finishedMoving.Invoke();
    }

    public void FinishRotating()
    {
        m_finishedRotating.Invoke();
    }

    public void FinishScaling()
    {
        m_finishedScaling.Invoke();
    }
}
