namespace Zent.API.Endpoints.Projects.AddProject;

public sealed record AddProjectRequest(string Name, string? Description, string? Client, List<Guid>? Members);