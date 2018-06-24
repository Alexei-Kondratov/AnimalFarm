import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

import Urls from '../Urls';
import Overlay from './Overlay';
import Ruleset from '../model/Ruleset';
import AnimalType from '../model/AnimalType';
import Animal from '../model/Animal';
import GuidGenerator from '../GuidGenerator';

interface AnimalsViewState {
    userToken: string | null;
    userId: string | null;
    loading: boolean;
    ruleset: Ruleset;
    animals: Animal[];
    processing: boolean; 
}

export class AnimalsView extends React.Component<RouteComponentProps<{}>, AnimalsViewState> {
    constructor() {
        super();
        this.state = {
            userToken: localStorage.getItem('userToken'),
            userId: localStorage.getItem('userId'),
            loading: true,
            ruleset: { id: '', name: '', animalTypes: {}, animalActions: {} },
            animals: [],
            processing: false
        };
    }

    buildLoadAnimalTask(animalId: string):Promise<Animal> {
        return fetch(Urls.Server + 'animal/' + animalId, {
            headers: {
                'User-Token': this.state.userToken as string
            }
        })
        .then(response => response.json() as Promise<Animal>);
    }

    public async componentDidMount() {
        if (!this.state.userToken)
            return;

        const loadRulesetTask: Promise<Response> = fetch(Urls.Server + 'ruleset/');
        const loadAnimalIdsTask: Promise<Response> = fetch('/api/animal/' + this.state.userId as string);

        const animals: Animal[] = [];

        const animalIds: string[] = await (await loadAnimalIdsTask).json() as string[];
        const loadAnimalTasks: Promise<void>[] =
            animalIds.map(id => this.buildLoadAnimalTask(id).then(a => { animals.push(a); }));

        await Promise.all(loadAnimalTasks);
        const ruleset: Ruleset = await (await loadRulesetTask).json() as Ruleset;

        this.setState({
            ruleset: ruleset,
            animals: animals,
            loading: false
        });
    }

    createAnimal() {
        this.props.history.push('/createAnimal');
    }

    async doAction(animalId: string, actionId: string) {
        this.setState({
            processing: true
        });

        const actionTask: Promise<Response> = fetch(Urls.Server + 'animal/event', {
            method: 'put',
            body: JSON.stringify({
                eventType: 'interact',
                eventId: GuidGenerator.newGuid(),
                animalId: animalId,
                ownerUserId: this.state.userId,
                actingUserId: this.state.userId,
                animalActionId: actionId,
                time: new Date(new Date().getTime()).toISOString(),
            }),
            headers: {
                'Content-Type': 'application/json',
                'User-Token': this.state.userToken as string
            }
        });

        const response = await actionTask;
        if (!response.ok) {
            alert('Operation failed');
            this.setState({ processing: false });
            return;
        }

        const animal = await this.buildLoadAnimalTask(animalId);
        const animals = [...this.state.animals];
        animals[animals.findIndex(a => a.id == animalId)] = animal;
        this.setState({
            animals: animals,
            processing: false
        });
    } 

    renderAnimal(animal: Animal) {
        const attributes = animal.attributes;
        const attributeIds = Object.keys(attributes);
        const attributeLines = attributeIds.map(id =>
            <div className='small' key={id}>   
                <div>
                    {id} ({Math.floor(attributes[id])})
                </div>
                <div style={{ background: 'gray', width: '100%' }}>
                    <div style={{ width:  attributes[id] + '%', background: 'teal', height: '10px' }}></div>
                </div>
            </div>);

        const actions = this.state.ruleset.animalActions;
        const actionIds = Object.keys(actions);
        const actionButtons = actionIds.map(id =>
            <div key={id} style={{ marginTop: '5px' }}>
                <button onClick={() => this.doAction(animal.id, id)} style={{width: '100%'}} className='btn btn-admin'>{actions[id].name}</button>
            </div>);

        return <div className='tile' style={{ width: '250px'}} key={animal.id}>
            <h5 className='header'>{animal.name}</h5>
            <h6>{this.state.ruleset.animalTypes[animal.typeId].name}</h6>
            <h6>Data as of {new Date(animal.lastCalculated).toLocaleString()}</h6>
            {attributeLines}
            {actionButtons}
        </div>;
    }

    public render() {
        if (!this.state.userToken)
            return <Overlay caption="Please log in." />;

        if (this.state.loading)
            return <Overlay caption="Loading..." />;

        const processing = this.state.processing ?
            <Overlay caption="Processing..." />
            : null;

        const animals = this.state.animals.map(a => this.renderAnimal(a));

        return <div>
            {processing}
            <div className='row' style={{ marginTop: '10px' }}>
                <button className='btn btn-primary btn-admin ' onClick={() => this.createAnimal()}>Create New</button>
            </div>
            <div className='row' style={{ marginTop: '10px' }}>
                {animals}
            </div>
        </div>;
    }
}