using AutoMapper;
using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Repository.Dto;
using TripleZero.Repository.Repository.Dto;

namespace TripleZero.Repository._Mapping
{
    internal class MappingConfiguration : IMappingConfiguration
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
                    .ForMember(dest => dest.Arena, src => src.MapFrom(source => source.Arena))
                    .ForMember(dest => dest.LoadedFromCache, src => src.Ignore())
                    ;

                    //guildDto to guild
                    cfg.CreateMap<GuildDto, Guild>()
                    .ForMember(dest => dest.GalacticPower, src => src.MapFrom(source => source.GP))
                    .ForMember(dest => dest.GalacticPowerAverage, src => src.MapFrom(source => source.GPaverage))
                    .ForMember(dest => dest.EntryUpdateDate, src => src.MapFrom(source => source.LastClassUpdated))
                    .ForMember(dest => dest.SWGoHUpdateDate, src => src.MapFrom(source => source.LastSwGohUpdated))
                    .ForMember(dest => dest.Players, src => src.MapFrom(source => source.Players))
                    .ForMember(dest => dest.LoadedFromCache, src => src.Ignore())
                    ;

                    //characterDto to Character
                    cfg.CreateMap<CharacterAbilityDto, SWGoH.Model.Ability>();

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

                    cfg.CreateMap<ModStat, SWGoH.Model.ModStat>();

                    cfg.CreateMap<ModDto, SWGoH.Model.Mod>()
                    .ForMember(dest => dest.PrimaryStat, src => src.MapFrom(source => source.PrimaryStat))
                    .ForMember(dest => dest.SecondaryStat, src => src.MapFrom(source => source.SecondaryStat))
                    ;

                    cfg.CreateMap<CharacterDto, Character>()
                    .ForMember(dest => dest.Abilities, src => src.MapFrom(s => s.Abilities))
                    .ForMember(dest => dest.GeneralStats, src => src.MapFrom(s => s))
                    .ForMember(dest => dest.OffenseStats, src => src.MapFrom(s => s))
                    .ForMember(dest => dest.Survivability, src => src.MapFrom(s => s))                    
                    ;

                    //shipDto to ship
                    cfg.CreateMap<ShipAbilityDto, SWGoH.Model.Ability>();

                    cfg.CreateMap<ShipDto, GeneralStats>();

                    cfg.CreateMap<ShipDto, OffenseStats>()
                    .ForMember(dest => dest.PhysicalOffense, src => src.MapFrom(s => s))
                    .ForMember(dest => dest.SpecialOffense, src => src.MapFrom(s => s))
                    ;

                    cfg.CreateMap<ShipDto, PhysicalOffense>();
                    cfg.CreateMap<ShipDto, SpecialOffense>();

                    cfg.CreateMap<ShipDto, Survivability>()
                    .ForMember(dest => dest.PhysicalSurvivability, src => src.MapFrom(s => s))
                    .ForMember(dest => dest.SpecialSurvivability, src => src.MapFrom(s => s))
                    ;

                    cfg.CreateMap<ShipDto, PhysicalSurvivability>();
                    cfg.CreateMap<ShipDto, SpecialSurvivability>();                    

                    cfg.CreateMap<ShipDto, Ship>()
                    .ForMember(dest => dest.Abilities, src => src.MapFrom(s => s.Abilities))
                    .ForMember(dest => dest.GeneralStats, src => src.MapFrom(s => s))
                    .ForMember(dest => dest.OffenseStats, src => src.MapFrom(s => s))
                    .ForMember(dest => dest.Survivability, src => src.MapFrom(s => s))                    
                    ;

                    cfg.CreateMap<ArenaDto, Arena>()                    
                    ;

                    //queue
                    cfg.CreateMap<QueueDto, Queue>()
                    .ForMember(dest => dest.InsertDate, src => src.MapFrom(source => source.InsertedDate))
                    .ForMember(dest => dest.ProcessingBy, src => src.MapFrom(source => source.ComputerName))
                    .ForMember(dest => dest.GuildName, src => src.MapFrom(source => source.Guild))
                    ;

                    //character config
                    cfg.CreateMap<CharacterConfigDto, CharacterConfig>()
                    .ForMember(dest => dest.LoadedFromCache, src => src.Ignore())
                    ;

                    //ship config
                    cfg.CreateMap<ShipConfigDto, ShipConfig>()
                    .ForMember(dest => dest.LoadedFromCache, src => src.Ignore())
                    ;

                    //guild config
                    cfg.CreateMap<GuildConfigDto, GuildConfig>()
                    .ForMember(dest => dest.LoadedFromCache, src => src.Ignore())
                    ;

                    //guildCharacter
                    cfg.CreateMap<GuildUnitDto, GuildUnit>()
                    .ForMember(dest => dest.Name, src => src.MapFrom(source => source.Name))
                    .ForMember(dest => dest.LoadedFromCache, src => src.Ignore())
                    ;

                    //guildPlayerCharacter
                    cfg.CreateMap<GuildPlayerUnitDto, GuildPlayerUnit>()
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
