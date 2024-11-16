export interface UsersVM {
    users: UserVM[];
}

export interface UserVM {
    id: string;
    email: string;
}

export interface UsersPresentationDataVM {
    users: UserPresentationDataVM[];
}

export interface UserPresentationDataVM {
    userId: string;
    firstName: string;
    lastName: string;
    avatarColor: string;
}