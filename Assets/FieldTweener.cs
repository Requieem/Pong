using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

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

    public void Tween(int index)
    {
        if(index < 0 || index >= m_tweenData.Count)
        {
            Debug.LogError($"Index {index} is out of range");
            return;
        }

        TweenData _data = m_tweenData[index];
        transform.DOMove(_data.Position, m_duration);
        transform.DORotate(_data.Rotation, m_duration);
        transform.DOScale(_data.Scale, m_duration);
    }
}
