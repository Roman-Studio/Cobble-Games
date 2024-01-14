using Unity.Plastic.Newtonsoft.Json;

namespace CobbleGames.SaveSystem.Data
{
    [System.Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class CGSaveDataEntryString : CGSaveDataEntry
    {
        [JsonProperty]
        public string Data { get; set; }
        
        public CGSaveDataEntryString() : base(ECGSaveDataType.String)
        {
            
        }
        
        public CGSaveDataEntryString(string data) : this()
        {
            Data = data;
        }

        public override object GetData() => Data;
    }
}