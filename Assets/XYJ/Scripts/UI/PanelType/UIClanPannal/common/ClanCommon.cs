using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.UI
{
    namespace Clan
    {
        public class ClanCommon
        {
            static public List<ClanUser> GetMemberName(ClanDbData data,ClanPost post)
            {
                List<ClanUser> tempList = new List<ClanUser>();
                tempList.Clear();

                foreach (var item in data.member.membermap)
                {
                    if (post == item.Value.post)
                    {
                        tempList.Add(item.Value);
                    }
                }

                return tempList;
            }

            static public string GetBuildName(int id)
            {
                switch (id)
                {
                    case 1:
                        return "氏族";
                    case 2:
                        return "集贤阁";
                    case 3:
                        return "金库";
                    case 4:
                        return "技能坊";
                    case 5:
                        return "百宝阁";
                    default:
                        return "";
                }

            }

            static public string GetPostname(ClanPost post)
            {
                switch (post)
                {
                    case ClanPost.Clan_None:
                        return "";
                    case ClanPost.Clan_Leader:
                        return "族长";
                    case ClanPost.Clan_Subleader:
                        return "副族长";
                    case ClanPost.Clan_Elder:
                        return "长老";
                    case ClanPost.Clan_Member:
                        return "成员";
                    case ClanPost.Clan_Apprentice:
                        return "学徒";
                    case ClanPost.Clan_Oiran:
                        return "花魁";
                    default:
                        return "";
                        break;
                }
            }
        }
    }
}