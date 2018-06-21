import IdMap from './IdMap';

export default interface Animal {
    id: string;
    name: string;
    typeId: string;
    lastCalculated: Date;
    attributes: IdMap<number>;
}