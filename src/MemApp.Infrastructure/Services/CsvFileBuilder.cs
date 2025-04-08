using CsvHelper;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Colleges.Models;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Infrastructure.Services
{
    public class CsvFileBuilder : ICsvFileBuilder
    {
        public byte[] BuildCollegesFile<T>(IEnumerable<T> records)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
                csvWriter.WriteRecords(records);
            }
            return memoryStream.ToArray();
        }
    }
}
