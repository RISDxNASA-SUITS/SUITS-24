using Newtonsoft.Json;

namespace MixedReality.Toolkit.Suits.Map
{
    public class UiaData
    {
        public UiaDataRecord Data;

        public UiaData(string json)
        {
            Data = JsonConvert.DeserializeObject<UiaDataRecord>(json);

        }


    }
    public record UiaDataRecord
    {
        public bool eva1_power;
        public bool eva1_oxy;
        public bool eva1_water_supply;
        public bool eva1_water_waste;
        public bool eva2_power;
        public bool eva2_oxy;
        public bool eva2_water_supply;
        public bool eva2_water_waste;


    };
}
