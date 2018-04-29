using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
namespace Roma
{
    public class NearState : FSMState
    {
        public NearState(CCreature go)
            : base(go)
        {
            m_stateId = StateID.NearState;
        }
        public override void Enter()
        {
            if (m_creature.m_targetCreature == null)
                return;

            Vector3 masterPos = m_creature.GetPos();
            Vector2 startPos = new Vector2(masterPos.x, masterPos.z);

            Vector3 npcPos = m_creature.m_targetCreature.GetPos();
            Vector2 endPos = new Vector2(npcPos.x, npcPos.z);

            if (m_creature.m_targetCreature.IsChunnel())  // 传送门，直接走上去
            {
                bool starGo = m_creature.GoTo(endPos.x, endPos.y, eControlMode.eCM_auto, 0);
                //if (starGo)
                   // m_creature.m_moveStateParam.m_moveEnd = MoveEnd;
                return;
            }

            Vector3 range = m_creature.m_targetCreature.GetEntity().GetBoxSize();
            m_speakDistance = (range.y + range.z) * 0.5f + 0.3f;

            if (m_speakDistance < 1)
                m_speakDistance = 1;
            if (Vector2.Distance(endPos, startPos) > (m_speakDistance + 2f)) //+2保持下次检查不会大于谈话距离
            {
                //Vector2 dir = startPos - endPos;
                Vector3 dir3 = m_creature.m_targetCreature.GetQua() * Vector3.forward;
                Vector2 dir = new Vector2(dir3.x, dir3.z);
                Vector2 moveEndPos = endPos + dir.normalized * m_speakDistance;
                bool starGo = m_creature.GoTo(moveEndPos.x, moveEndPos.y, eControlMode.eCM_auto, 0);
                //if (starGo)
                   // m_creature.m_moveStateParam.m_moveEnd = MoveEnd;
            }
            else
            {
                StartHandle();
            }
        }

        private void MoveEnd(CCreature creature)
        {
            if (Vector3.Distance(m_creature.GetPos(), m_creature.m_targetCreature.GetPos()) < m_speakDistance + 2f)
                StartHandle();
        }

        private void StartHandle()
        {
            m_creature.PushCommand(StateID.IdleState);
            Vector3 dir = (m_creature.m_targetCreature.GetPos() - m_creature.GetPos()).normalized;
            Quaternion qua = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            m_creature.m_moveStateParam.m_btEndQua = qua;
            m_creature.PushCommand(StateID.RotateState);

            if (m_creature.m_targetCreature == null)
            {
                return;
            }
            if (m_creature.m_targetCreature.IsNpc()) // 点击NPC触发对话
            {
                StartSpeak();
            }
            else if(m_creature.m_targetCreature.IsChunnel())  // 传送门，执行传送
            {
                StartTransmit();
            }
            else if(m_creature.m_targetCreature.IsMonster() || m_creature.m_targetCreature.IsPet())    // 点击怪物触发战斗
            {
                StartSpeak();
            }
            else if(m_creature.m_targetCreature.IsWorldBoss())
            {
                // 世界BOSS直接触发战斗
               // string sLevelId = ((CMonster)m_creature.m_targetCreature).m_nearStateParem.m_clickActionID;
               // int levelId = 0;
               // int.TryParse(sLevelId, out levelId);
            }
        }

        private void StartSpeak()
        {
           
        
            //npcDialog.SetSpeakID(((CNpc)m_creature.m_targetCreature).m_npcCfg.ActionSpeakID);
            //npcDialog.SetNpcType((NpcType)((CNpc)m_creature.m_targetCreature).m_npcCfg.Type);
            //npcDialog.SetActionId((int)(NpcType)((CNpc)m_creature.m_targetCreature).m_clickActionID);
            //npcDialog.SetNpcName(((CNpc)m_creature.m_targetCreature).m_npcCfg.Name);

            //CCameraMgr.m_battleCamera.transform.position = CCameraMgr.m_mainCamera.transform.position;
            //CCameraMgr.m_battleCamera.transform.eulerAngles = CCameraMgr.m_mainCamera.transform.eulerAngles;
            //CCameraMgr.m_battleCamera.fieldOfView = 60f;
            //if (m_creature.m_targetCreature == null)
            //{
            //    return;
            //}
            //// 找到NPC前方3米，高度yPos的点
            //Entity tEnt = m_creature.m_targetCreature.GetEntity();
            //float yPos = tEnt.GetBoxCenter().y;
            //Vector3 target = tEnt.GetPos() + tEnt.GetRotateQua() * Vector3.forward * 3;
            //// 计算方向
            //Vector3 dir = tEnt.GetPos() - target;
            //dir = Quaternion.LookRotation(dir).eulerAngles;
            //CCameraMgr.MoveCamera(target + new Vector3(0, yPos, 0), dir, null);
        }

        private void StartTransmit()
        {
            if (m_creature.m_targetCreature == null)
                return;
            //NpcFunctionCsv functionCsv = CsvManager.Inst.GetCsv<NpcFunctionCsv>((int)eAllCSV.eAC_NpcFunction);
            //NpcFunctionCsvData functionData = functionCsv.GetData(m_creature.m_targetCreature.m_functionId);
            //if (functionData != null)
            //{
            //    if (functionData.infoList.Count == 1 && functionData.infoList[0].type == (int)eNpcFunctionType.Chunnel)
            //    {
            //        MapMgr.OnTransmitToPos((uint)functionData.infoList[0].id, functionData.infoList[0].pos.x, functionData.infoList[0].pos.y);
            //    }
            //}
        }

        public override void Update(float fTime, float fDTime)
        {
            base.Update(fTime, fDTime);
        }

        public override void Exit()
        {
           
        }

        //private Quaternion m_rotateCurQua = Quaternion.identity;        // 当前
        //private Quaternion m_rotateDestQua = Quaternion.identity;       // 目标
        //private float m_rotateTime = 0.0f;
       // private float m_rotateCurTime = 0.0f;
        //private bool m_bRotateing = false;

        private float m_speakDistance = 5;
    }
}
