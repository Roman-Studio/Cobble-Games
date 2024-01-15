using System.Collections.Generic;
using Newtonsoft.Json;

namespace CobbleGames.SaveSystem.Data
{
    [System.Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class CGSaveDataEntryList : CGSaveDataEntry
    {
        [JsonProperty] 
        private List<CGSaveDataEntry> _DataList;
        public IReadOnlyList<CGSaveDataEntry> DataList => _DataList;
        
        public CGSaveDataEntryList() : this(new List<CGSaveDataEntry>())
        {
            
        }
        
        public CGSaveDataEntryList(List<CGSaveDataEntry> dataList) : base(ECGSaveDataType.List)
        {
            _DataList = dataList;
        }

        public override object GetData() => _DataList;

        public void AddListEntry(CGSaveDataEntry saveDataEntry)
        {
            _DataList.Add(saveDataEntry);
        }

        public bool RemoveListEntry(CGSaveDataEntry saveDataEntry)
        {
            return _DataList.Remove(saveDataEntry);
        }
    }
}