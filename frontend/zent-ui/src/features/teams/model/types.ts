export type TeamRole = "Owner" | "Admin" | "Member";

export interface UserTeamDto {
  id: string;
  ownerId: string;
  name: string;
  role: TeamRole;
  membersCount: number;
  projectsCount: number;
}

export type Specialization =
  | "Backend"
  | "Frontend"
  | "Fullstack"
  | "QA"
  | "Designer"
  | "PM"
  | "DevOps";

export interface TeamMemberDto {
  userId: string;
  firstName: string;
  lastName: string;
  email: string;
  role: TeamRole;
  specialization: Specialization | null;
}

export interface TeamProjectDto {
  projectId: string;
  name: string;
  description?: string;
  client?: string;
  membersCount: number;
  boardsCount: number;
}

export interface GetTeamMembersResponse {
  members: TeamMemberDto[];
}

export interface TeamDto {
  id: string;
  ownerId: string;
  name: string;
  currentUserRole: TeamRole;
  membersCount: number;
  projectsCount: number;
}

export interface GetTeamResponse {
  team: TeamDto;
}

export interface TeamsResponse {
  teams: UserTeamDto[];
}

export interface TeamMemberRoleEntry {
  userId: string;
  role: Exclude<TeamRole, "Owner">;
}

export interface AddTeamRequest {
  name: string;
  members?: TeamMemberRoleEntry[];
}

export interface AddTeamMembersRequest {
  members: TeamMemberRoleEntry[];
}
