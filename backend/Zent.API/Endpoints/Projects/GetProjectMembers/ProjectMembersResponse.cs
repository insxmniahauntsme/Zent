using Zent.Application.Features.Projects.GetProjectMembers;

namespace Zent.API.Endpoints.Projects.GetProjectMembers;

public sealed record ProjectMembersResponse(IReadOnlyList<ProjectMemberDto> Members);