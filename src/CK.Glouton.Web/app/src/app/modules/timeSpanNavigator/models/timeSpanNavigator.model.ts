export enum Scale {
    Year,
    Months,
    Days,
    Hours,
    Minutes,
    Seconds
}

export interface ITimeSpanNavigator {
    from: Date;
    to: Date;
}

export interface ITimeSpanNavigatorSettings {
    from: Date;
    to: Date;
    scale: Scale;
}

export interface IScale {
    name: string;
    min: number;
    max: number;
    // Todo: Add raising || falling edge ?
}

export interface IDefaultScale {
    from: Date;
    to: Date;
}