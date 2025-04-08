using MemApp.Application.Mem.Colleges.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Interfaces
{
    public interface ICsvFileBuilder
    {
        public byte[] BuildCollegesFile<T>(IEnumerable<T> records);
    }
}
