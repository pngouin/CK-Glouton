export interface ITimeSpanNavigator {
    from: Date;
    to: Date;
}

export interface ITimeSpanNavigatorSettings {
    scales: IScale[];
    default: IDefaultScale;
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