using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlenMe.DataModel
{
    //[JsonObject("content")]
    public class Content
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Data { get; set; }
    }
}
