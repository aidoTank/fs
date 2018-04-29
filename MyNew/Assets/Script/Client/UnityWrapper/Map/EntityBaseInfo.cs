using UnityEngine;
using System.Collections;

namespace Roma
{
    public class EntityBaseInfo
    {
        public int m_resID;
        public string m_strName;
        public Vector3 m_vPos = Vector3.zero;
        public Vector3 m_vRotate = Vector3.zero;
        public Vector3 m_vScale = Vector3.one;
        public bool m_bStatic = false;
        // 子材质球个数
        public int m_lightMapIndexNum = 0;
        public int[] m_lightMapIndex;
        public Vector4[] m_lightMapScaleOffset;
        // 3D环境音效个数
        public int m_envSoundNum = 0;
        public int[] m_envSoundResId;

        // 游戏逻辑加载时，需要设置的参数
        public bool m_bPlayer = false; // 是否是真是玩家
        public string m_defaultIdleAnima = AnimationInfo.m_animaStand;  // 默认待机动作
        public int m_ilayer = (int)LusuoLayer.eEL_Default;
        public int m_order = 0;
        public bool m_active = true;
        public EntityInitNotify m_initNotify;

        public void Save(ref LusuoStream ls)
        {
            //Debug.Log("模型自身保存数据");
            ls.WriteInt(m_resID);
            ls.WriteString(m_strName);
            ls.WriteVector3(m_vPos.x, m_vPos.y, m_vPos.z);
            ls.WriteVector3(m_vRotate.x, m_vRotate.y, m_vRotate.z);
            ls.WriteVector3(m_vScale.x, m_vScale.y, m_vScale.z);
            ls.WriteBool(m_bStatic);

            // 读取子模型个数，遍历写入个数，索引，和偏移
            ls.WriteInt(m_lightMapIndexNum);
            for (int i = 0; i < m_lightMapIndexNum; i ++ )
            {
                ls.WriteInt(m_lightMapIndex[i]);
            }
            for (int i = 0; i < m_lightMapIndexNum; i++)
            {
                ls.WriteVector4(ref m_lightMapScaleOffset[i]);
            }
            // 写入环境音效
            ls.WriteInt(m_envSoundNum);
            for (int i = 0; i < m_envSoundNum; i++)
            {
                ls.WriteInt(m_envSoundResId[i]);
            }
        }

        public void Load(LusuoStream ls)
        {
            //Debug.Log("模型自身读取数据");
            m_resID = ls.ReadInt();
            ls.ReadString(out m_strName);
            ls.ReadVector3(ref m_vPos);
            ls.ReadVector3(ref m_vRotate);
            ls.ReadVector3(ref m_vScale);
            ls.ReadBool(ref m_bStatic);
            // 读取光照图
            ls.ReadInt(ref m_lightMapIndexNum);
            m_lightMapIndex = new int[m_lightMapIndexNum];
            m_lightMapScaleOffset = new Vector4[m_lightMapIndexNum];
            for (int i = 0; i < m_lightMapIndexNum; i++)
            {
                ls.ReadInt(ref m_lightMapIndex[i]);
            }
            for (int i = 0; i < m_lightMapIndexNum; i++)
            {
                ls.ReadVector4(ref m_lightMapScaleOffset[i]);
            }
            // 读取环境音效
            ls.ReadInt(ref m_envSoundNum);
            m_envSoundResId = new int[m_envSoundNum];
            for(int i =0; i < m_envSoundNum; i ++)
            {
                ls.ReadInt(ref m_envSoundResId[i]);
            }
        }
    }
}
