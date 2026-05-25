using Zent.Common.Enums;

namespace Zent.Application.Features.Teams;

public sealed record TeamMemberRoleEntry(Guid UserId, TeamRole Role);