import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { AdminView } from './components/AdminView';
import { ConfigurationView } from './components/ConfigurationView';
import { LogView } from './components/LogView';
import { RulesetView } from './components/RulesetView';

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/ruleset' component={RulesetView} />
    <Route path='/admin' component={AdminView} />
    <Route path='/logs' component={LogView} />
    <Route path='/configuration' component={ConfigurationView} />
</Layout>;
