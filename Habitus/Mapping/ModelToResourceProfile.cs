using AutoMapper;
using Habitus.Models;
using Habitus.Resources;

namespace Habitus.Mapping;

public class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<Category, CategoryResource>();
    }
}
