using Zent.Common.Enums;

namespace Zent.Application.Features.Projects.GetProjectMembers;

public sealed record ProjectMemberDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    TeamRole Role,
    Specialization? Specialization);