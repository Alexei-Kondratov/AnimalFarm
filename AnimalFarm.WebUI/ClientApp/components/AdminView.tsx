import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

import LogRecord from '../model/LogRecord';
import Urls from '../Urls';
import Overlay from './Overlay';

interface AdminViewState {
    processing: boolean;
}

export class AdminView extends React.Component<RouteComponentProps<{}>, AdminViewState> {
    constructor() {
        super();
        this.state = { processing: false };
    }

    async sendClearCache() {
        this.setState({processing: true });
        await fetch(Urls.Server + 'admin/ClearCache', {
            method: 'POST'
        });
        this.setState({ processing: false });
    } 

    async sendResetData() {
        this.setState({ processing: true });
        await fetch(Urls.Server + 'admin/ResetData', {
            method: 'POST'
        });
        this.setState({ processing: false });
    } 

    public render() {
        var processing = this.state.processing ?
            <Overlay caption="Processing..." />
            : null;

        return <div>
            {processing}
            <div style={{ marginTop: '10px' }}>
                <button className='btn btn-admin btn-primary' onClick={() => this.sendClearCache()}>Clear Cache</button> (Clear caches for all service nodes.)
            </div>
            <div style={{ marginTop: '10px' }}>
                <button className='btn btn-admin btn-primary' onClick={() => this.sendResetData()}>Reset Data</button> (Reset database data to default.)
            </div>
        </div>;
    }
}