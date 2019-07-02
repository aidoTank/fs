using System.Collections.Generic;
using UnityEngine;
using System;

namespace Roma
{
    public enum eColliderType
    {
        None,
        Circle,
        Polygon,
    }

    /// <summary>
    /// �߼�����ײ��
    /// </summary>
    public class Collider
    {
        public eColliderType type;
        public Vector2d c;
        public bool active = true;
        public bool isObstacle = true;   // �Ƿ����ϰ�
        public bool notPush = false;

        public Action<Vector2d, Vector2d> m_updatePosEvent;

        public eColliderType GetColliderType()
        {
            return type;
        }

        public void AddOffsetPos(Vector2d oPos, Vector2d dir)
        {
            c += oPos;
            if (m_updatePosEvent != null)
                m_updatePosEvent(c, dir);
        }

        /// <summary>
        /// ��Ϊ���ϲ��߼�����������Ч����߷ǳ���
        /// </summary>
        public virtual void Update()
        {
        }

        public virtual void Draw()
        {

        }
    }

    public class Polygon : Collider
    {
        public bool bAirWall = true;   // �Ƿ��ǿ���ǽ�ϰ�
        public List<Vector2d> m_edgesList = new List<Vector2d>();    // ���б�
        public List<Vector2d> m_worldPosList = new List<Vector2d>(); // �����б�

        public Polygon()
        {
            type = eColliderType.Polygon;
        }

        /// <summary>
        /// ���붥���б�
        /// </summary>
        public void Init(Vector2d[] point)
        {
            for (int i = 0; i < point.Length; i++)
            {
                m_worldPosList.Add(point[i]);
            }
            InitDdgesVecList();
        }

        /// <summary>
        /// ��ʼ�����б�
        /// </summary>
        public void InitDdgesVecList()
        {
            for (int i = 0; i < m_worldPosList.Count; i++)
            {
                int pointIndex1 = i;
                int pointIndex2 = (i + 1) % m_worldPosList.Count;
                Vector2d vec = m_worldPosList[pointIndex2] - m_worldPosList[pointIndex1];
                m_edgesList.Add(vec);
            }
        }
    }

    public class Circle : Collider
    {
        public FixedPoint r;

        public Circle()
        {
            type = eColliderType.Circle;
        }

        public override void Update()
        {
            PhysicsManager.Inst.MoveCheck(this);
        }

#if UNITY_EDITOR
        public override void Draw()
        {
            base.Draw();
            Gizmos.DrawSphere(c.ToVector3(), r.value);
        }
#endif
    }

    /// <summary>
    /// ��������������100����ʱ�����ͨ����ײ���������������2��forѭ�����������ײ��CPU��ʱ��4MS
    /// �����ϲ�����AOI����ʱֻ��Ҫ���Ǹ�������������⣬��15���ң�ÿ��ȥ����һ�Σ���ʱ��0.4MS
    /// </summary>
    public class PhysicsManager : Singleton
    {
        public static PhysicsManager Inst = null;

        public List<Collider> m_list = new List<Collider>();
        public bool m_rolePush = false;

        public PhysicsManager()
            : base(true)
        {
        }

        public override void Init()
        {

        }

        public void SetRolePush(bool bPush)
        {
            m_rolePush = bPush;
        }


