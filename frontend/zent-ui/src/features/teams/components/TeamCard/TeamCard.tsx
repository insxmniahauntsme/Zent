import { Link } from "react-router-dom";
import styles from "./TeamCard.module.css";
import type { UserTeamDto } from "../../model/types";
import memberIcon from "@/assets/icons/member-icon.svg";
import projectsIcon from "@/assets/icons/projects-icon.svg";

interface Props {
  team: UserTeamDto;
}

const TeamCard = ({ team }: Props) => {
  const initials = team.name
    .split(" ")
    .map((w) => w[0])
    .join("")
    .slice(0, 2)
    .toUpperCase();

  return (
    <Link to={`/app/${team.id}`} className={styles.card}>
      <div className={styles.top}>
        <div className={styles.avatar}>{initials}</div>

        <span className={styles.role}>{team.role}</span>
      </div>

      <h3 className={styles.name}>{team.name}</h3>

      <div className={styles.stats}>
        <span className={styles.statItem}>
          <img src={memberIcon} className={styles.statIcon} alt="" />
          {team.membersCount} members
        </span>

        <span className={styles.statItem}>
          <img src={projectsIcon} className={styles.statIcon} alt="" />
          {team.projectsCount} projects
        </span>
      </div>

      <div className={styles.footer}>
        <span>Open team</span>
        <span className={styles.arrow}>›</span>
      </div>
    </Link>
  );
};

export default TeamCard;
