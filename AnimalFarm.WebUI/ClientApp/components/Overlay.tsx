import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

import LogRecord from '../model/LogRecord';
import Urls from '../Urls';

interface OverlayProps {
    caption: string
}

export default class Overlay extends React.Component<OverlayProps> {
    constructor() {
        super();
    }

    public render() {
        return <div className='overlay-container'><div><em>{this.props.caption}</em></div></div>;
    }
}