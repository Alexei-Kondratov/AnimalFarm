import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

import Ruleset from '../model/Ruleset';
import Urls from '../Urls';
import GuidGenerator from '../GuidGenerator';
import Overlay from './Overlay';
import AnimalType from 'ClientApp/model/AnimalType';

interface CreateAnimalViewState {
    userToken: string | null;
    userId: string | null;
    loading: boolean;
    ruleset: Ruleset;
    name: string;
    animalTypeId: string;
    processing: boolean;
}

export class CreateAnimalView extends React.Component<RouteComponentProps<{}>, CreateAnimalViewState> {
    constructor() {
        super();
        this.state = {
            userToken: localStorage.getItem('userToken'),
            userId: localStorage.getItem('userId'),
            loading: true,
            ruleset: { id: '', name: '', animalTypes: {}, animalActions: {} },
            name: "",
            animalTypeId: "",
            processing: false
        };
    }

    public async componentDidMount() {
        const loadRulesetTask: Promise<Response> = fetch(Urls.Server + 'ruleset/');
        const ruleset: Ruleset = await (await loadRulesetTask).json() as Ruleset;

        this.setState({
            ruleset: ruleset,
            loading: false
        });
    }

    setName(name: string) {
        this.setState({ name: name });
    }

    setType(typeId: string) {
        this.setState({ animalTypeId: typeId });
    }

    async createAnimal() {
        this.setState({
            processing: true
        });

        const createTask: Promise<Response> = fetch(Urls.Server + 'animal/event', {
            method: 'put',
            body: JSON.stringify({
                eventType: 'create',
                eventId: GuidGenerator.newGuid(),
                animalId: GuidGenerator.newGuid(),
                ownerUserId: this.state.userId,
                actingUserId: this.state.userId,
                name: this.state.name,
                animalTypeId: this.state.animalTypeId,
                time: new Date(new Date().getTime()).toISOString(),
            }),
            headers: {
                'Content-Type': 'application/json',
                'User-Token': this.state.userToken as string
            }
        });

        const response = await createTask;
        if (!response.ok) {
            alert('Operation failed');
            this.setState({ processing: false });
            return;
        }

        this.props.history.push('/animals');
    }

    renderAnimalTypeTile(animalType: AnimalType) {
        const formatRatio = (ratio: number): string => {
            let result = ratio.toString();
            if (result[0] !== '-')
                result = '+' + result;
            return result;
        }

        const isSelected = animalType.id === this.state.animalTypeId;
        const tileClasses = ['tile'];
        if (isSelected) {
            tileClasses.push('selected');
        }

        const attributes = animalType.attributes;
        const attributeIds = Object.keys(attributes);
        const attributeLines = attributeIds.map(id => <div className='small' key={id}>{id}: {attributes[id].initialValue} ({formatRatio(attributes[id].ratioPerMinute)})</div>);
        return <div className={tileClasses.join(' ')} onClick={() => this.setType(animalType.id)} key={animalType.id}><h5 className='header'>{animalType.name}</h5>{attributeLines}</div>
    }

    public render() {
        if (!this.state.userToken)
            return <Overlay caption="Please log in." />;

        if (this.state.loading)
            return <Overlay caption="Loading..." />;

        const processing = this.state.processing ?
            <Overlay caption="Processing..." />
            : null;

        const animalTypeIds = Object.keys(this.state.ruleset.animalTypes);
        const animalTypeOptions = animalTypeIds.map(id => this.renderAnimalTypeTile(this.state.ruleset.animalTypes[id]));

        const createButtonClasses = this.state.name !== '' && this.state.animalTypeId !== '' ? '' : 'disabled';

        return <div>
            <h4 className='header'> Create New Animal</h4>
            {processing}
            <div className='row' style={{ marginTop: '10px' }}>
                {animalTypeOptions}
            </div>
            <div className='row' style={{ marginTop: '10px' }}>
                <label>Name <input className='form-control' type='text' value={this.state.name} onChange={(e) => this.setName(e.currentTarget.value)} /></label>
            </div>
            <div className='row' style={{ marginTop: '10px' }}>
                <button className={'btn btn-primary btn-admin ' + createButtonClasses} onClick={() => this.createAnimal()}>Create</button>
            </div>
        </div>;
    }
}