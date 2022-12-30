using AutoMapper;
using CodeCompetitionBackendCSharp.Dto;
using CodeCompetitionBackendCSharp.Models;

namespace CodeCompetitionBackendCSharp.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Models.Task, TaskDto>();
            CreateMap<TaskCategory, TaskCategoryDto>();
        }
    }
}
