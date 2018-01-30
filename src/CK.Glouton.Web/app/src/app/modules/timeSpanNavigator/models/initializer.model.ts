import { Scale } from './';

export interface ITimeSpanNavigatorSettings {
    from: Date;
    to: Date;
    initialScale: Scale;
    edges: IScaleEdge;
}

export interface IScaleEdge {
    Years :  IEdge;
    Months :  IEdge;
    Days :  IEdge;
    Hours :  IEdge;
    Minutes :  IEdge;
    Seconds :  IEdge;
}

export interface IEdge {
    min : number;
    max : number;
}

