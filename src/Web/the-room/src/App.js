import './App.css';

import { Route, Switch } from 'react-router-dom'

import Forbidden from './pages/Forbidden'
import React from 'react';
import Services from "./pages/Services";
import UnAuthorized from './pages/UnAuthorized'

function App() {
  return (
    <Switch>
      <Route path="/" component={Services} exact />
      <Route path="/UnAuthorized" component={UnAuthorized} />
      <Route path="/Forbidden" component={Forbidden} />
    </Switch>
  );
}

export default App;
