import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

import Ruleset from '../model/Ruleset';
import IdMap from '../model/IdMap';
import Urls from '../Urls';

interface RulesetState {
    currentRuleset: Ruleset;
    nextRulesetId: string;
    nextRulesetName: string;
    nextRulesetDate: Date;
    loading: boolean;
}

export class RulesetView extends React.Component<RouteComponentProps<{}>, RulesetState> {
    constructor() {
        super();
        this.state = {
            currentRuleset: { id: '', name: '', animalTypes: {}, animalActions: {} },
            nextRulesetId: "",
            nextRulesetName: "",
            nextRulesetDate: new Date(),
            loading: true
        };
    }

    public async componentDidMount() {
        const loadRulesetTask: Promise<Response> = fetch(Urls.Server + 'ruleset/');
        const loadUpdatesPlanTask: Promise<Response> = fetch(Urls.Server + 'updatesPlan/');
        await Promise.all([loadRulesetTask, loadUpdatesPlanTask]);

        const currentRuleset: Ruleset = await (await loadRulesetTask).json() as Ruleset;
        const updatesPlan: IdMap<string> = await (await loadUpdatesPlanTask).json() as IdMap<string>;

        this.setState({
            currentRuleset: currentRuleset
        })

        for (let dateStr in updatesPlan) {
            this.setState({
                nextRulesetDate: new Date(dateStr),
                nextRulesetId: updatesPlan[dateStr]
            });
            break;
        }

        this.setState({
            loading: false
        });

        if (this.state.nextRulesetId === "")
            return;

        const nextRulesetResponse: Response = await fetch(Urls.Server + 'ruleset/' + this.state.nextRulesetId);
        const nextRuleset: Ruleset = await nextRulesetResponse.json() as Ruleset;

        this.setState({
            nextRulesetName: nextRuleset.name
        });
    }

    public render() {
        if (this.state.loading)
            return <p className='loading'><em>Loading...</em></p>
        else {
            const ruleset = this.state.currentRuleset;
            const animalActionIds = Object.keys(ruleset.animalActions);
            const animalTypeIds = Object.keys(ruleset.animalTypes);

            const currentRulesetDescription = (
                <div>
                    <h2>Current Ruleset</h2>
                    <p><b>Name:</b> {ruleset.name} </p>
                    <p><b>Id:</b> {ruleset.id} </p>
                    <p><b>Actions:</b> {animalActionIds.map(id => ruleset.animalActions[id].name).join(', ')} </p>
                    <p><b>Animals:</b> {animalTypeIds.map(id => ruleset.animalTypes[id].name).join(', ')} </p>
                </div>)

            const plannedUpdate = this.state.nextRulesetName === "" ?
                null
                : (<div>
                    <h2>Planned Update</h2>
                    <p><b>Time:</b> {this.state.nextRulesetDate.toLocaleString()} </p>
                    <p><b>Name:</b> {this.state.nextRulesetName} </p>
                </div>);

            return <div>
                {currentRulesetDescription}
                {plannedUpdate}
            </div>;
        }
    }
}