        /// <summary>
        /// �Ƿ��ϰ���������ֹ���
        /// </summary>
        public bool Isblock(int x, int y)
        {
            Circle cir = new Circle();
            cir.c = new Vector2d(x, y);
            cir.r = FixedPoint.one;
            for (int j = 0; j < m_list.Count; j++)
            {
                Collider col = m_list[j];
                if (col is Polygon)
                {
                    Polygon pol = col as Polygon;
                    if (CheckPolygonAndCircle(pol, cir, false))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Add(Collider col)
        {
            if (!m_list.Contains(col))
            {
                m_list.Add(col);
            }
        }

        public void Remove(Collider col)
        {
            if (m_list.Contains(col))
            {
                m_list.Remove(col);
                col = null;
            }
        }

        /// <summary>
        ///  ����Ҫ�ƶ����������
        /// </summary>
        public void MoveCheck(Circle cir)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                MoveCheck(cir, m_list[i]);
            }
        }

        public void OnDrawGizmos()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].Draw();
            }
        }

        public FixedPoint CircleCircleDis2 = new FixedPoint(4);
        public FixedPoint CirclePolygonDis2 = new FixedPoint(6400);
        private FixedPoint m_dis2;
        /// <summary>
        ///  ͨ�����룬���Ż�
        /// </summary>
        public void MoveCheck(Collider a, Collider b)
        {
            if (!a.active || !b.active)
                return;
            if (a == b)
                return;
            if (a is Polygon && b is Polygon)     // �ϰ�֮�䲻���
                return;

            if (!m_rolePush)                      // ��ɫ����ײʱ������Բʱ��������
            {
                if (a is Circle && b is Circle)
                {
                    return;
                }
            }
            else
            {
                if (a is Circle && b is Circle)   // �����ɫ��Ҫ��ײ�����Ҷ���Բ����Զʱ��������
                {
                    FPCollide.GetDis2(ref m_dis2, ref a.c, ref b.c);
                    if (m_dis2 > CircleCircleDis2)
                    {
                        return;
                    }
                }
            }

            if (a is Circle && b is Polygon)      // ��ɫ���ϰ�̫Զ������
            {
                FPCollide.GetDis2(ref m_dis2, ref a.c, ref b.c);
                if (m_dis2 > CirclePolygonDis2)
                {
                    return;
                }
            }

            eColliderType type1 = a.GetColliderType();
            eColliderType type2 = b.GetColliderType();
            switch (type1)
            {
                case eColliderType.Circle:
                    switch (type2)
                    {
                        case eColliderType.Circle:
                            CheckCircleAndCircle(a as Circle, b as Circle);
                            break;
                        case eColliderType.Polygon:
                            CheckPolygonAndCircle(b as Polygon, a as Circle);
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// 1.��ȡa��b�����ķ���
        /// 2.��ȡa�ڴ˷����ϵ�ͶӰ��
        /// 3.��ȡa����ΪԲ�뾶�ľ��룬��ɵ�ֱ�ߣ�Ϊa��ͶӰ��
        /// 4.��ȡb��ͶӰ��
        /// 5.�Ա�2��ͶӰ���Ƿ��ཻ
        /// 6.����ཻ����ȡa�Ƿ�����ߣ��Լ��ཻ�ľ���
        /// 7.����ab����ƫ����������֪������ƫ������
        /// 8.���ݾ���ƫ���������Ƿ�a����ߣ��������ƿ��ľ���
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool CheckCircleAndCircle(Circle a, Circle b, bool bPush = true)
        {
            Vector2d axis = a.c - b.c;
            axis.Normalize();

            FixedPoint projPoint = Vector2d.Dot(a.c, axis);
            FixedPoint min = projPoint - a.r;
            FixedPoint max = projPoint + a.r;
            Vector2d projA = new Vector2d(min, max);

            projPoint = Vector2d.Dot(b.c, axis);
            min = projPoint - b.r;
            max = projPoint + b.r;
            Vector2d projB = new Vector2d(min, max);

            if (CheckLine(projA, projB))
            {
                if (bPush)
                {
                    bool bAtLeft = false;
                    FixedPoint offset = FixedPoint.zero;
                    SetPushVec(projA, projB, ref bAtLeft, ref offset);
                    Push(a, b, offset, axis, bAtLeft);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 1.��ȡ���е����������
        /// 2.��ȡÿ���ߵ�����
        /// 3.����ÿ���ߵ���������ȡ���Ĵ�ֱ������Ҳ����ͶӰ��
        /// 4.ͨ����ǰ����ε�ĳһ�ߵ�ͶӰ�ᣬ����Բ�����ͶӰ���ϵ�ͶӰ�߶�
        /// 5.ͨ����ǰ����ε�ĳһ�ߵ�ͶӰ�ᣬ���㵱ǰ���������ͶӰ���ϵ��߶Σ��������е㣬ȡ��С�����
        /// 6.�Աȶ����ͶӰ�߶Σ���ԲͶӰ�߶��Ƿ��ཻ�����һ���ߵĲ��ཻ����ײ���˳�ѭ��
        /// 7.����ཻ����ȡa�Ƿ�����ߣ��Լ��ཻ�ľ���
        /// 8.����ѭ����һ����������ȡ��С���ཻ���룬��Ϊ��С���ཻ�����������ཻ����
        /// </summary>
        /// <returns>�Ƿ��ཻ</returns>
        private bool CheckPolygonAndCircle(Polygon a, Circle b, bool bPush = true)
        {
            bool bInit = false;
            bool bAtLeft = true;
            Vector2d offsetVec = Vector2d.zero;
            FixedPoint offsetLen = FixedPoint.zero;

            List<Vector2d> wPosList = a.m_worldPosList;
            List<Vector2d> edgesVecList = a.m_edgesList;

            for (int i = 0; i < edgesVecList.Count; i++)
            {
                Vector2d axis = edgesVecList[i];
                // ��ȡͶӰ��
                axis = GetN(axis);
                axis.Normalize();

                // ��ȡ��ǰͶӰ���ϣ�����ε�ͶӰ��
                Vector2d curPoint = wPosList[0];
                FixedPoint minProj = Vector2d.Dot(curPoint, axis);
                FixedPoint maxPorj = minProj;
                for (int p = 1; p < wPosList.Count; p++)
                {
                    curPoint = wPosList[p];
                    FixedPoint curPorj = Vector2d.Dot(curPoint, axis);
                    if (curPorj < minProj)
                        minProj = curPorj;
                    if (curPorj > maxPorj)
                        maxPorj = curPorj;
                }
                Vector2d projA = new Vector2d(minProj, maxPorj);

                // ��Բ��ͶӰ�����
                FixedPoint projPont = Vector2d.Dot(b.c, axis);
                FixedPoint min = projPont - b.r;
                FixedPoint max = projPont + b.r;
                Vector2d projB = new Vector2d(min, max);

                if (!CheckLine(projA, projB))  // ����һ��ʱ�����е��ͶӰ���ཻ�����뽻
                {
                    return false;
                }
                else
                {
                    if (bPush)
                    {
                        bool bAtLeftTemp = false;
                        FixedPoint offsetLenTemp = FixedPoint.zero;
                        SetPushVec(projA, projB, ref bAtLeftTemp, ref offsetLenTemp);

                        if (!bInit)
                        {
                            bAtLeft = bAtLeftTemp;
                            offsetLen = offsetLenTemp;
                            offsetVec = axis;
                            bInit = true;
                        }
                        else
                        {
                            // ������ǵ�һ�Σ���ô�͵û�ȡ��С���ཻ����
                            if (offsetLenTemp < offsetLen)
                            {
                                bAtLeft = bAtLeftTemp;
                                offsetLen = offsetLenTemp;
                                offsetVec = axis;
                            }
                        }
                    }
                }
            }
            if (bPush)
            {
                Push(a, b, offsetLen, offsetVec, bAtLeft);
            }
            return true;
        }

        /// <summary>
        /// ������ͨ�� x1x2+y1y2 = 0 �ɵã�(-B,A) �� (B,-A)��
        /// </summary>
        private Vector2d GetN(Vector2d vec)
        {
            if (vec.y == FixedPoint.zero)
            {
                return new Vector2d(0, 1);
            }
            return new Vector2d(-vec.y, vec.x);
        }

        /// <summary>
        /// �Ƿ��ཻ
        /// </summary>
        private bool CheckLine(Vector2d a, Vector2d b)
        {
            if (a.y < b.x || a.x > b.y)
            {
                return false;
            }
            return true;
        }

        public void SetPushVec(Vector2d projA, Vector2d projB, ref bool bAtLeft, ref FixedPoint offsetLen)
        {
            if (projA.x == projB.x)
            {
                if (projA.y <= projB.y)
                {
                    bAtLeft = true;
                    offsetLen = projA.y - projA.x;
                }
                else
                {
                    bAtLeft = false;
                    offsetLen = projB.y - projA.x;
                }
            }
            else if (projA.x < projB.x)
            {
                bAtLeft = true;
                offsetLen = projA.y - projB.x;
            }
            else
            {
                bAtLeft = false;
                offsetLen = projB.y - projA.x;
            }
        }

        public void Push(Collider a, Collider b, FixedPoint offsetLen, Vector2d offsetVect, bool bAtLeft)
        {
            Vector2d offsetPos = offsetVect * offsetLen;
            //Debug.Log("offsetPos:" + offsetPos);
            if (m_rolePush)
            {
                if (!a.isObstacle && !b.isObstacle)
                {
                    if (a.notPush && !b.notPush)
                    {
                        if (bAtLeft)
                        {
                            b.AddOffsetPos(offsetPos, offsetVect);
                        }
                        else
                        {
                            b.AddOffsetPos(-offsetPos, offsetVect);
                        }
                    }
                    else if (b.notPush && !a.notPush)
                    {
                        if (bAtLeft)
                        {
                            a.AddOffsetPos(-offsetPos, offsetVect);
                        }
                        else
                        {
                            a.AddOffsetPos(offsetPos, offsetVect);
                        }
                    }
                    else if (!b.notPush && !a.notPush)
                    {
                        if (bAtLeft)
                        {
                            a.AddOffsetPos(-offsetPos * new FixedPoint(0.5f), -offsetVect);
                            b.AddOffsetPos(offsetPos * new FixedPoint(0.5f), offsetVect);
                        }
                        else
                        {
                            a.AddOffsetPos(offsetPos * new FixedPoint(0.5f), offsetVect);
                            b.AddOffsetPos(-offsetPos * new FixedPoint(0.5f), -offsetVect);
                        }
                    }
                }
            }

            // A���ϰ�
            if (a.isObstacle && !b.isObstacle)
            {
                if (bAtLeft)
                {
                    b.AddOffsetPos(offsetPos, offsetVect);
                }
                else
                {
                    b.AddOffsetPos(-offsetPos, offsetVect);
                }
            }
            else if (!a.isObstacle && b.isObstacle)
            {
                if (bAtLeft)
                {
                    a.AddOffsetPos(-offsetPos, offsetVect);
                }
                else
                {
                    a.AddOffsetPos(offsetPos, offsetVect);
                }
            }
        }
    }
}