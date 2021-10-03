export class TimeOption {
    displayText: string;
    timeInSeconds: number;

    constructor(displayText: string, timeInSeconds: number) {
        this.displayText = displayText;
        this.timeInSeconds = timeInSeconds;
    }
}