namespace Zent.Application.Features.Tasks.GetTaskDetails;

public sealed record TaskAssigneeDto(Guid UserId, string FirstName, string LastName, string Email);