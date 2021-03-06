import * as React from 'react';
import { Link, NavLink } from 'react-router-dom';

export class NavMenu extends React.Component<{}, {}> {
    public render() {
        return <div className='main-nav'>
                <div className='navbar navbar-inverse'>
                <div className='navbar-header'>
                    <button type='button' className='navbar-toggle' data-toggle='collapse' data-target='.navbar-collapse'>
                        <span className='sr-only'>Toggle navigation</span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                    </button>
                    <Link className='navbar-brand' to={ '/' }>AnimalFarm.WebUI</Link>
                </div>
                <div className='clearfix'></div>
                <div className='navbar-collapse collapse'>
                    <ul className='nav navbar-nav'>
                        <li>
                            <NavLink to={ '/' } exact activeClassName='active'>
                                <span className='glyphicon glyphicon-home'></span> Home
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={'/login'} activeClassName='active'>
                                <span className='glyphicon glyphicon-user'></span> Log in
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={'/animals'} activeClassName='active'>
                                <span className='glyphicon glyphicon-tree-deciduous'></span> My Animals
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={ '/ruleset' } activeClassName='active'>
                                <span className='glyphicon glyphicon-tasks'></span> Ruleset
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={ '/logs' } activeClassName='active'>
                                <span className='glyphicon glyphicon-wrench'></span> Logs
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={'/admin'} activeClassName='active'>
                                <span className='glyphicon glyphicon-wrench'></span> Administration
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={'/configuration'} activeClassName='active'>
                                <span className='glyphicon glyphicon-tasks'></span> Configuration
                            </NavLink>
                        </li>
                    </ul>
                </div>
            </div>
        </div>;
    }
}
