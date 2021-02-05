using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Zasm.Models;

namespace Zasm.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Student, StudentInputModel>();
            CreateMap<StudentInputModel, Student>().ForMember(i => i.Id, opt => opt.Ignore());
        }
    }
}
