using Zent.Common.Enums;

namespace Zent.Application.Features.Teams;

public record TeamMemberRoleEntry(Guid UserId, TeamRole Role);