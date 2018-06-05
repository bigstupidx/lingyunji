#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using System.Collections.Generic;
    using Config;
    using battle;
    using NetProto.Hot;

    class TrumpsMgr
    {
        public const int MAX_EQUIP_POS = 4;
        protected readonly float[] EQUIP_ADD = new float[] { 1.0f, 0.7f, 0.3f, 0.0f };
        public TrumpsTable table { get { return m_TrumpsTable; } }
        TrumpsTable m_TrumpsTable = new TrumpsTable();

        //判断当前装备栏是否为空
        public bool IsNullEquip()
        {
            foreach (int trumpid in table.equiptrumps.Values)
                if (trumpid != 0)
                    return false;
            return true;
        }

        public string GetTasteDes(int trumpId)
        {
            int tasteLv = 0;
            if (this.CheckTrumps(trumpId))
                tasteLv = m_TrumpsTable.attributes[trumpId].tastelv;

            return string.Format("境界•{0}", GlobalSymbol.ToNum(tasteLv)); 
        }

        public string GetInfusedDes(int trumpId)
        {
            TrumpAttribute attribute = this.GetTrumpAttribute(trumpId);
            if (!TrumpInfused.infusedDic.ContainsKey(trumpId) || !TrumpInfused.infusedDic[trumpId].ContainsKey(attribute.tastelv))
                return string.Empty;
            int Lingshu = 0, Lingqiao = 0;
            int cLingshu = 0, cLingqiao = 0;
            int tasteLv = attribute.tastelv;
            for(int i = 0;i < attribute.infuseds.Count;i++)
            {
                if (TrumpInfused.FindIndex(attribute.infuseds[i]) == -1)
                    continue;
                if (TrumpInfused.Get(attribute.infuseds[i]).type == TrumpsInfusedType.Normal)
                    Lingshu += 1;
                else if (TrumpInfused.Get(attribute.infuseds[i]).type == TrumpsInfusedType.Special)
                    Lingqiao += 1;
            }

            List<TrumpInfused> infuseAll = TrumpInfused.infusedDic[trumpId][attribute.tastelv];
            foreach(TrumpInfused item in infuseAll)
            {
                if (item.type == TrumpsInfusedType.Normal)
                    cLingshu += 1;
                else if (item.type == TrumpsInfusedType.Special)
                    cLingqiao += 1;
            }
            if(cLingqiao == 0)
                return string.Format("灵窍 {0}/{1}", Lingshu, cLingshu);
            else
                return string.Format("灵窍{0}/{1} 灵枢 {2}/{3}", Lingshu, cLingshu, Lingqiao, cLingqiao);
        }
        public bool CanInfuse(int trumpId, int infusedId)
        {
            if (!this.CheckTrumps(trumpId))
                return false;
            if (TrumpInfused.FindIndex(infusedId) == -1)
                return false;
            TrumpAttribute attribute = this.m_TrumpsTable.attributes[trumpId];
            if (attribute.infuseds.Contains(infusedId))
                return false;
            if (!TrumpInfused.infusedDic.ContainsKey(trumpId) || !TrumpInfused.infusedDic[trumpId].ContainsKey(attribute.tastelv))
                return false;
            TrumpInfused infusedData = this.GetLastInfusedData(trumpId);
            TrumpInfused nextInfusedData = TrumpInfused.Get(infusedId);


            if (nextInfusedData.bindid == 0)
                return infusedData.infusedid == nextInfusedData.infusedid;

            return !this.m_TrumpsTable.attributes[trumpId].infuseds.Contains(nextInfusedData.infusedid)
                && this.m_TrumpsTable.attributes[trumpId].infuseds.Contains(nextInfusedData.bindid);
        }

        #region 双端通用
        //法宝当前最大潜修等级
        public int GetMaxSoulLv(int trumpId)
        {
            int tasteLv = 0;
            if (this.CheckTrumps(trumpId))
                tasteLv = m_TrumpsTable.attributes[trumpId].tastelv;

            int maxLv = 0;
            foreach (TrumpCultivateExp data in TrumpCultivateExp.GetAll().Values)
            {
                if (data.tastelv > tasteLv)
                {
                    maxLv = data.id;
                    break;
                }
            }

            if (TrumpCultivateExp.Get(TrumpCultivateExp.GetAll().Count - 1).tastelv <= tasteLv)
                maxLv = TrumpCultivateExp.Get(TrumpCultivateExp.GetAll().Count - 1).id;

            return maxLv;
        }

        #region 注灵
        //得到指定法宝最后一位灵窍点数据,如果返回空，则表示该境界还没有灵窍点被点亮
        public TrumpInfused GetLastInfusedData(int trumpId)
        {
            if (!this.CheckTrumps(trumpId))
                return null;
            int infusedid = 0;
            for (int i = m_TrumpsTable.attributes[trumpId].infuseds.Count - 1; i >= 0; i--)
            {
                infusedid = m_TrumpsTable.attributes[trumpId].infuseds[i];
//                 if (TrumpInfused.FindIndex(infusedid) == -1)
//                     continue;
                if (TrumpInfused.Get(infusedid).type == TrumpsInfusedType.Normal)
                    return TrumpInfused.Get(infusedid);
            }
            return TrumpInfused.infusedDic[trumpId][m_TrumpsTable.attributes[trumpId].tastelv][0];
        }
        //检查注灵数据是否存在
        public bool CheckInfuse(int trumpId, int infusedId)
        {
            if (!this.CheckTrumps(trumpId))
                return false;
            int infusedid = 0;
            for (int i = m_TrumpsTable.attributes[trumpId].infuseds.Count - 1; i >= 0; i--)
            {
                infusedid = m_TrumpsTable.attributes[trumpId].infuseds[i];
                if (infusedid == infusedId)
                    return true;
            }
            return false;
        }
        #endregion

        public bool MaxTaste(int trumpId)
        {
            if (!this.CheckTrumps(trumpId))
                return false;

            TrumpAttribute trumpAttribute = m_TrumpsTable.attributes[trumpId];
            return TrumpInfused.infusedDic[trumpId].ContainsKey(trumpAttribute.tastelv + 1) ? false : true;
        }
        public bool CanTasteUp(int trumpId)
        {
            if (!this.CheckTrumps(trumpId))
                return false;
            TrumpAttribute trumpAttribute = m_TrumpsTable.attributes[trumpId];
            //检测境界等级数据
            if (!TrumpSoul.soulDic.ContainsKey(trumpAttribute.id) || !TrumpSoul.soulDic[trumpAttribute.id].ContainsKey(trumpAttribute.tastelv))
                return false;  
           if (!TrumpInfused.infusedDic.ContainsKey(trumpAttribute.id) || !TrumpInfused.infusedDic[trumpAttribute.id].ContainsKey(trumpAttribute.tastelv))
                return false;
            //检测注灵是否满足淬魂条件
            List<TrumpInfused> trumpInfused = TrumpInfused.infusedDic[trumpAttribute.id][trumpAttribute.tastelv];
            if (trumpInfused.Count != trumpAttribute.infuseds.Count || !TrumpInfused.infusedDic[trumpAttribute.id].ContainsKey(trumpAttribute.tastelv + 1))
                return false;
            return true;
        }

        //装备位置索引
        public int GetEquipPos(int trumpId)
        {
            if (!m_TrumpsTable.equiptrumps.ContainsValue(trumpId))
                return -1;
            foreach (int pos in m_TrumpsTable.equiptrumps.Keys)
                if (m_TrumpsTable.equiptrumps[pos] == trumpId)
                    return pos;

            return -1;
        }

        public TrumpAttribute GetTrumpAttribute(int trumpId)
        {
            if (CheckTrumps(trumpId))
                return m_TrumpsTable.attributes[trumpId];
            
            TrumpAttribute attribute = this.GetNewTrumpAttribute(trumpId);
            return attribute;
        }

        public TrumpAttribute GetNewTrumpAttribute(int trumpId)
        {
            TrumpProperty property = TrumpProperty.Get(trumpId);
            TrumpAttribute attribute = new TrumpAttribute();
            attribute.id = trumpId;
            attribute.souldata.lv = 0;
            attribute.souldata.exp = 0;
            attribute.activeskill.id = property.activeskill;
            attribute.activeskill.lv = 1;
            attribute.passiveskill.id = property.passiveskill;
            attribute.passiveskill.lv = 1;
            return attribute;
        }

        /// <summary>
        /// 检测法宝激活状态
        /// </summary>
        public bool CheckTrumps(int trumpId)
        {
            return m_TrumpsTable.attributes.ContainsKey(trumpId);
        }

        /// <summary>
        /// 是否能升级
        /// </summary>
        public bool CanLvUp(int trumpId, int lv, int exp)
        {
            if (!TrumpCultivateExp.GetAll().ContainsKey(lv))
                return false;
            TrumpCultivateExp data = TrumpCultivateExp.Get(lv);
            if (lv >= TrumpCultivateExp.Get(TrumpCultivateExp.GetAll().Count - 1).id && exp >= TrumpCultivateExp.Get(TrumpCultivateExp.GetAll().Count - 1).exp)
                return false;
            if (lv >= this.GetMaxSoulLv(trumpId) && exp >= data.exp)
                return false;
            return true;
        }

        void CalculateLv(int tasteLv, TrumpSoulData soulData, int itemExp)
        {
            int lv = soulData.lv;
            int exp = soulData.exp + itemExp;
            while (true)
            {
                TrumpCultivateExp data = TrumpCultivateExp.Get(lv);
                if (data == null) break;
                if (exp < data.exp) break;
                if (tasteLv < data.tastelv || TrumpCultivateExp.Get(TrumpCultivateExp.GetAll().Count - 1).id == lv)
                {
                    exp = data.exp;
                    break;
                }
                exp -= data.exp;
                lv += 1;
            }
            soulData.lv = lv;
            soulData.exp = exp;
        }

        public bool CalculateTrumps(ref BattleAttri battleAttri)
        {
            if (this.m_TrumpsTable.equiptrumps.Count == 0)
                return false;
            //计算法宝属性
            foreach (int pos in this.m_TrumpsTable.equiptrumps.Keys)
            {
                int trumpId = this.m_TrumpsTable.equiptrumps[pos];
                if (!this.m_TrumpsTable.attributes.ContainsKey(trumpId))
                    continue;
                TrumpObj trumpObj = new TrumpObj();
                if (trumpObj.Init(this.m_TrumpsTable.attributes[trumpId]))
                    foreach (int index in trumpObj.battleAttri.GetKeys())
                        battleAttri.Add(index, trumpObj.battleAttri.Get(index) * this.EQUIP_ADD[pos]);
            }
            //计算连携属性
            this.CalculateJoining(ref battleAttri);
            return true;
        }

        //计算装备法宝中属性
        void CalculateJoining(ref BattleAttri battleAttri)
        {
            #region 连携属性计算
            List<TrumpJoining> joinings = this.GetActiveJoining();
            for (int i = 0; i < joinings.Count; i++)
                battleAttri.Add(joinings[i].battleAttri);
            #endregion
        }

        /// <summary>
        /// 当前装备激活的连协技能
        /// </summary>
        /// <returns></returns>
        public List<TrumpJoining> GetActiveJoining()
        {
            List<TrumpJoining> res = new List<TrumpJoining>();
            //缓存装备法宝的连协数据
            List<int> tempGroud = new List<int>();
            foreach (int trumpid in m_TrumpsTable.equiptrumps.Values)
            {
                if (!TrumpProperty.GetAll().ContainsKey(trumpid))
                    continue;
                TrumpProperty property = TrumpProperty.Get(trumpid);
                for (int i = 0; i < property.joinings.Length; i++)
                {
                    if (!TrumpJoining.GetAll().ContainsKey(property.joinings[i]))
                        continue;
                    if (tempGroud.Contains(property.joinings[i]))
                        continue;
                    tempGroud.Add(property.joinings[i]);
                }
            }

            List<int> equip = new List<int>(m_TrumpsTable.equiptrumps.Values);
            List<int> compare = null;
            for (int i = 0; i < tempGroud.Count; i++)
            {
                if (!TrumpJoining.GetAll().ContainsKey(tempGroud[i]))
                    continue;
                TrumpJoining joiningData = TrumpJoining.Get(tempGroud[i]);
                compare = new List<int>(joiningData.trump);
                bool isAdd = true;
                for(int j = 0; j < compare.Count;j++)
                {
                    if(!equip.Contains(compare[j]))
                    {
                        isAdd = false;
                        break;
                    }
                }
                if (isAdd)
                    res.Add(TrumpJoining.Get(tempGroud[i]));
            }
            return res;
        }

        public bool CompareJoiningList(List<TrumpJoining> list1 ,List<TrumpJoining> list2)
        {
            bool isNewJoining = false;
            for (int i = 0; i < list2.Count; i++)
            {
                if (!list1.Contains(list2[i]))
                {
                    isNewJoining = true;
                    break;
                }
            }

            return isNewJoining;
        }

        public bool CanUpgradeSkill(int trumpId,int skillType)
        {
            if (!this.CheckTrumps(trumpId))
                return false;
            TrumpAttribute attribute = m_TrumpsTable.attributes[trumpId];
            TrumpSkillData skillData = skillType == (int)TrumpSkillType.Active ? attribute.activeskill : attribute.passiveskill;
            if (!TrumpSkill.GetAll().ContainsKey(skillData.id))
                return false;
            TrumpSkill skillProperty = TrumpSkill.Get(skillData.id);
            if (skillProperty.needtastelv > attribute.tastelv)
                return false;
            return true;
        }
        #endregion
    }
}
#endif