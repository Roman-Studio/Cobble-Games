using Unity.Plastic.Newtonsoft.Json;

namespace CobbleGames.SaveSystem.Data
{
    [System.Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class CGSaveDataEntryInt : CGSaveDataEntry
    {
        [JsonProperty]
        public int Data { get; set; }
        
        public CGSaveDataEntryInt() : base(ECGSaveDataType.Int)
        {
            
        }
        
        public CGSaveDataEntryInt(int data) : this()
        {
            Data = data;
        }

        public override object GetData() => Data;
    }
}