using AutoMapper;
using TaskManagement.DTOs;
using TaskManagement.Models;

namespace TaskManagement.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping CreateUpdateTaskDTO to TaskItem and ignore CreatedDate and ReporterId fields

            CreateMap<CreateUpdateTaskDTO, TaskItem>()
                 .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                 .ForMember(dest => dest.ReporterId, opt => opt.Ignore());
        }
    }
}
