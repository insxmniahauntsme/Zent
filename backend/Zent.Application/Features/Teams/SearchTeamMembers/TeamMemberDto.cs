using Zent.Common.Enums;

namespace Zent.Application.Features.Teams.SearchTeamMembers;

public sealed record TeamMemberDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    TeamRole Role,
    Specialization? Specialization);