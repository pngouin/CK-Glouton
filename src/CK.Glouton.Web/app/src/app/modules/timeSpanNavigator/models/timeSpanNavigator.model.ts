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
    step: number;
}

export interface IDefaultScale {
    scale: string;
    from: number;
    to: number;
}