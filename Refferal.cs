using System;
using Newtonsoft.Json;
using Postgrest.Models;
using Postgrest.Attributes;

[Serializable]
[Table("referrals")]
public class Referral : BaseModel
{
    [PrimaryKey("id", false)]  // ? DODAJ OVO
    [JsonProperty("id")]
    public string id { get; set; }

    [JsonProperty("referrer_id")]
    public string referrer_id { get; set; }

    [JsonProperty("referred_id")]
    public string referred_id { get; set; }

    [JsonProperty("created_at")]
    public DateTime? created_at { get; set; }
}