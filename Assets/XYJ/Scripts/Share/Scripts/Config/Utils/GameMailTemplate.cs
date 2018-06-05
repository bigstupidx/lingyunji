using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Config
{
    public partial class GameMailTemplate
    {
        public List<NetProto.MailDbItem> attachments = new List<NetProto.MailDbItem>();
        public static void OnLoadEndLine(GameMailTemplate data, CsvCommon.ICsvLoad reader, int i)
        {
            foreach (string itemStr in data.attachementStr.Split(';'))
            {
                const int ID_IDX = 0;
                const int NUM_IDX = 1;

                string[] itemParams = itemStr.Split(',');
                if (itemParams.Length >= NUM_IDX + 1)
                {
                    try
                    {
                        int id = int.Parse(itemParams[ID_IDX]);
                        int num = int.Parse(itemParams[NUM_IDX]);
                        if (id <= 0 || num <= 0)
                            Log.Error(string.Format("WorldMailTemplate.OnLoadEndLine id:{0}, num:{1}", id, num));
                        else
                            data.attachments.Add(new NetProto.MailDbItem() { id = id, num = num });
                    }
                    catch (Exception e)
                    {
                        Log.Error(string.Format("WorldMailTemplate.OnLoadEndLine {0}", e.Message));
                    }
                }
            }
        }
    }
}
