using System;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace CobbleGames.SaveSystem.Data
{
    public class CGSaveDataConverter : JsonConverter<CGSaveDataEntry>
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, CGSaveDataEntry value, JsonSerializer serializer)
        {
            
        }

        public override CGSaveDataEntry ReadJson(JsonReader reader, Type objectType, CGSaveDataEntry existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            CGSaveDataEntry result;

            var tagType = (ECGSaveDataType)jsonObject.Value<int>(nameof(result.DataType));

            result = tagType switch
            {
                ECGSaveDataType.Int => new CGSaveDataEntryInt(),
                ECGSaveDataType.Float => new CGSaveDataEntryFloat(),
                ECGSaveDataType.String => new CGSaveDataEntryString(),
                ECGSaveDataType.Bool => new CGSaveDataEntryBool(),
                ECGSaveDataType.List => new CGSaveDataEntryList(),
                ECGSaveDataType.Dictionary => new CGSaveDataEntryDictionary(),
                ECGSaveDataType.Null => new CGSaveDataEntryBool(),
                _ => throw new ArgumentOutOfRangeException()
            };

            serializer.Populate(jsonObject.CreateReader(), result);
            return result;
        }
    }
}