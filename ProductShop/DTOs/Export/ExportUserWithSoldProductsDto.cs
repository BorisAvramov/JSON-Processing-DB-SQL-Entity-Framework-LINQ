using Newtonsoft.Json;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductShop.DTOs.Export
{
    public class ExportUserWithSoldProductsDto
    {
        [JsonProperty("firstName")]
        public string? UserFirstName { get; set;}

        [JsonProperty("lastName")]
        public string UserLastName { get; set; } = null!;

        [JsonProperty("soldProducts")]
        public ICollection<ExportSoldProductDto> UserSoldProducts { get; set; }

    }
}
