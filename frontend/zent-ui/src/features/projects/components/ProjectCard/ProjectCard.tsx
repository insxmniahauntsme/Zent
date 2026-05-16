import { Link } from "react-router-dom";
import type { ProjectDto } from "@/features/projects/model/types";

import memberIcon from "@/assets/icons/member-icon.svg";
import projectsIcon from "@/assets/icons/projects-icon.svg";

import styles from "./ProjectCard.module.css";

interface ProjectCardProps {
  teamId: string;
  project: ProjectDto;
}

const ProjectCard = ({ teamId, project }: ProjectCardProps) => {
  return (
    <Link
      to={`/app/${teamId}/projects/${project.projectId}`}
      className={styles.card}
    >
      <div className={styles.iconBox}>
        {project.name.slice(0, 2).toUpperCase()}
      </div>

      <div className={styles.main}>
        <div className={styles.titleRow}>
          <h3 className={styles.name}>{project.name}</h3>

          {project.client && (
            <span className={styles.client}>{project.client}</span>
          )}
        </div>

        <p className={styles.description}>
          {project.description || "No description provided yet."}
        </p>

        <div className={styles.stats}>
          <span>
            <img src={memberIcon} alt="" />
            {project.membersCount} members
          </span>

          <span>
            <img src={projectsIcon} alt="" />
            {project.boardsCount} boards
          </span>
        </div>
      </div>

      <div className={styles.open}>
        <span>Open</span>
        <span className={styles.arrow}>›</span>
      </div>
    </Link>
  );
};

export default ProjectCard;
