using Newtonsoft.Json;

namespace CobbleGames.SaveSystem.Data
{
    [System.Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(CGSaveDataConverter))]
    public class CGSaveDataEntry
    {
        [JsonProperty] 
        public ECGSaveDataType DataType { get; protected set; }

        public CGSaveDataEntry()
        {
            
        }

        protected CGSaveDataEntry(ECGSaveDataType dataType)
        {
            DataType = dataType;
        }
        
        public virtual object GetData() => null;
    }
}