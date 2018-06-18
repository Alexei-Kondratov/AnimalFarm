
export default interface LogRecord {
    rowKey: string;
    preciseTimeStamp: Date;
    eventMessage: string;
    message: string;
}
