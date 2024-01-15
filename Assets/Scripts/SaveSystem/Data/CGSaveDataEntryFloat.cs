using Newtonsoft.Json;

namespace CobbleGames.SaveSystem.Data
{
    [System.Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class CGSaveDataEntryFloat : CGSaveDataEntry
    {
        [JsonProperty]
        public float Data { get; set; }
    
        public CGSaveDataEntryFloat() : base(ECGSaveDataType.Float)
        {
        
        }
    
        public CGSaveDataEntryFloat(float data) : this()
        {
            Data = data;
        }

        public override object GetData() => Data;
    }
}