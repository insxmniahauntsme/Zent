namespace Zent.API.Endpoints.Tasks.GetTaskDetails;

public sealed record GetTaskDetailsRequest(Guid BoardId, Guid TaskId);