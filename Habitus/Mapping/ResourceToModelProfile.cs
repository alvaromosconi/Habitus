using AutoMapper;
using Habitus.Models;
using Habitus.Resources;

namespace Habitus.Mapping;

public class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        CreateMap<SaveCategoryResource, Category>();
    }
}
