import { Component, Input, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { EffectDispatcher } from '@ck/rx';
import { ITimeSpanNavigatorSettings } from 'app/modules/timeSpanNavigator/models';

export class Initializer {

    /**
     * Returns true if the argument is valid, otherwise false.
     * @param argument The argument passed by the user.
     */
    public static validateArgument(argument: any): argument is ITimeSpanNavigatorSettings {
        if(argument === undefined || argument === null) {return false;}
        return (argument as ITimeSpanNavigatorSettings).from !== undefined
            && (argument as ITimeSpanNavigatorSettings).to !== undefined
            && (argument as ITimeSpanNavigatorSettings).scale !== undefined;
    }

}