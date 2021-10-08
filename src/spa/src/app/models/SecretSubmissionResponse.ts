export class SecretSubmissionResponse {
    id: string;
    key: string;
    expireDateTime: string;
    displayExpireDateTime: string;

    constructor(id: string, key:string, expireDateTime: string) {
        this.id = id;
        this.key = key;
        this.expireDateTime = expireDateTime;

        const expiry = new Date(expireDateTime);
        this.displayExpireDateTime = `${expiry.getUTCMonth()}/${expiry.getUTCDay()}/${expiry.getUTCFullYear()}`;
    }
}