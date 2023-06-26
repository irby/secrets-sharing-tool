import { SecretSubmissionRequest } from "./SecretSubmissionRequest"

describe('SecretSubmissionRequest', () => {
    it('when message is null', () => {
        const request = new SecretSubmissionRequest(null, 60);
        expect(request.message).toBe("");
        expect(request.expireMinutes).toBe(60);
    });

    it('when message is empty', () => {
        const request = new SecretSubmissionRequest("", 30);
        expect(request.message).toBe("");
        expect(request.expireMinutes).toBe(30);
    });
    
    it('when message is not null', () => {
        const request = new SecretSubmissionRequest("Hello", 10);
        expect(request.message).toBe("Hello");
        expect(request.expireMinutes).toBe(10);
    });
})
