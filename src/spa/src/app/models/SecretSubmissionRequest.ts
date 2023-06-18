export class SecretSubmissionRequest {
    message: string;
    expireMinutes: number;

    constructor(message: string|null, expireMinutes: number) {
        this.message = message ?? "";
        this.expireMinutes = expireMinutes;
    }
}
