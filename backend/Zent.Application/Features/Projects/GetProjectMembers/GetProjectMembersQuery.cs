using Zent.Application.Messaging.Abstractions;

namespace Zent.Application.Features.Projects.GetProjectMembers;

public sealed record GetProjectMembersQuery(Guid UserId, Guid ProjectId) : IQuery<IReadOnlyList<ProjectMemberDto>>;