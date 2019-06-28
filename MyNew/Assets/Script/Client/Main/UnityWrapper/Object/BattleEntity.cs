using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Profiling;

namespace Roma
{
    public enum eEquipType
    {
        None,
        HandRight = 1,
        Back = 2,
        Grenade = 3,    // 手榴弹
        Armet = 4,      // 衣服
        Clothes = 5,   // 头盔
        Ornaments = 6, // 饰品

        HandLeftAndRight = 11, // 表现层才有的左右手
    }

    /// <summary>
    /// 实体类
    /// </summary>
    public class BattleEntity : BoneEntity
    {
        /// <summary>
        /// 部位-资源HID
        /// </summary>
        public Dictionary<int, object> m_equipList;

        public const string HAND_RIGHT = "R_hand";
        public const string HAND_LEFT = "l_hand";
        public const string BACK = "back";

        private AnimationAction m_animaInfo;

        public int m_commonAtkIndex;

        public BattleEntity(int handle, Action<Entity> notify, eEntityType eType, EntityBaseInfo entityInfo)
            : base(handle, notify, eType, entityInfo)
        {

        }

        public override void Revive(int handleId, Action<Entity> notity, EntityBaseInfo baseInfo)
        {
            base.Revive(handleId, notity, baseInfo);
        }

        /// <summary>
        /// 设置优先级
        /// </summary>
        public void SetPriority(int pri)
        {
            m_animaPriority = pri;
        }

        /// <summary>
        /// 能单独设置播放速度的动画接口
        /// </summary>
        public void PlayAnima(int animaId, float playSpeed)
        {
            PlayAnima(animaId, null, true, playSpeed);
        }

        /// <summary>
        /// 播放完动作后，默认播放待机
        /// </summary>
        public void PlayAnima(int animaId, Action playerEnd = null, bool bCross = true, float playSpeed = 1.0f)
        {
            if (m_animaInfo == null)
                m_animaInfo = new AnimationAction();

            AnimationCsvData animData = CsvManager.Inst.GetCsv<AnimationCsv>((int)eAllCSV.eAC_Animation).GetData(animaId);
            if (animData == null)
            {
                //Debug.LogError("动作为空 id= " + animaId);
                return;
            }

            // 通过优先级和技能保护时间，来判断是否需要播放
            if (animData.priority < m_animaPriority)
            {
                return;
            }

            m_animaPriority = animData.priority;
            //if(m_object != null)
            //Debug.LogWarning(m_object.name + " id: " + animaId +  " 设置优先级： " + m_animaPriority);

            if (bCross)
            {
                m_animaInfo.crossTime = AnimationInfo.m_crossTime;
            }

            if (animaId == SMtCreatureAnima.ANIMA_DIE)
            {
                m_animaInfo.crossTime = 0;
            }

            m_animaInfo.playSpeed = playSpeed * animData.speed;
            m_animaInfo.strFull = animData.animationName;
            m_animaInfo.eMode = (WrapMode)animData.mode;

            if (m_animaInfo.eMode == WrapMode.Once)
            {
                m_animaInfo.endEvent = playerEnd;
            }
            else
            {
                m_animaInfo.endEvent = null;
            }
            m_animaInfo.atOnce = animData.atOnce;
            Play(m_animaInfo);
        }


        /// <summary>
        /// 部位-资源id
        /// 最终表现层借口
        /// </summary>
        public void UpdateEquip(Dictionary<eEquipType, int> dic, Action end = null)
        {
            if (dic == null)
                return;

            if (!IsInited())
                return;
            if (!dic.ContainsKey(eEquipType.Clothes))
            {
                dic[eEquipType.Clothes] = 0;
            }
            if (dic[eEquipType.Clothes] == 0)
            {
                Debug.Log("衣服资源Id不能为0，请检查");
                //return;
            }

            // 如果和原始模型不一样，需要换装，并且销毁之前的其他装备
            if (dic[eEquipType.Clothes] != 0 && dic[eEquipType.Clothes] != GetResource().GetResInfo().nResID)
            {
                RemoveAllEquip();

                EntityBaseInfo info = new EntityBaseInfo();
                info.m_resID = dic[eEquipType.Clothes];
                info.m_ilayer = (int)LusuoLayer.eEL_Dynamic;
                info.m_vPos = GetPos();
                info.m_vScale = GetScale();
                info.m_vRotate = GetRotate();
                ChangeResource((newEnt) =>
                {
                    //Debug.Log("换装完成，已经清除之前的装备，重新装备武器");
                    foreach (KeyValuePair<eEquipType, int> item in dic)
                    {
                        if (item.Key == eEquipType.Clothes)
                            continue;
                        if (item.Value == 0)
                        {
                            RemoveEquip(item.Key);
                        }
                        else
                        {
                            AddEquip(item.Key, item.Value);
                        }
                    }
                    if (end != null)
                        end();
                }, info);
            }
            else
            {
                //Debug.Log("无换装，切换武器");
                foreach (KeyValuePair<eEquipType, int> item in dic)
                {
                    if (item.Key == eEquipType.Clothes)
                        continue;
                    if (item.Value == 0)
                    {
                        RemoveEquip(item.Key);
                    }
                    else
                    {
                        AddEquip(item.Key, item.Value);
                    }
                }
                if (end != null)
                    end();
            }
        }

