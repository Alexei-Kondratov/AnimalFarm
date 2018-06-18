import IdMap from './IdMap';

export default interface AnimalAction {
    id: string;
    name: string;
    attributeEffects: IdMap<number>;
}
