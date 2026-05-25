namespace Zent.Common;

public static class ErrorCodes
{
    // general
    public const string InternalServerError = "internal_server_error";
    public const string ValidationFailed = "validation_failed";
    
    // user
    public const string InvalidCredentials = "invalid_credentials";
    public const string UserAlreadyExists = "user_already_exists";
    public const string UserNotFound = "user_not_found";
    
    // team
    public const string TeamAlreadyExists = "team_already_exists";
    public const string TeamNotFound = "team_not_found";
    public const string TeamAccessDenied = "team_access_denied";
    public const string TeamMemberAlreadyExists = "team_member_already_exists";
    
    // project
    public const string ProjectAlreadyExists = "project_already_exists";
    public const string ProjectNotFound = "project_not_found";
    public const string ProjectAccessDenied = "project_access_denied";
    
    // board
    public const string BoardAlreadyExists = "board_already_exists";
    public const string BoardAccessDenied = "board_access_denied";
    public const string BoardNotFound = "board_not_found";
    
    // column
    public const string ColumnNotFound = "column_not_found";
    public const string ColumnAlreadyExists = "column_already_exists";
    
    // task
    public const string TaskNotFound = "task_not_found";
}