using AutoMapper;
using Askly.Domain;
using Askly.Application.DTOs;
using Askly.Application.DTOs.Polls;
using Askly.Application.DTOs.Users;

namespace Askly.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<PollEntity, PollDto>();
        CreateMap<CreatePollDto, PollEntity>();
        CreateMap<CreateOptionDto, OptionEntity>();
        CreateMap<OptionEntity, OptionDto>();
        CreateMap<UserEntity, UserProfileDto>();
        CreateMap<OptionEntity, VoteResultsDto>();
        CreateMap<Tuple<Guid, int>, VoteResultsDto>()
            .ForMember(
                d => d.OptionId,
                opt => opt.MapFrom(s => s.Item1))
            .ForMember(
                d => d.VotesCount,
                opt => opt.MapFrom(s => s.Item2));

        // CreateMap<PollEntity, PollResultsDto>();
    }
}