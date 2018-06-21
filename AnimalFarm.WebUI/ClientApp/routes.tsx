import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { AdminView } from './components/AdminView';
import { AnimalsView } from './components/AnimalsView';
import { ConfigurationView } from './components/ConfigurationView';
import { CreateAnimalView } from './components/CreateAnimalView';
import { LoginView } from './components/LoginView';
import { LogView } from './components/LogView';
import { RulesetView } from './components/RulesetView';

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/admin' component={AdminView} />
    <Route path='/animals' component={AnimalsView} />
    <Route path='/configuration' component={ConfigurationView} />
    <Route path='/createAnimal' component={CreateAnimalView} />
    <Route path='/login' component={LoginView} />
    <Route path='/logs' component={LogView} />
    <Route path='/ruleset' component={RulesetView} />
</Layout>;
