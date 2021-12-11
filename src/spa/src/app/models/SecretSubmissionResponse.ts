export class SecretSubmissionResponse {
    id: string;
    key: string;
    expireDateTime: string;
    expireDateTimeUnix: number;

    constructor(id: string, key:string, expireDateTime: string, expireDateTimeUnix: number) {
        this.id = id;
        this.key = key;
        this.expireDateTime = expireDateTime;
        this.expireDateTimeUnix = expireDateTimeUnix;
    }
}