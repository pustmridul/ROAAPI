using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Interfaces
{
    public interface IFileSaveService
    {
        Task<bool> UploadFile(FileSaveModel model);
    }
}
