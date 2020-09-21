using Azure.Search.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AzureSearch.SDK.Quickstart.v11
{
    public class Hotel
    {
        [JsonPropertyName("hotelId")]
        [SimpleField(IsKey = true, IsFilterable =true, IsSortable = true)]
        public string Id { get; set; }


        [JsonPropertyName("hotelName")]
        [SearchableField(IsFilterable = true, IsSortable = true)]
        public string Name { get; set; }

        [JsonPropertyName("hotelCategory")]
        [SearchableField(IsFilterable = true, IsSortable = true)]
        public string Category { get; set; }

        [JsonPropertyName("baseRate")]
        [SimpleField(IsFilterable = true, IsSortable = true)]
        public Int32 Rate { get; set; }

        [JsonPropertyName("lastRenovationDate")]
        [SimpleField(IsFilterable = true, IsSortable = true)]
        public DateTime Updated { get; set; }

        [FieldBuilderIgnore]
        [JsonIgnore]
        public IEnumerable<Room> Rooms { get; set; }
    }
}