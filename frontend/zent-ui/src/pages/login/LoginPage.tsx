import styles from "./LoginPage.module.css";
import compassIcon from "@/assets/icons/compass-icon.svg";
import starsIcon from "@/assets/icons/stars-icon.svg";
import LoginForm from "@/features/auth/components/LoginForm/LoginForm";
import bgVideoSrc from "@/assets/videos/loginPageBg.mp4";
import BgVideo from "@/features/auth/components/BgVideo/BgVideo";

const LoginPage = () => {
  return (
    <div className={styles.page}>
      <section className={styles.leftPanel}>
        <BgVideo src={bgVideoSrc} className={styles.videoBg} />

        <div className={styles.leftContent}>
          <div className={styles.brand}>
            <p>Zent</p>
          </div>

          <div className={styles.hero}>
            <h1 className={styles.title}>
              Focus on the flow,
              <br />
              not the friction.
            </h1>

            <p className={styles.description}>
              A workspace designed with architectural precision.
              <br />
              Receding into the background so your work can take center stage.
            </p>
          </div>

          <div className={styles.cardsContainer}>
            <div className={styles.card}>
              <img src={compassIcon} alt="Compass" className={styles.icon} />
              <h3>Precision Design.</h3>
              <p>Built for the craftsperson.</p>
            </div>

            <div className={styles.card}>
              <img src={starsIcon} alt="Stars" className={styles.icon} />
              <h3>Quiet interface</h3>
              <p>Negative space with intent.</p>
            </div>
          </div>
        </div>
      </section>
      <section className={styles.rightPanel}>
        <div className={styles.rightContent}>
          <div className={styles.formArea}>
            <div className={styles.formContainer}>
              <LoginForm />
            </div>
          </div>

          <div className={styles.footerLinks}>
            <a href="#">PRIVACY POLICY</a>
            <a href="#">TERMS OF SERVICE</a>
            <span>© 2026 ZENT</span>
          </div>
        </div>
      </section>
    </div>
  );
};

export default LoginPage;
