// import { useMemo, useState } from "react";
// import { useMutation, useQueryClient } from "@tanstack/react-query";
// import { useParams } from "react-router-dom";

// import { useTeamMembers } from "@/features/teams/hooks/useTeamMembers";
// import type { TeamMemberDto, TeamRole } from "@/features/teams/model/types";

// import { teamsApi } from "@/features/teams/api/teamsApi";
// import InviteTeamMemberModal from "@/features/teams/components/InviteTeamMemberModal/InviteTeamMemberModal";
// import type { SelectedInviteMember } from "@/features/teams/components/InviteTeamMemberSelector/InviteTeamMemberSelector";

// import styles from "./TeamMembersPage.module.css";

// const roleLabels: Record<TeamRole, string> = {
//   Owner: "Owner",
//   Admin: "Admin",
//   Member: "Member",
// };

// const roleClassNames: Record<TeamRole, string> = {
//   Owner: styles.roleOwner,
//   Admin: styles.roleAdmin,
//   Member: styles.roleMember,
// };

// const TeamMembersPage = () => {
//   const { teamId } = useParams<{ teamId: string }>();

//   const { data: members = [], isLoading, isError } = useTeamMembers(teamId);

//   const [search, setSearch] = useState("");

//   const queryClient = useQueryClient();

//   const [isInviteModalOpen, setIsInviteModalOpen] = useState(false);
//   const [inviteError, setInviteError] = useState<string | null>(null);

//   const addTeamMembersMutation = useMutation({
//     mutationFn: (members: SelectedInviteMember[]) => {
//       return teamsApi.addTeamMembers(teamId, {
//         members: members.map((member) => ({
//           userId: member.userId,
//           role: member.role,
//         })),
//       });
//     },

//     onSuccess: async () => {
//       await queryClient.invalidateQueries({
//         queryKey: ["team-members", teamId],
//       });

//       await queryClient.invalidateQueries({
//         queryKey: ["team", teamId],
//       });

//       setInviteError(null);
//       setIsInviteModalOpen(false);
//     },
//   });

//   const filteredMembers = useMemo(() => {
//     const value = search.trim().toLowerCase();

//     if (!value) {
//       return members;
//     }

//     return members.filter((member) => {
//       const fullName = `${member.firstName} ${member.lastName}`.toLowerCase();
//       const email = member.email.toLowerCase();
//       const role = member.role.toLowerCase();
//       const specialization = member.specialization?.toLowerCase() ?? "";

//       return (
//         fullName.includes(value) ||
//         email.includes(value) ||
//         role.includes(value) ||
//         specialization.includes(value)
//       );
//     });
//   }, [members, search]);

//   const adminsCount = members.filter(
//     (member) => member.role === "Admin",
//   ).length;

//   if (isLoading) {
//     return (
//       <main className={styles.page}>
//         <div className={styles.stateCard}>Loading team members...</div>
//       </main>
//     );
//   }

//   if (isError) {
//     return (
//       <main className={styles.page}>
//         <div className={styles.stateCard}>Failed to load team members.</div>
//       </main>
//     );
//   }

//   return (
//     <main className={styles.page}>
//       <section className={styles.header}>
//         <div>
//           <span className={styles.eyebrow}>Team</span>
//           <h1>Members</h1>
//           <p>Manage people who have access to this workspace.</p>
//         </div>

//         <div className={styles.stats}>
//           <StatCard value={members.length} label="Members" tone="members" />
//           <StatCard value={adminsCount} label="Admins" tone="admins" />
//         </div>
//       </section>

//       <section className={styles.toolbar}>
//         <div className={styles.searchBox}>
//           <input
//             value={search}
//             type="text"
//             placeholder="Search members..."
//             onChange={(event) => setSearch(event.target.value)}
//           />
//         </div>

//         <button
//           type="button"
//           className={styles.inviteButton}
//           onClick={() => {
//             setInviteError(null);
//             setIsInviteModalOpen(true);
//           }}
//         >
//           Invite members
//         </button>
//       </section>

//       <section className={styles.membersCard}>
//         <div className={styles.tableHeader}>
//           <span>Member</span>
//           <span>Role</span>
//           <span>Specialization</span>
//         </div>

//         {filteredMembers.length === 0 ? (
//           <div className={styles.emptyState}>
//             <strong>No members found</strong>
//             <span>Try another name, email, role or specialization.</span>
//           </div>
//         ) : (
//           <div className={styles.membersList}>
//             {filteredMembers.map((member) => (
//               <MemberRow key={member.userId} member={member} />
//             ))}
//           </div>
//         )}
//       </section>
//       {isInviteModalOpen && (
//         <InviteTeamMemberModal
//           currentMembers={members}
//           isSubmitting={addTeamMembersMutation.isPending}
//           submitError={inviteError}
//           onClose={() => {
//             if (addTeamMembersMutation.isPending) {
//               return;
//             }

//             setInviteError(null);
//             setIsInviteModalOpen(false);
//           }}
//           onSubmit={async (selectedMembers) => {
//             try {
//               setInviteError(null);
//               await addTeamMembersMutation.mutateAsync(selectedMembers);
//             } catch (error) {
//               if (axios.isAxiosError(error)) {
//                 const code = error.response?.data?.code;

//                 if (code === "UserNotFound") {
//                   setInviteError("One or more selected users were not found.");
//                   return;
//                 }

//                 if (code === "TeamMemberAlreadyExists") {
//                   setInviteError(
//                     "One or more selected users are already members.",
//                   );
//                   return;
//                 }

//                 if (code === "TeamAccessDenied") {
//                   setInviteError("Only team owner can add members.");
//                   return;
//                 }
//               }

//               setInviteError("Failed to add members. Please try again.");
//             }
//           }}
//         />
//       )}
//     </main>
//   );
// };

// interface StatCardProps {
//   value: number;
//   label: string;
//   tone?: "members" | "admins";
// }

// const StatCard = ({ value, label, tone = "members" }: StatCardProps) => {
//   return (
//     <div className={`${styles.statCard} ${styles[`statCard_${tone}`]}`}>
//       <div className={styles.statIcon}>{tone === "members" ? "👥" : "🛡"}</div>

//       <div>
//         <span>{value}</span>
//         <span>{label}</span>
//       </div>
//     </div>
//   );
// };

// interface MemberRowProps {
//   member: TeamMemberDto;
// }

// const MemberRow = ({ member }: MemberRowProps) => {
//   return (
//     <article className={styles.memberRow}>
//       <div className={styles.memberMain}>
//         <div className={styles.avatar}>
//           {getInitials(member.firstName, member.lastName)}
//         </div>

//         <div className={styles.memberInfo}>
//           <strong>
//             {member.firstName} {member.lastName}
//           </strong>
//           <span>{member.email}</span>
//         </div>
//       </div>

//       <div>
//         <span
//           className={`${styles.roleBadge} ${
//             roleClassNames[member.role] ?? styles.roleMember
//           }`}
//         >
//           {roleLabels[member.role] ?? member.role}
//         </span>
//       </div>

//       <div>
//         {member.specialization ? (
//           <span className={styles.specializationBadge}>
//             {member.specialization}
//           </span>
//         ) : (
//           <span className={styles.mutedText}>Not specified</span>
//         )}
//       </div>
//     </article>
//   );
// };

// const getInitials = (firstName: string, lastName: string) => {
//   return `${firstName[0] ?? ""}${lastName[0] ?? ""}`.toUpperCase();
// };

// export default TeamMembersPage;
// export default TeamMembersPage;
// export default TeamMembersPage;
// export default TeamMembersPage;
