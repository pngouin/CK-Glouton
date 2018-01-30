import { ActionReducer } from '@ngrx/store';

import timeSpanNavigatorReducer from './modules/timeSpanNavigator/state/timeSpanNavigator.reducer';
import logsParametersReducer from './modules/logs/state/logsPreference.reducer';

/**
 * The compose function is one of our most handy tools. In basic terms, you give
 * it any number of functions and it returns a function. This new function
 * takes a value and chains it through every composed function, returning
 * the output.
 *
 * More: https://drboolean.gitbooks.io/mostly-adequate-guide/content/ch5.html
 */
import { compose } from '@ngrx/core/compose';

/**
 * storeFreeze prevents state from being mutated. When mutation occurs, an
 * exception will be thrown. This is useful during development mode to
 * ensure that none of the reducers accidentally mutates the state.
 */
import { storeFreeze } from 'ngrx-store-freeze';

/**
 * combineReducers is another useful metareducer that takes a map of reducer
 * functions and creates a new reducer that stores the gathers the values
 * of each reducer and stores them using the reducer's key. Think of it
 * almost like a database, where every reducer is a table in the db.
 *
 * More: https://egghead.io/lessons/javascript-redux-implementing-combinereducers-from-scratch
 */
import { combineReducers } from '@ngrx/store';

/**
 * Because metareducers take a reducer function and return a new reducer,
 * we can use our compose helper to chain them together. Here we are
 * using combineReducers to make our top level reducer, and then
 * wrapping that in storeLogger. Remember that compose applies
 * the result from right to left.
 */
const reducers: any = {
    timeSpanNavigator: timeSpanNavigatorReducer,
    logsParameters: logsParametersReducer
};

const developmentReducer: ActionReducer<any> = compose(storeFreeze, combineReducers)(reducers);
const productionReducer: ActionReducer<any> = combineReducers(reducers);

export function reducer(state: any, action: any): any {
  if (ENV === 'production') {
    return productionReducer(state, action);
  } else {
    return developmentReducer(state, action);
  }
}
