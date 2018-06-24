import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export class Home extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div>
            <h1>Animal Farm web test harness</h1>
            <h4>Things to do</h4>
            <ul>
                <li>Use <b>Administration</b> to clear all data.</li>
                <li>Check <b>Rulesets</b> to see the current ruleset and an upcoming update.</li>
                <li><b>Login</b> as one of the predefined users.</li>
                <li><b>Create</b> a new animal from <b>My Animals</b> page</li>
                <li><b>Pet</b> and <b>Feed</b> your animal.</li>
                <li>Wait until the updates are applied. (There are two of them. First adds the <b>Tiger</b>. Second adds <b>Thirst</b> attribute and <b>Water</b> action.)</li>
                <li>Check that your animals are updated to the latest ruleset when you view them.</li>
                <li>View the <b>Logs</b>.</li>
                <li>Check the current <b>Configuration</b> for data sources and repositories.</li>
            </ul>
        </div>;
    }
}
