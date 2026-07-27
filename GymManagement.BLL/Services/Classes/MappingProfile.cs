using AutoMapper;
using GymManagement.BLL.ViewModels.HealthRecordViewModels;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;


namespace GymManagement.BLL.Services.Classes
{
    public class MappingProfile : Profile
    {


        public MappingProfile()
        {

            // Member Mapping
            CreateMap<Member, MemberViewModel>()
             .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNo} - {src.Address.Street} - {src.Address.City}"))
             .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()));

            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address()
                {
                    City = src.City,
                    BuildingNo = src.BuildingNumber,
                    Street = src.Street
                }
                ));

            CreateMap<Member, MemberToUpdateViewModel>()
                .AfterMap((src, dest) =>
                {
                    dest.Street = src.Address.Street;
                    dest.City = src.Address.City;
                    dest.BuildingNumber = src.Address.BuildingNo;
                });

            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Address.Street = src.Street;
                    dest.Address.City = src.City;
                    dest.Address.BuildingNo = src.BuildingNumber;
                });


            // Plan Mapping
            CreateMap<Plan, PlanViewModel>().ForMember(dest => dest.duration, opt => opt.MapFrom(src => src.DurationDays));

            CreateMap<Plan, PlanDetailsViewModel>().ForMember(dest => dest.duration, opt => opt.MapFrom(src => src.DurationDays));

            CreateMap<Plan, PlanToUpdateViewModel>().ForMember(dest => dest.duration, opt => opt.MapFrom(src => src.DurationDays));

            CreateMap<PlanToUpdateViewModel, Plan>().ForMember(dest => dest.Name, opt => opt.Ignore()).ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => src.duration));


            // Trainer Mapping

            CreateMap<Trainer, TrainerViewModel>().ReverseMap();

            CreateMap<Trainer, TrainerDetailsViewModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(
                    src => $"{src.Address.City} - {src.Address.Street} - {src.Address.BuildingNo}"));

            CreateMap<CreateTrainerViewModel, Trainer>().AfterMap(
                (src, dest) => dest.Address = new Address
                {
                    City = src.City,
                    BuildingNo = src.BuildingNumber,
                    Street = src.Street
                });

            CreateMap<Trainer, TrainerToUpdateViewModel>().AfterMap(
                (src, dest) =>
                {
                    dest.Street = src.Address.Street;
                    dest.City = src.Address.City;
                    dest.BuildingNumber = src.Address.BuildingNo;
                });

            CreateMap<TrainerToUpdateViewModel, Trainer>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.DateOfBirth, opt => opt.Ignore())
                .ForMember(dest => dest.Gender, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Address.City = src.City;
                    dest.Address.BuildingNo = src.BuildingNumber;
                    dest.Address.Street = src.Street;
                });

            
           // Session Mapping

            CreateMap<Session, SessionViewModel>().ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name));

            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();

            CreateMap<CreateSessionViewModel, Session>().ReverseMap();

            CreateMap<Trainer, TrainerSelectViewModel>().ReverseMap();

            CreateMap<Category, CategorySelectViewModel>().ReverseMap();


            // Membership Mapping

            CreateMap<MemberPlans, MembershipViewModel>().ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreatedAt))
                .AfterMap((src,dest) =>
                {
                    dest.MemberName = src.Member.Name;
                    dest.MermberId = src.Member.Id;
                    dest.PlanName = src.Plan.Name;
                });

            CreateMap<CreateMembershipViewModel,MemberPlans>();
            CreateMap<Plan, PlanSelectListViewModel>();
            CreateMap<Member, MemberSelectListViewModel>();
        }
    }
}
