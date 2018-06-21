import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

import Urls from '../Urls';
import Overlay from './Overlay';
import Ruleset from '../model/Ruleset';
import AnimalType from '../model/AnimalType';
import Animal from '../model/Animal';

interface AnimalsViewState {
    userToken: string | null;
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
            loading: true,
            ruleset: { id: '', name: '', animalTypes: {}, animalActions: {} },
            animals: [],
            processing: false
        };
    }

    public async componentDidMount() {
        if (!this.state.userToken)
            return;

        const loadRulesetTask: Promise<Response> = fetch(Urls.Server + 'ruleset/');
        const ruleset: Ruleset = await (await loadRulesetTask).json() as Ruleset;

        this.setState({
            ruleset: ruleset,
            loading: false
        });
    }

    createAnimal() {
        this.props.history.push('/animals');
    }

    renderAnimal(animal: Animal) {
        const attributes = animal.attributes;
        const attributeIds = Object.keys(attributes);
        const attributeLines = attributeIds.map(id => <div className='small' key={id}>{id}: {attributes[id]} </div>);
        return <div className='tile' key={animal.id}>
            <h5 className='header'>{animal.name}</h5>
            <h6>{this.state.ruleset.animalTypes[animal.typeId].name}</h6>
            {attributeLines}
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

        //const animalTypeIds = Object.keys(this.state.ruleset.animalTypes);
        //const animalTypeOptions = animalTypeIds.map(id => this.renderAnimalTypeTile(this.state.ruleset.animalTypes[id]));

        //const createButtonClasses = this.state.name !== '' && this.state.animalTypeId !== '' ? '' : 'disabled';

        //return <div>
        //    <h4 className='header'> Create New Animal</h4>
        //    {processing}
        //    <div className='row' style={{ marginTop: '10px' }}>
        //        {animalTypeOptions}
        //    </div>
        //    <div className='row' style={{ marginTop: '10px' }}>
        //        <label>Name <input className='form-control' type='text' value={this.state.name} onChange={(e) => this.setName(e.currentTarget.value)} /></label>
        //    </div>
        //    <div className='row' style={{ marginTop: '10px' }}>
        //        <button className={'btn btn-primary btn-admin ' + createButtonClasses} onClick={() => this.createAnimal()}>Create</button>
        //    </div>
        //</div>;
    }
}