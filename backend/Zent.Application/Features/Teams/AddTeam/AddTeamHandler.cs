using Microsoft.EntityFrameworkCore;
using Npgsql;
using Zent.Application.Messaging.Abstractions;
using Zent.Common.Enums;
using Zent.Common.Exceptions;
using Zent.Data;
using Zent.Data.Entities;

namespace Zent.Application.Features.Teams.AddTeam;

internal sealed class AddTeamHandler(ZentDbContext dbContext) : ICommandHandler<AddTeamCommand, Guid>
{
    private const string TeamsNameOwnerIdIndexName = "IX_teams_name_owner_id";

    public async Task<Guid> Handle(AddTeamCommand command, CancellationToken ct)
    {
        try
        {
            var entity = command.ToEntity();

            dbContext.Teams.Add(entity);

            var teamId = entity.Id;

            var teamCreatorEntity = new TeamMemberEntity
            {
                UserId = command.CreatorId,
                TeamId = teamId,
                MemberRole = TeamRole.Owner
            };

            dbContext.TeamMembers.Add(teamCreatorEntity);
            await dbContext.SaveChangesAsync(ct);

            if (command.Members is null || command.Members.Count == 0)
            {
                await dbContext.SaveChangesAsync(ct);
                return teamId;
            }

            var normalizedMembers = command.Members
                .Where(x => x.UserId != command.CreatorId)
                .GroupBy(x => x.UserId)
                .Select(g => g.First())
                .ToList();

            var memberIds = normalizedMembers
                .Select(x => x.UserId)
                .ToList();

            var existingUserIds = await dbContext.Users
                .Where(x => memberIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(ct);

            if (existingUserIds.Count != memberIds.Count)
            {
                throw new UserNotFoundException("One or more users were not found.");
            }

            dbContext.TeamMembers.AddRange(
                normalizedMembers.Select(x => new TeamMemberEntity
                {
                    UserId = x.UserId,
                    TeamId = teamId,
                    MemberRole = x.Role
                }));

            await dbContext.SaveChangesAsync(ct);

            return teamId;
        }
        catch (DbUpdateException ex) when (IsUniqueNameException(ex))
        {
            throw new TeamAlreadyExistsException("Team with this name and owner id already exists.");
        }
    }

    private static bool IsUniqueNameException(DbUpdateException ex)
        => ex.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: TeamsNameOwnerIdIndexName
        };
}