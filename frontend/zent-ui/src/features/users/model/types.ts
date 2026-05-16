export interface CurrentUserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
}

export interface UserSearchDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
}

export interface SearchUsersResponse {
  users: UserSearchDto[];
}
