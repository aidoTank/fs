using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    public struct sVOjectBaseInfo
    {
        public int m_uid;
        public int m_resId;
        public Vector3 m_pos;
        public Vector3 m_dir;  // 方向向量
        public float m_scale;
        public float m_speed;
        public float m_headHeight;
        //public eEntityType m_entType;
        public bool m_showHead;
        public int m_dieSound;
        public int m_dieEffect;
        public int m_speakId;
    }

    public struct MtBaseMoveInfo
    {
        public bool m_teleport;
        public Vector3 m_pos;
        public Vector3 m_dir;   // 单位方向
        public float m_speed;
        // 弥补最新偏差的倍率。默认是1，如果当前表现层和逻辑层的差值，小于逻辑层当前帧移动的值，则使用表现层的移动值
        public int RepairFramesMin;
        // 卡住时的当前帧，其实是设置表现层位置时最后的帧号，用于识别网络是否卡住
        public int FrameBlockIndex;

        public bool m_isBarrier;
        // 平滑分步，逻辑停止移动时，会通知表现层，但是表现层并不会立马停止，而是在下一帧停止
        public int LerpStep;
    }

    public struct sCurveParam
    {
        public Vector3 endPos;
        public float time;
        public float heigh;
    }

    public partial class VBase
    {
        public sVOjectBaseInfo m_baseInfo;

        // 人物和技能 共用实体对象移动同步，销毁，创建并不共用，比如技能施法时，资源配置的是特效id
        public int m_id;
        public Entity m_ent;
        public MtBaseMoveInfo m_moveInfo;
        public bool m_bMoveing;
        public bool m_destroy;
        // 外观是否旋转
        // 如果是普攻机枪状态，则玩家方向不会跟随移动方向改变
        public bool m_bRotate = true;
        // 通用的曲线动画
        public sCurveParam m_curveParam;

        public VBase()
        {

        }

        /// <summary>
        /// 创建模型
        /// </summary>
        public virtual void Create(sVOjectBaseInfo baseInfo)
        {
            m_baseInfo = baseInfo;
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_uid = baseInfo.m_uid;
            info.m_resID = baseInfo.m_resId;
            info.m_ilayer = (int)LusuoLayer.eEL_Dynamic;
            info.m_vPos = baseInfo.m_pos;
            info.m_vScale = Vector3.one * baseInfo.m_scale;
            if (baseInfo.m_dir != Vector3.zero)
                info.m_vRotate = Quaternion.LookRotation(baseInfo.m_dir).eulerAngles;

            eEntityType eType = eEntityType.eBattleEntity;
            if(this is VObject)
            {
                eType = eEntityType.eBattleEntity;
            }
            //else if(this is VTrigger)
            //{
            //    eType = eEntityType.eEffectEntity;
            //}

            int hid = EntityManager.Inst.CreateEntity(eType, info, CreateEnd);
            SetDir(baseInfo.m_dir);
            SetPos(baseInfo.m_pos, true);
            SetSpeed(baseInfo.m_speed);
            m_ent = EntityManager.Inst.GetEnity(hid);
        }

        public virtual void CreateEnd(Entity ent)
        {
            // 用于初始化旋转效果的角度
            m_rotateCurQua = ent.GetRotateQua();
            PlayCurve(m_curveParam);
        }

        public Entity GetEnt()
        {
            return m_ent;
        }

        public void SetScale(float val)
        {
            if (m_ent != null)
            {
                m_ent.SetScale(Vector3.one * val);
            }
        }

        public virtual void SetPos(Vector3 pos, bool isTeleport = false)
        {
            m_moveInfo.m_pos = pos;
            if (isTeleport)
            {
                if (m_ent != null && m_ent.IsInited())
                {
                    m_ent.SetPos(m_moveInfo.m_pos);
                }
            }
            else
            {
                m_moveInfo.LerpStep = 2;
            }
        }

        /// <summary>
        /// 是否外观也要旋转，默认是
        /// 用于移动时的旋转
        /// </summary>
        public virtual void SetDir(Vector3 dir)
        {
            m_moveInfo.m_dir = dir;
            if(m_bRotate && dir != Vector3.zero)
            {
                Quaternion dest = Quaternion.LookRotation(dir);
                StartRotate(dir); // 写法待优化
            }
        }

        public virtual void SetBarrier(bool isBarrier)
        {
            m_moveInfo.m_isBarrier = isBarrier;
        }

        public virtual void SetSpeed(float speed)
        {
            m_moveInfo.m_speed = speed;
        }

        public virtual void PushCommand(IFspCmdType cmd)
        {
          
        }

        /// <summary>
        /// 曲线动画
        /// 1.用于爆装备
        /// 2.技能抛物线
        /// </summary>
        public void PlayCurve(sCurveParam param)
        {
            if (param.time != 0)
            {
                //m_ent.SetCurve(m_curveParam.endPos, m_curveParam.time, m_curveParam.heigh);
                m_curveParam = param;
            }
        }


        public virtual void Update(float time, float fdTime)
        {
            if (m_ent == null || !m_ent.IsInited())
                return;

            _UpdateRotate(time, fdTime);

            if (m_bMoveing || m_moveInfo.LerpStep > 0)
            {
                Entity ent = m_ent as Entity;
                _UpdateMove(time, fdTime, ref ent, m_moveInfo);
                m_moveInfo.LerpStep -= 1;
            }
        }

        public virtual void Destory()
        {
            m_destroy = true;

            if(m_ent != null)
            {
                //m_ent.RemoveShader();
                EntityManager.Inst.RemoveEntity(m_ent.m_hid, true);
                m_ent = null;
            }
        }

        /// <summary>
        /// 1.距离差值 小于 逻辑每帧偏移量，最终位置为 表现层位置（也理解为，当按照表现层去平滑移动时，如果达不到逻辑位置，就继续按照表现层计算）
        /// 2.距离差值 小于 3倍的每帧偏移量，大于 逻辑每帧偏移量（就是要把表现层拉一点回去）
        ///     1.调节比例 = 距离差值 / 最大偏移距离
        ///     2.距离差值的方向 与 当前移动方向 做点乘
        ///     3.根据逻辑信息 计算 评估位置
        ///     4.根据 最大移动的位置 计算 评估方向
        ///     5.根据点乘值 与 距离差值 做判断
        ///         1.如果同方向表现层过快，就拉回来一点点
        ///         2.如果方向相反，就增加一点点
        ///         3.其他情况同2
        /// 3.距离差值 大于 3倍的每帧偏移量，帧率在变化的时候，就是网络正常时，直接取逻辑位置 | 网络卡主了，实体位置不变
        /// （比如在切后台，按暂停键，再切回游戏时，因为逻辑层的加速播放导致每帧逻辑位置增加非常快，如果逻辑位置和表现位置差值大，此时直接让玩家跳到逻辑位置即可）
        /// </summary>
        /// <param name="fTime"></param>
        /// <param name="fdTime"></param>
        /// <param name="ent"></param>
        /// <param name="moveInfo"></param>
        /// <returns></returns>
        public Vector3 _UpdateMove(float fTime, float fdTime, ref Entity ent, MtBaseMoveInfo moveInfo)
        {
            // 表现层每帧增加距离
            float dist = moveInfo.m_speed * fdTime;
            Vector3 dir = moveInfo.m_dir;
            Vector3 logicPos = moveInfo.m_pos;
            Vector3 curPos = ent.GetPos();
            // 表现层模拟的新位置
            Vector3 viewPos = curPos + dir * dist;

            Vector3 result = logicPos;
            if(moveInfo.m_isBarrier)   // 正常的帧同步在遇到障碍边界时，直接处理平滑，而纯单机的也走这里
            {
                Debug.Log("遇到障碍");
                Vector3 offsetMove = curPos - logicPos;
                float tempDis = offsetMove.magnitude;
                if(tempDis != 0.0f)
                {
                    result = Vector3.Lerp(curPos, logicPos, dist / tempDis);
                }
                moveInfo.RepairFramesMin = 1;
                moveInfo.FrameBlockIndex = GameManager.Inst.GetFspManager().GetCurFrameIndex();
            }
            else
            {
                // 逻辑位置和表现层位置差值向量
                Vector3 offsetVec = viewPos - logicPos;
                // 偏移向量的长度
                float offsetVecLen = offsetVec.magnitude;
                // 最大偏移量， 这个应该是不对的
                float minOffset = moveInfo.m_speed * FSPParam.clientFrameScTime;
                float maxOffset = minOffset * 3;
                if (offsetVecLen < moveInfo.RepairFramesMin * minOffset)  // （每帧表现-逻辑）小于最小的每帧偏移量时，取渲染位置，这样才能保证每一帧移动的流畅
                {
                    result = viewPos;
                    moveInfo.RepairFramesMin = 1;
                    moveInfo.FrameBlockIndex = GameManager.Inst.GetFspManager().GetCurFrameIndex();
                    //Debug.Log("1.渲染位置和逻辑位置差值小于最小帧偏移， 直接取渲染位置");
                }
                else if (offsetVecLen < maxOffset)   // （每帧表现-逻辑）大于每帧偏移量时，也需要平滑处理
                {
                    //Debug.Log("2.渲染位置和逻辑位置差值小于最大帧偏移，进行插值逼近");
                    float adjustRatio = Mathf.Clamp(offsetVecLen / maxOffset, 0.05f, 0.3f);
                    float dotValue = Vector3.Dot(offsetVec, dir);        // 计算方向是否相反，逻辑可能改变方向，但是表现层还在保持之前的
                    Vector3 estimPos = logicPos + dir * maxOffset;       // 逻辑估计的目标位置
                    Vector3 estimDir = (estimPos - curPos).normalized;   // 逻辑估计的方向

                    //Debug.Log("dotValue:" + dotValue);

                    if (dotValue > offsetVecLen * 0.707f)           // 方向一致时，减少一点点
                    {
                        result = curPos + estimDir * dist * (1.0f - adjustRatio);
                    }

                    else if (dotValue < offsetVecLen * (-0.707f))  // 方向相反时（逻辑层改变了方向），根据最新的方向，增加一点点
                    {
                        result = curPos + estimDir * dist * (1.0f + adjustRatio);
                    }
                    else
                    {
                        result = curPos + estimDir * dist * (1.0f + adjustRatio);
                    }
                    moveInfo.RepairFramesMin = 1;
                    moveInfo.FrameBlockIndex = GameManager.Inst.GetFspManager().GetCurFrameIndex();
                }
                else
                {
                    //Debug.Log("3.差值非常大时");
                    //卡住了暂时保持原位
                    if (GameManager.Inst.GetFspManager().GetCurFrameIndex() == moveInfo.FrameBlockIndex)
                    {
                        //Debug.Log("4.差值非常大时，帧率不变时，取实体本身的位置");
                        result = curPos;
                    }
                    else
                    {
                        //超出最大偏差，直接拉扯
                        moveInfo.RepairFramesMin = 1;
                    }
                }
            }
           
            // 切到后台时，网络层没卡，渲染帧卡主，但是切回来时，会获取最新逻辑位置
            //if ((this as MtCreature) != null)
            //{
            //    Debug.Log("logicPos:" + logicPos.x + "|" + logicPos.z);
            //    Debug.Log("entityPos:" + curPos.x + "|" + curPos.z);
            //}
            ent.SetPos(result);
            return result;
        }


        public Quaternion m_rotateCurQua;        // 当前
        private Quaternion m_rotateDestQua;       // 目标
        private float m_rotateTime = 0.1f;
        private float m_rotateCurTime = 0.0f;
        private bool m_bRotateing = false;

        /// <summary>
        /// 控制平滑旋转
        /// 1.移动转向时
        /// 2.释放技能时
        /// </summary>
        public void StartRotate(Vector3 dir, float rotateTime = 0.1f)
        {
            m_rotateTime = rotateTime;
            Entity ent = GetEnt();
            if (ent == null)
                return;

            if (dir == Vector3.zero)
                return;
            Quaternion dest = Quaternion.LookRotation(dir);
            //if (m_rotateCurQua == dest)
            //{
            //    return;
            //}

            m_rotateCurQua = ent.GetRotateQua();
            m_bRotateing = true;
            m_rotateDestQua = dest;
            //m_rotateTime = time;
            m_rotateCurTime = 0.0f;
        }

        private void _UpdateRotate(float fTime, float fDTime)
        {
            if (!m_bRotateing)
                return;
            m_rotateCurTime += fDTime;
            float t = m_rotateCurTime / m_rotateTime;

            t = t >= 1.0f ? 1.0f : t;
            Quaternion rot = Quaternion.Slerp(m_rotateCurQua, m_rotateDestQua, t);
            GetEnt().SetRot(rot);
            if (t >= 1.0f)
            {
                m_bRotateing = false;
            }
        }

    }

    

}

