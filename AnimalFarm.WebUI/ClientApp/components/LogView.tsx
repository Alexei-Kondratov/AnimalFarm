import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

import LogRecord from '../model/LogRecord';
import Urls from '../Urls';

interface LogViewState {
    records: LogRecord[];
    loading: boolean;
}

export class LogView extends React.Component<RouteComponentProps<{}>, LogViewState> {
    constructor() {
        super();
        this.state = { records: [], loading: true };

        fetch('/api/log')
            .then(response => response.json() as Promise<LogRecord[]>)
            .then(data => {
                this.setState({ records: data, loading: false });
            });
    }

    public render() {
        if (this.state.loading)
            return <p><em>Loading...</em></p>
        else {
            const logRows = this.state.records.map(l => (
                <tr key={l.rowKey}>
                    <td> {l.eventMessage} </td>
                    <td> {l.preciseTimeStamp.toLocaleString()} </td>
                </tr>
            ));

            return (
                <table>
                    {logRows}
                </table>);
        }
    }
}