using Unity.Plastic.Newtonsoft.Json;

namespace CobbleGames.SaveSystem.Data
{
    [System.Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class CGSaveDataEntryBool : CGSaveDataEntry
    {
        [JsonProperty]
        public bool Data { get; set; }
        
        public CGSaveDataEntryBool() : base(ECGSaveDataType.Bool)
        {
            
        }
        
        public CGSaveDataEntryBool(bool data) : this()
        {
            Data = data;
        }

        public override object GetData() => Data;
    }
}