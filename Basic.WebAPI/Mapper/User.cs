using AutoMapper;
using Basic.Model;
using Basic.WebAPI.ViewModels;

namespace Basic.WebAPI.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, SignupVm>();
            CreateMap<SignupVm, User>();
        }
    }
}
