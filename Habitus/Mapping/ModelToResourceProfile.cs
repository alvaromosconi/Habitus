using AutoMapper;
using Habitus.Domain.Models;
using Habitus.Domain.Models.Auth;
using Habitus.Resources;

namespace Habitus.Mapping;

public class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<HabitusUser, AuthenticateResponse>();
        CreateMap<Category, CategoryResource>();
        CreateMap<Habit, HabitResource>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => new HabitusUserResource { UserId = src.User.Id, UserName = src.User.UserName }));
        // CreateMap<HabitusUser, HabitusUserResource>();
    }
}