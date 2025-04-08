using AutoMapper;
using MemApp.Application.Mem.Members.Models;
using MemApp.Domain.Entities.mem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.AutoMapper
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<RegisterMember, MemberRes>().ReverseMap();
         //   CreateMap<List<RegisterMember>, List<MemberRes>>().ReverseMap();

        }
    }
}
