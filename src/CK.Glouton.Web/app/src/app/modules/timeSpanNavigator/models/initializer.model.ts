export interface IScaleEdge {
    Years :  IEdge;
    Months :  IEdge;
    Days :  IEdge;
    Hours :  IEdge;
    Minutes :  IEdge;
    Seconds :  IEdge;
}

export interface IEdge {
    min : number,
    max : number
}