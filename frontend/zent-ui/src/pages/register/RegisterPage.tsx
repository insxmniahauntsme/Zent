import BgVideo from "@/features/auth/components/BgVideo/BgVideo";
import bgVideoSrc from "@/assets/videos/registerPageBg.mp4";
import styles from "./RegisterPage.module.css";
import starsIcon from "@/assets/icons/stars-icon.svg";
import layersIcon from "@/assets/icons/layers-icon.svg";
import RegisterForm from "@/features/auth/components/RegisterForm/RegisterForm";

const RegisterPage = () => {
  return (
    <div className={styles.page}>
      <BgVideo src={bgVideoSrc} className={styles.videoBg} />

      <section className={styles.leftPanel}>
        <div className={styles.leftContent}>
          <div className={styles.brand}>
            <p>Zent</p>
          </div>

          <div className={styles.hero}>
            <h1 className={styles.title}>
              Where deep focus meets
              <br />
              architectural precision.
            </h1>

            <p className={styles.description}>
              Join a workspace designed for the quiet intensity of
              <br />
              high-performance teams. Build faster together.
            </p>
          </div>

          <div className={styles.cardsContainer}>
            <div className={styles.card}>
              <img src={starsIcon} alt="Stars" className={styles.icon} />
              <h3>Editorial UI</h3>
              <p>Focus on content over clutter.</p>
            </div>

            <div className={styles.card}>
              <img src={layersIcon} alt="Layers" className={styles.icon} />
              <h3>Tonal Layering</h3>
              <p>Hierarchy defined by color shifts.</p>
            </div>
          </div>
        </div>
      </section>

      <section className={styles.rightPanel}>
        <div className={styles.rightContent}>
          <div className={styles.formArea}>
            <div className={styles.formCard}>
              <RegisterForm />
            </div>
          </div>
        </div>
      </section>
    </div>
  );
};

export default RegisterPage;
