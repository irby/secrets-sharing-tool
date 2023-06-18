export class SecretSubmissionResponse {
    secretId: string;
    key: string;
    expireDateTime: string;
    expireDateTimeEpoch: number;

    constructor(id: string, key:string, expireDateTime: string, expireDateTimeEpoch: number) {
        this.secretId = id;
        this.key = key;
        this.expireDateTime = expireDateTime;
        this.expireDateTimeEpoch = expireDateTimeEpoch;
    }
}
