import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { RulesetView } from './components/RulesetView';
import { LogView } from './components/LogView';

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/ruleset' component={RulesetView} />
    <Route path='/logs' component={LogView} />
</Layout>;
