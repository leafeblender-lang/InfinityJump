using System;
using Newtonsoft.Json;
using Postgrest.Models;
using Postgrest.Attributes;

[Serializable]
[Table("users")]
public class User : BaseModel
{
    [PrimaryKey("id", false)]  // ? DODAJ OVO - false zna?i "auto-generated"
    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("referral_code")]
    public string ReferralCode { get; set; }

    [JsonProperty("device_id")]
    public string DeviceId { get; set; }

    [JsonProperty("diamonds")]
    public int Diamonds { get; set; }
}