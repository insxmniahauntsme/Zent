import styles from "./RegisterForm.module.css";
import rightArrowIcon from "@/assets/icons/right-arrow-icon.svg";
import googleLogo from "@/assets/logos/google-logo.svg";
import appleLogo from "@/assets/logos/apple-logo.svg";
import eyeIcon from "@/assets/icons/eye-icon.svg";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { validateRegister } from "../../model/validation";
import { authApi } from "../../api/authApi";
import { tokenStorage } from "@/shared/lib/tokenStorage";

import type { RegisterRequestDto } from "../../model/types";

const RegisterForm = () => {
  const navigate = useNavigate();

  const [showPassword, setShowPassword] = useState(false);

  const [values, setValues] = useState<RegisterRequestDto>({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
  });

  const [errors, setErrors] = useState<{
    firstName?: string;
    lastName?: string;
    email?: string;
    password?: string;
    form?: string;
  }>({});

  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleChange =
    (field: keyof RegisterRequestDto) =>
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

    const validationErrors = validateRegister(values);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {
      setIsSubmitting(true);
      setErrors({});

      const response = await authApi.register(values);

      tokenStorage.set(response.accessToken);

      navigate("/teams");
    } catch (error) {
      console.error(error);
      setErrors({
        form: "Register failed. This email is already registered.",
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className={styles.formWrapper}>
      <h2 className={styles.title}>Create your account</h2>

      <p className={styles.subtitle}>
        Start your journey into a more focused workspace today.
      </p>

      <form className={styles.form} onSubmit={handleSubmit} noValidate>
        <div className={styles.nameRow}>
          <div className={styles.field}>
            <label className={styles.label} htmlFor="firstName">
              First name
            </label>
            <input
              id="firstName"
              type="text"
              className={`${styles.input} ${errors.firstName ? styles.inputError : ""}`}
              placeholder="Enter your first name"
              onChange={handleChange("firstName")}
            />
            {errors.firstName && (
              <p className={styles.errorText}>{errors.firstName}</p>
            )}
          </div>

          <div className={styles.field}>
            <label className={styles.label} htmlFor="lastName">
              Last name
            </label>
            <input
              id="lastName"
              type="text"
              className={`${styles.input} ${errors.lastName ? styles.inputError : ""}`}
              placeholder="Enter your last name"
              onChange={handleChange("lastName")}
            />
            {errors.lastName && (
              <p className={styles.errorText}>{errors.lastName}</p>
            )}
          </div>
        </div>

        <div className={styles.field}>
          <label className={styles.label} htmlFor="email">
            Email
          </label>
          <input
            id="email"
            type="email"
            className={`${styles.input} ${errors.email ? styles.inputError : ""}`}
            placeholder="name@company.com"
            onChange={handleChange("email")}
          />
          {errors.email && <p className={styles.errorText}>{errors.email}</p>}
        </div>

        <div className={styles.field}>
          <label className={styles.label} htmlFor="password">
            Password
          </label>
          <div className={styles.passwordInputWrapper}>
            <input
              id="password"
              type={showPassword ? "text" : "password"}
              className={`${styles.input} ${errors.email ? styles.inputError : ""}`}
              placeholder="••••••••"
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

          <p className={styles.hint}>
            Minimum 8 characters with at least one symbol.
          </p>
        </div>

        <button
          type="submit"
          className={styles.submitFormButton}
          disabled={isSubmitting}
        >
          {isSubmitting ? "Signing in..." : "Create account"}
          <img src={rightArrowIcon} alt="" className={styles.buttonIcon} />
        </button>
      </form>

      <div className={styles.divider} />

      <div className={styles.accountExistence}>
        <span>Already have an account?</span>
        <a href="/login">Log in</a>
      </div>

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

      <p className={styles.legalText}>
        By creating an account, you agree to Atheneum&apos;s{" "}
        <a href="#">Terms of Service</a> and <a href="#">Privacy Policy</a>.
      </p>
    </div>
  );
};

export default RegisterForm;
