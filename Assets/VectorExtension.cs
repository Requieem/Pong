using UnityEngine;

public static class VectorExtension
{
    public static Vector3[] ChangeLength(this Vector3[] vec, int newLength)
    {
        if(newLength < 0)
        {
            Debug.LogError($"New length {newLength} is out of range");
            return vec;
        }

        if(newLength == vec.Length)
        {
            return vec;
        }

        if(newLength < vec.Length)
        {
            var _newVec = new Vector3[newLength];
            for(int _i = 0; _i < newLength; _i++)
            {
                _newVec[_i] = vec[_i];
            }
            vec = _newVec;
        }
        else
        {
            var _newVec = new Vector3[newLength];
            vec.CopyTo(_newVec, 0);
            vec = _newVec;
        }

        return vec;
    }
}