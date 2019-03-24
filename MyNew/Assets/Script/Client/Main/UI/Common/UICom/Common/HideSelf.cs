using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    public class HideSelf : MonoBehaviour
    {

        public float m_time;
        public bool isShow = false;
        private void Update()
        {
            if (isShow)
            {
                m_time -= Time.deltaTime;
                if (m_time < 0)
                {
                    isShow = false;
                    gameObject.SetActiveNew(false);
                }
            }

        }
    }
}