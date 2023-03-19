using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductShop.DTOs.Export
{
    public class ExportSoldProductDto
    {
        [JsonProperty("name")]
        public string SoldProductName { get; set; } = null!;
        [JsonProperty("price")]
        public decimal SoldProducPrice { get; set; }
        [JsonProperty("buyerFirstName")]
        public string? SoldProducBuyerFirstName { get; set; }
        [JsonProperty("buyerLastName")]
        public string SoldProducBuyerLastName { get; set; } = null!;


    }
}
