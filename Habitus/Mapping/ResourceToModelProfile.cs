using AutoMapper;
using Habitus.Domain.Models;
using Habitus.Resources;

namespace Habitus.Mapping;

public class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        CreateMap<SaveCategoryResource, Category>();
    }
}
