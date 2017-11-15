using AutoMapper;
using SWGoH;
using SWGoH.Model;
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
                    //playerDto to player
                    cfg.CreateMap<PlayerDto, Player>()
                    .ForMember(dest => dest.EntryUpdateDate, src => src.MapFrom(source => source.LastSwGohUpdated))
                    .ForMember(dest => dest.SWGoHUpdateDate, src => src.MapFrom(source => source.LastSwGohUpdated))
                    .ForMember(dest => dest.GalacticPowerCharacters, src => src.MapFrom(source => source.GPcharacters))
                    .ForMember(dest => dest.GalacticPowerShips, src => src.MapFrom(source => source.GPships))
                    ;

                    //guildDto to guild
                    cfg.CreateMap<GuildDto, Guild>()
                    .ForMember(dest => dest.GalacticPower, src => src.MapFrom(source => source.GP))
                    .ForMember(dest => dest.GalacticPowerAverage, src => src.MapFrom(source => source.GPaverage))
                    .ForMember(dest => dest.EntryUpdateDate, src => src.MapFrom(source => source.LastClassUpdated))
                    .ForMember(dest => dest.SWGoHUpdateDate, src => src.MapFrom(source => source.LastSwGohUpdated))
                    .ForMember(dest => dest.Players, src => src.MapFrom(source => source.Players))
                    ;

                    //characterDto to Character
                    cfg.CreateMap<SWGoH.Ability, SWGoH.Model.Ability>();

                    cfg.CreateMap<CharacterDto, GeneralStats>();

                    cfg.CreateMap<CharacterDto, OffenseStats>()
                    .ForMember(dest => dest.PhysicalOffense, src => src.MapFrom(s => s))
                    .ForMember(dest => dest.SpecialOffense, src => src.MapFrom(s => s))
                    ;

                    cfg.CreateMap<CharacterDto, PhysicalOffense>();
                    cfg.CreateMap<CharacterDto, SpecialOffense>();                    

                    cfg.CreateMap<CharacterDto, Survivability>()
                    .ForMember(dest => dest.PhysicalSurvivability, src => src.MapFrom(s => s))
                    .ForMember(dest => dest.SpecialSurvivability, src => src.MapFrom(s => s))
                    ;

                    cfg.CreateMap<CharacterDto, PhysicalSurvivability>();
                    cfg.CreateMap<CharacterDto, SpecialSurvivability>();

                    cfg.CreateMap<SWGoH.ModStat, SWGoH.Model.ModStat>();

                    cfg.CreateMap<SWGoH.Mod, SWGoH.Model.Mod>()
                    .ForMember(dest => dest.PrimaryStat, src => src.MapFrom(source => source.PrimaryStat))
                    .ForMember(dest => dest.SecondaryStat, src => src.MapFrom(source => source.SecondaryStat))
                    ;

                    cfg.CreateMap<CharacterDto, Character>()
                    .ForMember(dest=> dest.Abilities , src => src.MapFrom(s=>s.Abilities))                    
                    .ForMember(dest => dest.GeneralStats, src => src.MapFrom(s => s))
                    .ForMember(dest => dest.OffenseStats, src => src.MapFrom(s => s))
                    .ForMember(dest => dest.Survivability, src => src.MapFrom(s => s))                    
                    ;

                    //queue
                    cfg.CreateMap<QueueDto, Queue>()
                    .ForMember(dest => dest.InsertDate, src => src.MapFrom(source => source.InsertedDate))
                    ;

                    //character config
                    cfg.CreateMap<CharacterConfigDto, CharacterConfig>();

                    //guild config
                    cfg.CreateMap<GuildConfigDto, GuildConfig>();

                    //guildCharacter
                    cfg.CreateMap<GuildCharacterDto, GuildCharacter>()
                    .ForMember(dest => dest.CharacterName, src => src.MapFrom(source => source.Name))
                    ;

                    //guildPlayerCharacter
                    cfg.CreateMap<GuildPlayerCharacterDto, GuildPlayerCharacter>()
                    .ForMember(dest => dest.PlayerName, src => src.MapFrom(source => source.Name))
                    .ForMember(dest => dest.CombatType, src => src.MapFrom(source => source.Combat_Type))
                    ;

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
