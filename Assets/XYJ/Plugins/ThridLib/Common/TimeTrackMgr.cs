using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PackTool
{
#if ASSET_DEBUG
    public class TimeTrackMgr : Singleton<TimeTrackMgr>
    {
        static TimeTrackMgr()
        {
            CreateInstance();
        }

        Dictionary<string, TimeTrack> TimeTrackList = new Dictionary<string, TimeTrack>();

        public Dictionary<string, TimeTrack> GetAll()
        {
            return TimeTrackList;
        }

        public void Save(string defname = null)
        {
            string file_suffix ;
            if (string.IsNullOrEmpty(defname))
                file_suffix = ResourcesPath.LocalBasePath + "/TimeTrack/Json";
            else
                file_suffix = ResourcesPath.LocalBasePath + "/TimeTrack/" + defname;

            string file = file_suffix + ".txt";
            System.IO.Directory.CreateDirectory(ResourcesPath.LocalBasePath + "/TimeTrack/");
            int index = 0;
            while (System.IO.File.Exists(file))
                file = file_suffix + (++index).ToString() + ".txt";

            System.IO.File.WriteAllText(file, TimeTrackMgr.Instance.ToJson().toString());
        }

        public void Clear()
        {
            foreach (KeyValuePair<string, TimeTrack> itor in TimeTrackList)
                itor.Value.Reset();
        }

        public TimeTrack Get(string type)
        {
            TimeTrack tt = null;
            if (TimeTrackList.TryGetValue(type, out tt))
                return tt;

            tt = TimeTrack.Create();
            TimeTrackList.Add(type, tt);

            return tt;
        }

        public JSONObject ToJson()
        {
            JSONObject json = new JSONObject();
            foreach (KeyValuePair<string, TimeTrack> itor in TimeTrackList)
                json.put(itor.Key, itor.Value.ToJson());

            return json;
        }

        public void InitByJson(JSONObject json)
        {
            TimeTrackList.Clear();
            foreach (string key in JSONObject.getNames(json))
            {
                TimeTrack tt = TimeTrack.Create();
                tt.InitByJson(json.getJSONObject(key));
                TimeTrackList.Add(key, tt);
            }
        }
    }
#endif
}