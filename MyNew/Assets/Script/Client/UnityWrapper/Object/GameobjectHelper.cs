using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace Roma
{
    public class GameObjectHelper : MonoBehaviour
    {
        void OnWillRenderObject()
        {
            if (m_HelpObject != null)
            {
                // 得到最后一次渲染的帧
                //m_HelpObject.WillDraw();
            }
        }

        public void SetHelpObject(Entity entity)
        {
            m_HelpObject = entity;
        }

        public Entity GetHelpObject()
        {
            return m_HelpObject;
        }

        public void AsyncCall(IEnumerator routine)
        {
            StartCoroutine(routine);
        }

        private Entity m_HelpObject = null;

    }
}
