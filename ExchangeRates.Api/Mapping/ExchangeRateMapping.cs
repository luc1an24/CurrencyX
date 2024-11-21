using AutoMapper;
using ExchangeRates.Api.Dto;
using ExchangeRates.Shared.Models;

namespace ExchangeRates.Api.Mapping
{
    public class ExchangeRateMapping : Profile
    {
        public ExchangeRateMapping()
        {
            CreateMap<ExchangeRateCreateDto, ExchangeRate>()
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.CurrencyCode))
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));

            CreateMap<ExchangeRateUpdateDto, ExchangeRate>()
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate));

            CreateMap<ExchangeRate, ExchangeRateCreateDto>();
            CreateMap<ExchangeRate, ExchangeRateUpdateDto>();
        }
    }
}
