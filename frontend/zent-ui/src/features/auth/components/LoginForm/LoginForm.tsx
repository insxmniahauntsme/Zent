import styles from "./LoginForm.module.css";
import googleLogo from "@/assets/logos/google-logo.svg";
import appleLogo from "@/assets/logos/apple-logo.svg";
import eyeIcon from "@/assets/icons/eye-icon.svg";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { validateLogin } from "../../model/validation";
import { authApi } from "../../api/authApi";
import { tokenStorage } from "@/shared/lib/tokenStorage";

import type { LoginRequestDto } from "../../model/types";

const LoginForm = () => {
  const navigate = useNavigate();

  const [showPassword, setShowPassword] = useState(false);

  const [values, setValues] = useState<LoginRequestDto>({
    email: "",
    password: "",
  });

  const [errors, setErrors] = useState<{
    email?: string;
    password?: string;
    form?: string;
  }>({});

  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleChange =
    (field: keyof LoginRequestDto) =>
    (event: React.ChangeEvent<HTMLInputElement>) => {
      const newValue = event.target.value;

      setValues((prev) => ({
        ...prev,
        [field]: newValue,
      }));

      setErrors((prev) => ({
        ...prev,
        [field]: undefined,
        form: undefined,
      }));
    };

  const handleSubmit = async (event: React.SubmitEvent<HTMLFormElement>) => {
    event.preventDefault();

    const validationErrors = validateLogin(values);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {
      setIsSubmitting(true);
      setErrors({});

      const response = await authApi.login(values);

      tokenStorage.set(response.accessToken);

      navigate("/teams");
    } catch (error) {
      console.error(error);
      setErrors({
        form: "Login failed. Please check your credentials.",
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className={styles.formWrapper}>
      <h2 className={styles.title}>Welcome Back</h2>
      <p className={styles.subtitle}>Please choose an option to sign in.</p>

      <div className={styles.socialsContainer}>
        <button className={styles.socialButton} type="button">
          <img src={googleLogo} alt="" className={styles.socialIcon} />
          <span>Google</span>
        </button>
        <button className={styles.socialButton} type="button">
          <img src={appleLogo} alt="" className={styles.socialIcon} />
          <span>Apple</span>
        </button>
      </div>

      <div className={styles.dividerRow}>
        <div className={styles.divider} />
        <span className={styles.dividerText}>OR CONTINUE WITH EMAIL</span>
        <div className={styles.divider} />
      </div>

      <form className={styles.form} onSubmit={handleSubmit} noValidate>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="email">
            Email address
          </label>
          <input
            id="email"
            type="email"
            placeholder="name@company.com"
            className={`${styles.input} ${errors.email ? styles.inputError : ""}`}
            value={values.email}
            onChange={handleChange("email")}
          />
          {errors.email && <p className={styles.errorText}>{errors.email}</p>}
        </div>
        <div className={styles.field}>
          <div className={styles.passwordLabelRow}>
            <label className={styles.label} htmlFor="password">
              Password
            </label>
            <a className={styles.forgotLink} href="#">
              Forgot?
            </a>
          </div>

          <div className={styles.passwordInputWrapper}>
            <input
              id="password"
              type={showPassword ? "text" : "password"}
              placeholder="••••••••"
              className={`${styles.input} ${errors.password ? styles.inputError : ""}`}
              value={values.password}
              onChange={handleChange("password")}
            />
            <button
              type="button"
              className={styles.eyeButton}
              onClick={() => setShowPassword((prev) => !prev)}
              aria-label={showPassword ? "Hide password" : "Show password"}
            >
              <img src={eyeIcon} alt="" className={styles.eyeIcon} />
            </button>
          </div>

          {errors.password && (
            <p className={styles.errorText}>{errors.password}</p>
          )}
        </div>

        <label className={styles.rememberRow}>
          <input type="checkbox" className={styles.checkbox} />
          <span>Remember me for 30 days</span>
        </label>

        {errors.form && <p className={styles.errorText}>{errors.form}</p>}

        <button
          type="submit"
          className={styles.submitFormButton}
          disabled={isSubmitting}
        >
          {isSubmitting ? "Signing In..." : "Sign In"}
        </button>
      </form>

      <div className={styles.registerLink}>
        <span>New to Zent?</span>
        <a href="/register">Create an account</a>
      </div>
    </div>
  );
};

export default LoginForm;
