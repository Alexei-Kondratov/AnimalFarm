import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

import Ruleset from '../model/Ruleset';

interface RulesetState {
    currentRuleset: Ruleset;
    loading: boolean;
}

export class RulesetView extends React.Component<RouteComponentProps<{}>, RulesetState> {
    constructor() {
        super();
        this.state = { currentRuleset: { id: '', name: '', animalTypes: {}, animalActions: {} }, loading: true };

        fetch('http://localhost:8080/ruleset/')
            .then(response => response.json() as Promise<Ruleset>)
            .then(data => {
                this.setState({ currentRuleset: data, loading: false });
            });
    }

    public render() {
        if (this.state.loading)
            return <p><em>Loading...</em></p>
        else {
            const ruleset = this.state.currentRuleset;
            const animalActionIds = Object.keys(ruleset.animalActions);
            const animalTypeIds = Object.keys(ruleset.animalTypes);

            return (
                <div>
                    <p><b>Ruleset:</b> {ruleset.name} </p>
                    <p><b>Id:</b> {ruleset.id} </p>
                    <p><b>Actions:</b> {animalActionIds.map(id => ruleset.animalActions[id].name).join(', ')} </p>
                    <p><b>Animals:</b> {animalTypeIds.map(id => ruleset.animalTypes[id].name).join(', ')} </p>
                </div>);
        }
    }
}