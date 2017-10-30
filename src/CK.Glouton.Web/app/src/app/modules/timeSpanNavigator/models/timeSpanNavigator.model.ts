export interface ITimeSpanNavigator {
    from: Date;
    to: Date;
}

export interface ITimeSpanNavigatorSettings {
    scales: IScale[];
}

export interface IScale {
    name: string;
    min: number;
    max: number;
    step: number;
}