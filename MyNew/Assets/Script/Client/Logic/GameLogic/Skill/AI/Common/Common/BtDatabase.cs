using System.Collections.Generic;
using UnityEngine;

public class BtDatabase //: MonoBehaviour
{
    private Dictionary<int, object> m_dicData = new Dictionary<int, object>();

    private Dictionary<string, object> m_dicStrData = new Dictionary<string, object>();


    public T GetData<T>(int dataId)
    {
        object val;
        if(m_dicData.TryGetValue(dataId, out val))
        {
            return (T)val;
        }
        return default(T);
    }

    public void SetData<T>(int id, T data)
    {
        m_dicData[id] = (object)data;
    }

    //public T GetData<T>(string dataId)
    //{
    //    object val;
    //    if(m_dicStrData.TryGetValue(dataId, out val))
    //    {
    //        return (T)val;
    //    }
    //    return default(T);
    //}

    //public void SetData<T>(string dataId, T data)
    //{
    //    m_dicStrData[dataId] = (object)data;
    //}
}

