import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

import LogRecord from '../model/LogRecord';
import Urls from '../Urls';
import Overlay from './Overlay';

interface LogViewState {
    records: LogRecord[];
    showDetails: boolean;
    loading: boolean;
}

export class LogView extends React.Component<RouteComponentProps<{}>, LogViewState> {
    constructor() {
        super();
        this.state = { records: [], showDetails: false, loading: true };

        fetch('/api/log')
            .then(response => response.json() as Promise<LogRecord[]>)
            .then(data => {
                this.setState({ records: data, loading: false });
            });
    }

    toggleDetails() {
        this.setState({ showDetails: !this.state.showDetails });
    }

    renderLogRecord(record: LogRecord) {
        const timeStamp = new Date(record.timeStamp);
        const timeStampString = timeStamp.toLocaleTimeString(undefined, { hour12: false }) + ':' + timeStamp.getMilliseconds();

        const guidRegex = 'request \'[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}';
        const guidMatches = record.eventMessage.match(guidRegex);
        const style = { color: 'inherit' };

        if (guidMatches != null && (guidMatches as RegExpMatchArray).length > 0) {
            const guid = (guidMatches as RegExpMatchArray)[0];
            const guidHash = guid.split('').reduce(function (a, b) { a = ((a << 5) - a) + b.charCodeAt(0); return a & a }, 0);
            const r = ((guidHash & 0xFF0000) >> 17);
            const g = ((guidHash & 0x00FF00) >> 9);
            const b = (guidHash & 0x0000FF) >> 1;
            style.color = 'rgb(' + r.toString() + ',' + g.toString() + ',' + b.toString() + ')';
        }

        const details = this.state.showDetails ? <div>
            <pre>
                {record.details.replace(new RegExp("\" ", "g"), '\"\n')}
            </pre>
        </div>
            : null;

        return <div className='row' key={record.id}>
            <div className='col-sm-1'> {timeStampString} </div>
            <div className='col-sm-11' style={style}> {record.eventMessage} </div>
            {details}
        </div>;
    }

    public render() {
        if (this.state.loading)
            return <Overlay caption="Loading..." />
        else {
            const logRows = this.state.records.map(l => (this.renderLogRecord(l)));

            return (
                <div>
                    <div style={{ marginTop: '10px' }}>
                        <em>Logs feed has a 1 minute lag.</em>
                    </div>
                    <div>
                        <a style={{ cursor: 'pointer' }} onClick={() => this.toggleDetails()}>{this.state.showDetails ? 'Hide' : 'Show'} details</a>
                    </div>
                    <div className='table-logs' style={{ marginTop: '10px' }}>
                        {logRows}
                    </div>
                </div>);
        }
    }
}