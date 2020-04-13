export interface ModeratePhoto {
    id: number;
    url: string;
    description: string;
    dateAdded: Date;
    isMain: boolean;
    isApproved: boolean;
    userKnownAs: string;
}