        public void RemoveEquip(eEquipType type)
        {
            if (m_equipList == null)
                return;

            object val = null;
            if (m_equipList.TryGetValue((int)type, out val))
            {
                if (type == eEquipType.HandLeftAndRight)
                {
                    int[] hid = (int[])val;
                    EntityManager.Inst.RemoveEntity(hid[0]);
                    EntityManager.Inst.RemoveEntity(hid[1]);
                    m_equipList.Remove((int)type);
                }
                else
                {
                    int hid = (int)val;
                    EntityManager.Inst.RemoveEntity(hid);
                    m_equipList.Remove((int)type);
                }
            }
        }

        public void AddEquip(eEquipType type, int resId)
        {
            if (m_equipList == null)
                m_equipList = new Dictionary<int, object>();

            // 如果该部位有装备，相同资源不处理，不同的先移除
            object val = null;
            if (m_equipList.TryGetValue((int)type, out val))
            {
                if (type == eEquipType.HandLeftAndRight)
                {
                    int[] hid = (int[])val;
                    Entity ent = EntityManager.Inst.GetEnity(hid[0]);
                    if (ent.GetResource().m_resInfo.nResID == resId)
                    {
                        return;
                    }
                    EntityManager.Inst.RemoveEntity(hid[0]);
                    EntityManager.Inst.RemoveEntity(hid[1]);
                    m_equipList.Remove((int)type);
                }
                else
                {
                    int hid = (int)val;
                    Entity ent = EntityManager.Inst.GetEnity(hid);
                    if (ent == null || ent.GetResource() == null || ent.GetResource().m_resInfo.nResID == resId)
                    {
                        return;
                    }
                    EntityManager.Inst.RemoveEntity(hid);
                    m_equipList.Remove((int)type);
                }
            }

            if (type == eEquipType.HandLeftAndRight)
            {
                int[] hid = new int[2];
                hid[0] = CreatEquip(resId, HAND_RIGHT);
                hid[1] = CreatEquip(resId, HAND_LEFT);
                m_equipList[(int)type] = hid;
            }
            else
            {
                if (type == eEquipType.HandRight)
                {
                    m_equipList[(int)type] = CreatEquip(resId, HAND_RIGHT);
                }
                else if (type == eEquipType.Back)
                {
                    m_equipList[(int)type] = CreatEquip(resId, BACK);
                }
            }
        }

        public void RemoveAllEquip()
        {
            RemoveEquip(eEquipType.HandRight);
            RemoveEquip(eEquipType.HandLeftAndRight);
            RemoveEquip(eEquipType.Back);
        }

        public int CreatEquip(int resId, string parentName)
        {
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_resID = resId;
            info.m_parent = GetBone(parentName);
            info.m_ilayer = (int)LusuoLayer.eEL_Dynamic;
            return EntityManager.Inst.CreateEntity(eEntityType.eNone, info, null);
        }

        public void SetCurve(Vector3 end, float time)
        {
            if (m_object != null)
            {
                TweenCurve cur = TweenCurve.Get(m_object);
                cur.duration = time * 0.001f;
                cur.from = GetPos();
                cur.front = new Vector3(0, 4, 0);
                cur.back = new Vector3(0, 4, 0);
                cur.to = end;
                cur.method = UITweener.Method.EaseIn;
                cur.Reset();
                cur.Play(true);

                //TweenFloat rota = TweenFloat.Get(m_object);
                //rota.from = 0;
                //rota.to = 2000;
                //rota.duration = time * 0.001f;
                //rota.Reset();
                //rota.Play(true);
                //rota.FloatUpdateEvent = (val) => {
                //    SetDirection(val * Vector3.one);
                //};
            }
        }

        public void SetLine(Vector3 end, float time, UITweener.Method show, Action endEvent)
        {
            if (m_object != null)
            {
                TweenPosition cur = TweenPosition.Get(m_object);
                cur.duration = time * 0.001f;
                cur.from = GetPos();
                cur.to = end;
                cur.method = show;
                cur.Reset();
                cur.Play(true);
                cur.onFinished = (go) => {
                    if (endEvent != null)
                        endEvent();
                };
            }
        }

        public Transform GetRightPoint()
        {
            if (m_equipList == null)
                return null;
            if (!m_equipList.ContainsKey((int)eEquipType.HandRight))
            {
                return GetObject().transform;
            }
            int hid = (int)m_equipList[(int)eEquipType.HandRight];
            Entity ent = EntityManager.Inst.GetEnity(hid);
            return ent.GetObject().transform.FindChild("qk");
        }

        public Transform m_backEquip;
        public Transform m_handEquip;

        /// <summary>
        /// 切换火箭炮
        /// </summary>
        public void BackEquipStart()
        {
            Transform back = GetBone("back");
            if (back == null || back.childCount == 0)
                return;

            m_backEquip = back.GetChild(0);
            if (m_backEquip == null)
                return;

            Transform rHand = GetBone("R_hand");
            m_backEquip.SetParent(rHand);
            m_backEquip.localPosition = Vector3.zero;
            m_backEquip.localEulerAngles = Vector3.zero;
            m_backEquip.localScale = Vector3.one;
            // 手
            m_handEquip = rHand.GetChild(0);
            if (m_handEquip == null)
                return;
            m_handEquip.localPosition = Vector3.up * 1000;
        }

        /// <summary>
        /// 结束火箭炮
        /// </summary>
        public void BackEquipEnd()
        {
            if (m_backEquip == null)
                return;

            Transform back = GetBone("back");
            if (back == null)
                return;

            m_backEquip.SetParent(back);
            m_backEquip.localPosition = Vector3.zero;
            m_backEquip.localEulerAngles = Vector3.zero;
            m_backEquip.localScale = Vector3.one;
            if (m_handEquip == null)
                return;
            m_handEquip.localPosition = Vector3.zero;
        }


    }
}