import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

import Urls from '../Urls';
import Overlay from './Overlay';
import { BrowserRouter } from 'react-router-dom';

interface LoginViewState {
    userToken: string | null;
    processing: boolean;
    login: string;
    password: string;
}

interface LoginResponse {
    id: string,
    token: string
}

export class LoginView extends React.Component<RouteComponentProps<{}>, LoginViewState> {
    constructor() {
        super();
        this.state = {
            userToken: localStorage.getItem('userToken'),
            processing: false,
            login: '',
            password: ''
        };
    }

    setLogin(login: string) {
        this.setState({ login: login });
    }

    setPassword(password: string) {
        this.setState({ password: password });
    }

    logout() {
        localStorage.removeItem('userToken');
        localStorage.removeItem('userId');
        this.setState({ userToken: null });
    }

    async login() {
        this.setState({ processing: true });
        const loginTask: Promise<Response> = fetch(Urls.Server + 'login', {
            method: 'post',
            body: JSON.stringify({
                login: this.state.login,
                password: this.state.password
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const response = await loginTask;
        const data: LoginResponse = await (await loginTask).json() as LoginResponse;
        if (!response.ok || !data) {
            alert('Login failed');
            this.setState({ processing: false });
            return;
        }

        localStorage.setItem('userToken', data.token);
        localStorage.setItem('userId', data.id);
        this.props.history.push('/animals');
    }

    public render() {
        const processingOverlay = this.state.processing ?
            <Overlay caption="Logging in..." />
            : null;

        const loginButtonClasses = this.state.login !== '' && this.state.password !== '' ? '' : 'disabled';
        const logoutButton = this.state.userToken ? <div className='row' style={{ marginTop: '10px' }}>
            <button className='btn btn-primary btn-admin' onClick={() => this.logout()}>Log out</button>
        </div> : null;

        return <div>
            {processingOverlay}
            {logoutButton}
            <div className='well' style={{ margin: '10px', display: 'inline-block' }}>
                <p><b>Login:</b> FirstUser</p>
                <p><b>Password:</b> FirstPassword</p>
            </div>
            <div className='well' style={{ margin: '10px', display: 'inline-block' }}>
                <p><b>Login:</b> SecondUser</p>
                <p><b>Password:</b> SecondPassword</p>
            </div>
            <div className='row' style={{ marginTop: '10px' }}>
                <label>Login <input className='form-control' type='text' value={this.state.login} onChange={(e) => this.setLogin(e.currentTarget.value)} /></label>
            </div>
            <div className='row' style={{ marginTop: '10px' }}>
                <label>Password <input className='form-control' type='password' value={this.state.password} onChange={(e) => this.setPassword(e.currentTarget.value)} /></label>
            </div>
            <div className='row' style={{ marginTop: '10px' }}>
                <button className={'btn btn-primary btn-admin ' + loginButtonClasses} onClick={() => this.login()}>Log in</button>
            </div>
        </div>;
    }
}
