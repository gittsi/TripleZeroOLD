using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Repository.Dto;

namespace TripleZero._Mapping
{
    public class MappingConfiguration : IMappingConfiguration
    {
        private IMapper _Mapper;
        public MappingConfiguration()
        {
            _Mapper = GetConfigureMapper();
        }

        public IMapper GetConfigureMapper()
        {
            if (_Mapper != null)
                return _Mapper;
            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    //cfg.CreateMap<GuildPlayerCharacterDto, Model.CharacterStats>()
                    //.ForMember(dest => dest.Level, src => src.MapFrom(source => source.Level))
                    //.ForMember(dest => dest.Power, src => src.MapFrom(source => source.Power))
                    //.ForMember(dest => dest.Rarity, src => src.MapFrom(source => source.Rarity))
                    //;

                    //cfg.CreateMap<GuildCharacterDto, Model.Character>()
                    //.ForMember(dest => dest.Name, src => src.MapFrom(source => source.Name))
                    //.ForMember(dest => dest.Stats, src => src.Ignore())
                    //;

                    ////.ForMember(dest => dest.Tax, c => c.MapFrom(src => !string.IsNullOrWhiteSpace(src.TaxCode) ? new TaxInfo() { TaxCode = src.TaxCode, TaxOffice = src.TaxOffice } : null))

                    //cfg.CreateMap<GuildCharacterDto, Model.GuildCharacter>()
                    //.ForMember(dest => dest.Character, src => src.MapFrom(source => new Model.Character() { Name = source.Name })
                    //);

                    cfg.AllowNullDestinationValues = true;
                    cfg.AllowNullCollections = true;
                });

                config.AssertConfigurationIsValid();
                return config.CreateMapper();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
