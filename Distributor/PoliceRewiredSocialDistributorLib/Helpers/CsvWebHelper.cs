using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CsvHelper;

namespace PoliceRewiredSocialDistributorLib.Helpers
{
    public class CsvWebHelper
    {
        public static async Task<List<DTO>> ReadDataAsync<DTO>(string url)
        {
            using (var client = new WebClient())
            using (var stream = await client.OpenReadTaskAsync(url))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<DTO>().ToList();
            }
        }
    }
}
