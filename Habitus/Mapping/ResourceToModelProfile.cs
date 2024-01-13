using AutoMapper;
using Habitus.Domain.Models;
using Habitus.Requests;

namespace Habitus.Mapping;

public class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        CreateMap<SaveCategoryRequest, Category>();
        CreateMap<SaveHabitRequest, Habit>();
    }
}
