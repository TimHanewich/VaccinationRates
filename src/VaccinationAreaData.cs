using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VaccinationRates
{
    public class VaccinationAreaData
    {
        public string Id {get; set;}
        public string DisplayName {get; set;}
        public VaccinationAreaData[] ChildAreas {get; set;}
        public bool IsVaccineDataAvailable {get; set;}
        public uint? Cases {get; set;} //totalConfirmed
        public uint? Deaths {get; set;} //totalDeaths
        public uint? Recovered {get; set;} //totalRecovered
        public uint? RecoveredDelta {get; set;}
        public uint? DeathsDelta {get; set;}
        public uint? CasesDelta {get; set;} 
        public ulong? DosesAdministered {get; set;}
        public ulong? DosesDistributed {get; set;}
        public float? DosesWithAtLeast1Dose {get; set;}
        public float? DosesWithAtLeast2Dose {get; set;}
        public float? DosesPer100People {get; set;}
        public float? SupplyUsed {get; set;}
        public DateTime LastUpdated {get; set;}
        public float? Latitude {get; set;}
        public float? Longitude {get; set;}
        public string ParentId {get; set;}

        public static async Task<VaccinationAreaData> LoadWorldAsync()
        {
            HttpClient hc = new HttpClient();
            HttpResponseMessage hrm = await hc.GetAsync("https://www.bing.com/covid/local/unitedstates?vert=vaccineTracker");
            string content = hrm.Content.ReadAsStringAsync().Result;
            int loc1 = content.IndexOf("var data=");
            loc1 = content.IndexOf("=", loc1 + 1);
            int loc2 = content.IndexOf("</script>", loc1 + 1);
            loc2 = content.LastIndexOf(";", loc2);
            string data = content.Substring(loc1 + 1, loc2 - loc1 - 1);
            
            VaccinationAreaData ToReturn = VaccinationAreaData.Load(data);
            return ToReturn;
        }

        public static VaccinationAreaData Load(string json)
        {
            VaccinationAreaData ToReturn = new VaccinationAreaData();

            JObject jo = JObject.Parse(json);

            ToReturn.Id = jo.Property("id").Value.ToString();
            ToReturn.DisplayName = jo.Property("displayName").Value.ToString();
            ToReturn.IsVaccineDataAvailable = Convert.ToBoolean(jo.Property("isVaccineDataAvailable").Value.ToString());
            ToReturn.Cases = ToReturn.TryGetValue_uint(jo, "totalConfirmed");
            ToReturn.Deaths = ToReturn.TryGetValue_uint(jo, "totalDeaths");
            ToReturn.Recovered = ToReturn.TryGetValue_uint(jo, "totalRecovered");
            ToReturn.RecoveredDelta = ToReturn.TryGetValue_uint(jo, "totalRecoveredDelta");
            ToReturn.DeathsDelta = ToReturn.TryGetValue_uint(jo, "totalDeathsDelta");
            ToReturn.CasesDelta = ToReturn.TryGetValue_uint(jo, "totalConfirmedDelta");
            ToReturn.DosesAdministered = ToReturn.TryGetValue_ulong(jo, "totalDosesAdministered");
            ToReturn.DosesDistributed = ToReturn.TryGetValue_ulong(jo, "totalDosesDistributed");
            ToReturn.DosesWithAtLeast1Dose = ToReturn.TryGetValue_float(jo, "totalDosesWithAtleast1Dose");
            ToReturn.DosesWithAtLeast2Dose = ToReturn.TryGetValue_float(jo, "totalDosesWithAtleast2Dose");
            ToReturn.DosesPer100People = ToReturn.TryGetValue_float(jo, "totalDosesPer100People");
            ToReturn.SupplyUsed = ToReturn.TryGetValue_float(jo, "totalSupplyUsed");
            ToReturn.LastUpdated = DateTime.Parse(jo.Property("lastUpdated").Value.ToString());

            //Get lat and long
            JProperty prop_lat = jo.Property("lat");
            JProperty prop_long = jo.Property("long");
            if (prop_lat != null)
            {
                ToReturn.Latitude = Convert.ToSingle(jo.Property("lat").Value.ToString());
            }
            else
            {
                ToReturn.Latitude = null;
            }
            if (prop_long != null)
            {
                ToReturn.Longitude = Convert.ToSingle(jo.Property("long").Value.ToString());
            }
            else
            {
                ToReturn.Longitude = null;
            }
            
            //Get parent id
            JProperty prop_parentId = jo.Property("parentId");
            if (prop_parentId != null)
            {
                ToReturn.ParentId = jo.Property("parentId").Value.ToString();
            }
            else
            {
                ToReturn.ParentId = null;
            }
            
            //Get children
            JProperty prop_areas = jo.Property("areas");
            List<VaccinationAreaData> ToAdd = new List<VaccinationAreaData>();
            JArray ja = JArray.Parse(prop_areas.Value.ToString());
            foreach (JObject ca in ja)
            {
                ToAdd.Add(VaccinationAreaData.Load(ca.ToString()));
            }
            ToReturn.ChildAreas = ToAdd.ToArray();

            return ToReturn;
        }

        private uint? TryGetValue_uint(JObject obj, string property_name)
        {
            JProperty prop = obj.Property(property_name);
            if (prop == null)
            {
                throw new Exception("Object does not have property '" + property_name + "'");
            }
            if (prop.Value.Type == JTokenType.Null)
            {
                return null;
            }
            else
            {
                return Convert.ToUInt32(prop.Value.ToString());
            }
        }

        private ulong? TryGetValue_ulong(JObject obj, string property_name)
        {
            JProperty prop = obj.Property(property_name);
            if (prop == null)
            {
                throw new Exception("Object does not have property '" + property_name + "'");
            }
            if (prop.Value.Type == JTokenType.Null)
            {
                return null;
            }
            else
            {
                return Convert.ToUInt64(prop.Value.ToString());
            }
        }

        private float? TryGetValue_float(JObject obj, string property_name)
        {
            JProperty prop = obj.Property(property_name);
            if (prop == null)
            {
                throw new Exception("Object does not have property '" + property_name + "'");
            }
            if (prop.Value.Type == JTokenType.Null)
            {
                return null;
            }
            else
            {
                return Convert.ToSingle(prop.Value.ToString());
            }
        }
    }
}
