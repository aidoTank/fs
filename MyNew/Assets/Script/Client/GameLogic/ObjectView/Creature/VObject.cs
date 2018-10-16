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
        public int m_resId;
        public Vector3 m_pos;
        public Vector3 m_dir;  // 方向向量
    }

    public partial class VObject
    {
        // 人物和技能 共用实体对象创建，移动同步，销毁
        public int m_hid;
        private BoneEntity m_ent;
        private MtBaseMoveInfo m_moveInfo = new MtBaseMoveInfo();
        public bool m_bMoveing;


        public bool m_bMaster;
        public CmdFspEnum m_state;
        public CThingHead m_head;

        public VObject()
        {

        }

        
        /// <summary>
        /// 创建模型
        /// </summary>
        public virtual void Create(sVOjectBaseInfo baseInfo)
        {
            m_head = new CThingHead("1111");

            EntityBaseInfo info = new EntityBaseInfo();
            info.m_resID = baseInfo.m_resId;
            info.m_ilayer = (int)LusuoLayer.eEL_Dynamic;
            info.m_vPos = baseInfo.m_pos;
            info.m_vRotate = Quaternion.LookRotation(baseInfo.m_dir).eulerAngles;
            m_hid = EntityManager.Inst.CreateEntity(eEntityType.eBoneEntity, info, (ent)=> 
            {
                if(m_bMaster)
                {
                    CameraMgr.Inst.InitCamera(this);
                }
                PushCommand(new CmdFspStopMove());
            });
            m_ent = (BoneEntity)EntityManager.Inst.GetEnity(m_hid);
        }

        public BoneEntity GetEnt()
        {
            return m_ent;
        }

        public virtual void SetPos(Vector3 pos)
        {
            m_moveInfo.m_pos = pos;
        }

        public virtual void SetDir(Vector3 dir)
        {
            m_moveInfo.m_dir = dir;
        }
        

        public virtual void PushCommand(IFspCmdType cmd)
        {
            //Debug.Log("切换：" + cmd.GetCmdType());
            if(cmd.GetCmdType() == CmdFspEnum.eFspRotation)
            {
                CmdFspRotation rota = cmd as CmdFspRotation;
                StartRotate(rota.m_rotation, 0.1f);
            }
            else if(cmd.GetCmdType() == CmdFspEnum.eFspStopMove)
            {
                m_state = cmd.GetCmdType();
                m_bMoveing = false;
                AnimationAction animaInfo = new AnimationAction();
                animaInfo.crossTime = AnimationInfo.m_crossTime;
                animaInfo.playSpeed = 1;
                animaInfo.strFull = "stand";
                animaInfo.eMode = WrapMode.Loop;
                m_ent.Play(animaInfo);
            }
            else if(cmd.GetCmdType() == CmdFspEnum.eFspMove)
            {
                m_state = cmd.GetCmdType();
                m_bMoveing = true;
                AnimationAction animaInfo = new AnimationAction();
                animaInfo.crossTime = AnimationInfo.m_crossTime;
                animaInfo.playSpeed = 1;
                animaInfo.strFull = "run";
                animaInfo.eMode = WrapMode.Loop;
                m_ent.Play(animaInfo);
            }
            else if(cmd.GetCmdType() == CmdFspEnum.eUIHead)
            {
                CmdUIHead head = cmd as CmdUIHead;
                switch(head.type)
                {
                    case 1:
                    m_head.SetName(head.name);
                    break;
                    case 2:
                    m_head.SetLevel(head.lv);
                    break;
                    case 3:
                    m_head.SetHp(head.curHp, head.maxHp);
                    break;
                }
            }
            else if(cmd.GetCmdType() == CmdFspEnum.eLife)
            {
                CmdLife head = cmd as CmdLife;
                if(head.state)
                {

                }
                else
                {
                    AnimationAction animaInfo = new AnimationAction();
                    animaInfo.crossTime = AnimationInfo.m_crossTime;
                    animaInfo.playSpeed = 1;
                    animaInfo.strFull = "die_1";
                    animaInfo.eMode = WrapMode.Once;
                    m_ent.Play(animaInfo);
                }
            }
        }

        public virtual void Update(float time, float fdTime)
        {
            if(m_ent == null || !m_ent.IsInited())
                return;
            if(m_head != null)
            {
                m_head.UpdatePos(m_ent.GetPos() + Vector3.up * 4f);
            }
            if(m_bMoveing) 
            {
                Entity ent = m_ent as Entity;
                _UpdateMove(time, fdTime, ref ent, m_moveInfo);
                GetEnt().SetRot(Quaternion.LookRotation(m_moveInfo.m_dir));
            }
        }

        public virtual void Destory()
        {
            if(m_ent != null)
            {
                EntityManager.Inst.RemoveEntity(m_ent.m_hid, true);
                m_ent = null;
            }
        }


        public Vector3 _UpdateMove(float fTime, float fdTime, ref Entity ent, MtBaseMoveInfo moveInfo)
        {
            // 表现层每帧增加距离
            float dist = moveInfo.m_speed * fdTime;
            Vector3 dir = moveInfo.m_dir;
            Vector3 logicPos = moveInfo.m_pos;
            Vector3 curPos = ent.GetPos();
            // 表现层模拟的新位置
            Vector3 newPos = curPos + dir * dist;

            Vector3 result = logicPos;
            // 逻辑位置和表现层位置差值，距离
            Vector3 offsetMove = newPos - logicPos;
            float tempDis = offsetMove.magnitude;
            // 最大偏移量， 这个应该是不对的
            float minOffset = moveInfo.m_speed * FSPParam.clientFrameScTime;
            float maxOffset = minOffset * 3;
            if (tempDis < moveInfo.RepairFramesMin * minOffset)  // （每帧表现-逻辑）小于最小的每帧偏移量时，取渲染位置，这样才能保证每一帧移动的流畅
            {
                result = newPos;
                moveInfo.RepairFramesMin = 1;
                moveInfo.FrameBlockIndex = GameManager.Inst.GetFspManager().GetCurFrameIndex();
                //Debug.Log("1.渲染位置和逻辑位置差值小于最小帧偏移， 直接取渲染位置");
            }
            else if (tempDis < maxOffset)   // （每帧表现-逻辑）大于每帧偏移量时，也需要平滑处理
            {
                //Debug.Log("2.渲染位置和逻辑位置差值小于最大帧偏移，进行插值逼近");
                float adjustRatio = Mathf.Clamp(tempDis / maxOffset, 0.05f, 0.3f);
                float dotValue = Vector3.Dot(offsetMove, dir);
                Vector3 estimPos = logicPos + dir * maxOffset;       // 逻辑估计的目标位置
                Vector3 estimDir = (estimPos - curPos).normalized;   // 逻辑估计的方向

                //Debug.Log("dotValue:" + dotValue);
                //正1/4圆周区间，超前了，减速运动
                if (dotValue > tempDis * 0.707f)
                {
                    result = curPos + estimDir * dist * (1.0f - adjustRatio);
                }
                //负1/4圆周区间，调后了，加速运动
                else if (dotValue < tempDis * (-0.707f))
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
                if (GameManager.Inst.GetFspManager().GetCurFrameIndex()== moveInfo.FrameBlockIndex)
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
            // 切到后台时，网络层没卡，渲染帧卡主，但是切回来时，会获取最新逻辑位置
            //if ((this as MtCreature) != null)
            //{
            //    Debug.Log("logicPos:" + logicPos.x + "|" + logicPos.z);
            //    Debug.Log("entityPos:" + curPos.x + "|" + curPos.z);
            //}
            ent.SetPos(result);
            return result;
        }

    }

    
    public class MtBaseMoveInfo
    {
        public bool m_teleport;
        public Vector3 m_pos;
        public Vector3 m_dir;   // 单位方向
        public float m_speed;
        // 平滑插值下限偏差（好像并没什么用）
        public int RepairFramesMin = 1;
        // 卡住时的当前帧
        public int FrameBlockIndex;
    }
}

