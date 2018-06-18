import IdMap from './IdMap';

interface AnimalTypeAttribute {
    minValue: number;
    maxValue: number;
    ratioPerMinute: number;
    initialValue: number;
}

export default interface AnimalType {
    id: string;
    name: string;
    attributes: IdMap<AnimalTypeAttribute>;
}