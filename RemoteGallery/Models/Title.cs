using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteGallery.Models
{
    public partial class Title : BindableBase
    {
        [JsonProperty("revision")]
        public long Revision { get; set; }

        [JsonProperty("formatVersion")]
        public long FormatVersion { get; set; }

        [JsonProperty("npTitleId")]
        public string NpTitleId { get; set; }

        [JsonProperty("console")]
        public string Console { get; set; }

        [JsonProperty("names")]
        public Name[] Names { get; set; }

        [JsonProperty("icons")]
        public Icon[] Icons { get; set; }

        [JsonProperty("parentalLevel")]
        public long ParentalLevel { get; set; }

        [JsonProperty("pronunciation")]
        public Uri Pronunciation { get; set; }

        [JsonProperty("contentId")]
        public string ContentId { get; set; }

        [JsonProperty("backgroundImage")]
        public Uri BackgroundImage { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("psVr")]
        public long PsVr { get; set; }

        [JsonProperty("neoEnable")]
        public long NeoEnable { get; set; }
    }

    public partial class Icon
    {
        [JsonProperty("icon")]
        public Uri IconIcon { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Name
    {
        [JsonProperty("name")]
        public string NameName { get; set; }
    }
}
