import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

import Ruleset from '../model/Ruleset';
import IdMap from '../model/IdMap';
import Urls from '../Urls';
import Overlay from './Overlay';

interface ConfigurationRecord {
    id: string
    serializedConfiguration: string
}

interface ConfigurationState {
    configurations: ConfigurationRecord[]
    loading: boolean
}

export class ConfigurationView extends React.Component<RouteComponentProps<{}>, ConfigurationState> {
    constructor() {
        super();
        this.state = {
            configurations: [],
            loading: true
        };
    }

    public async componentDidMount() {
        const configurationResponse: Response = await fetch('/api/configuration');
        const configurations: ConfigurationRecord[] = await configurationResponse.json() as ConfigurationRecord[];

        this.setState({
            configurations: configurations,
            loading: false
        });
    }

    public render() {
        if (this.state.loading)
            return <Overlay caption="Loading..." />
        else {
            const configurationRows = this.state.configurations.map(c => (
                <div key={c.id} className='list-row'>
                    <p className='header'> {c.id} </p>
                    <pre>{JSON.stringify(JSON.parse(c.serializedConfiguration), null, 4)}</pre>
                </div>
            ));

            return (
                <div>
                    {configurationRows}
                </div>);
        }
    }
}