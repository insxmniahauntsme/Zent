import type { LoginRequestDto, RegisterRequestDto } from "./types";

export interface LoginErrors {
  email?: string;
  password?: string;
}

export interface RegisterErrors {
  firstName?: string;
  lastName?: string;
  email?: string;
  password?: string;
}

const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const passwordRegex = /^(?=.*[A-Za-z]).{8,}$/;

export const validateLogin = (values: LoginRequestDto): LoginErrors => {
  const errors: LoginErrors = {};

  if (!values.email.trim()) {
    errors.email = "Email is required.";
  } else if (!emailRegex.test(values.email)) {
    errors.email = "Enter a valid email.";
  }

  if (!values.password.trim()) {
    errors.password = "Password is required.";
  } else if (!passwordRegex.test(values.password)) {
    errors.password =
      "Password must be at least 8 characters and contain at least one letter.";
  }

  return errors;
};

export const validateRegister = (
  values: RegisterRequestDto,
): RegisterErrors => {
  const errors: RegisterErrors = {};

  if (!values.firstName.trim()) {
    errors.firstName = "First name is required.";
  }

  if (!values.lastName.trim()) {
    errors.lastName = "Last name is required.";
  }

  if (!values.email.trim()) {
    errors.email = "Email is required.";
  } else if (!emailRegex.test(values.email)) {
    errors.email = "Enter a valid email.";
  }

  if (!values.password.trim()) {
    errors.password = "Password is required.";
  } else if (!passwordRegex.test(values.password)) {
    errors.password =
      "Password must be at least 8 characters and contain at least one letter.";
  }

  return errors;
};
