import IdMap from './IdMap';
import AnimalAction from './AnimalAction';
import AnimalType from './AnimalType';

export default interface Ruleset {
    id: string;
    name: string;

    animalTypes: IdMap<AnimalType>;
    animalActions: IdMap<AnimalAction>;
}
