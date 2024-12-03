using AutoMapper;
using RoadReady.Models;
using RoadReady.Models.DTO;

namespace RoadReady
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping for Car
            CreateMap<Car, CarDTO>().ReverseMap();  // Car to CarDTO and vice versa
            CreateMap<Car, CreateCarDTO>().ReverseMap();  // Car to CarCreateDTO and vice versa
            CreateMap<Car, UpdateCarDTO>().ReverseMap();  // Car to CarUpdateDTO and vice versa

            // Mapping for CarExtra
            CreateMap<CarExtra, CarExtraDTO>().ReverseMap();  // CarExtra to CarExtraDTO and vice versa
            CreateMap<CarExtra, CreateCarExtraDTO>().ReverseMap();  // CarExtra to CarExtraCreateDTO and vice versa
            CreateMap<CarExtra, UpdateCarExtraDTO>().ReverseMap();  // CarExtra to CarExtraUpdateDTO and vice versa

            // Mapping for Payment
            CreateMap<Payment, PaymentDTO>().ReverseMap();  // Payment to PaymentDTO and vice versa
            CreateMap<CreatePaymentDTO, Payment>();  // CreatePaymentDTO to Payment
            CreateMap<Payment, UpdatePaymentDTO>().ReverseMap();  // Payment to PaymentUpdateDTO and vice versa

            // Mapping for Reservation
            CreateMap<Reservation, ReservationDTO>()
             .ForMember(dest => dest.Extras, opt => opt.MapFrom(src => src.Extras.Select(ce => new CarExtraDTO
             {
                 ExtraId = ce.ExtraId,
                 Name = ce.Name,
                 Price = (decimal)ce.Price
             }).ToList()));

            CreateMap<Reservation, CreateReservationDTO>().ReverseMap();  // Reservation to ReservationCreateDTO and vice versa
            CreateMap<Reservation, UpdateReservationDTO>().ReverseMap();  // Reservation to ReservationUpdateDTO and vice versa

            // Mapping for User
            CreateMap<User, Models.DTO.UserDTO>().ReverseMap();  // User to UserDTO and vice versa
            CreateMap<User, CreateUserDTO>().ReverseMap();  // User to UserCreateDTO and vice versa
            CreateMap<User, UpdateUserDTO>().ReverseMap();  // User to UserUpdateDTO and vice versa

            // Mapping for Review
            CreateMap<Review, ReviewsDTO>().ReverseMap();  // Review to ReviewDTO and vice versa
            CreateMap<Review, CreateReviewsDTO>().ReverseMap();  // Review to ReviewCreateDTO and vice versa
            CreateMap<Review, UpdateReviewsDTO>().ReverseMap();  // Review to ReviewUpdateDTO and vice versa

            CreateMap<PasswordReset, PasswordResetDTO>().ReverseMap();
            /*CreateMap<PasswordReset, PasswordResetRequestDTO>().ReverseMap();
             CreateMap<PasswordReset, PasswordResetResponseDTO>().ReverseMap();

             // Mapping for PasswordReset to PasswordResetCreateDTO and vice versa
             CreateMap<PasswordReset, PasswordResetCreateDTO>().ReverseMap();

             // Mapping for PasswordReset to PasswordResetUpdateDTO and vice versa
             CreateMap<PasswordReset, PasswordResetUpdateDTO>().ReverseMap();*/
        }
    }
}