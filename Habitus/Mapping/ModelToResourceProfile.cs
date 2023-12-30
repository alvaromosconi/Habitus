using AutoMapper;
using Habitus.Domain.Models;
using Habitus.Resources;

namespace Habitus.Mapping;

public class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<Category, CategoryResource>();
        CreateMap<Habit, HabitResource>();
    }
}
