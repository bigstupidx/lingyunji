using System.Collections.Generic;

namespace Config
{
    public partial class PersonalityDefine
    {
        public static int sectionCount = 5;

        public class PersonalitySection
        {
            public int rank;
            public int lowerLimit;
            public int upperLimit;
            public int cittaId;
            public string desc;

            public PersonalitySection(int id, int lower, int upper, int citta, string descri)
            {
                rank = id;
                lowerLimit = lower;
                upperLimit = upper;
                cittaId = citta;
                desc = descri;
            }
        }

        public PersonalitySection[] sectionGroup = new PersonalitySection[sectionCount];

        public static void OnLoadEndLine(PersonalityDefine data, CsvCommon.ICsvLoad reader, int i)
        {
            data.sectionGroup[0] = new PersonalitySection(1, data.lowerLimit_1, data.upperLimit_1, data.cittaId_1, data.desc_1);
            data.sectionGroup[1] = new PersonalitySection(2, data.lowerLimit_2, data.upperLimit_2, data.cittaId_2, data.desc_2);
            data.sectionGroup[2] = new PersonalitySection(3, data.lowerLimit_3, data.upperLimit_3, data.cittaId_3, data.desc_3);
            data.sectionGroup[3] = new PersonalitySection(4, data.lowerLimit_4, data.upperLimit_4, data.cittaId_4, data.desc_4);
            data.sectionGroup[4] = new PersonalitySection(5, data.lowerLimit_5, data.upperLimit_5, data.cittaId_5, data.desc_5);
        }
    }
}