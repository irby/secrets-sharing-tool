export class SecretSubmissionRequest {
    message: string;
    secondsToLive: number;

    constructor(message: string, secondsToLive: number) {
        this.message = message;
        this.secondsToLive = secondsToLive;
    }
}