export enum Scale {
    Year,
    Months,
    Days,
    Hours,
    Minutes,
    Seconds
}

export enum SliderSide {
    Left,
    Right
}

export interface ITimeSpanNavigator {
    from: Date;
    to: Date;
}

export interface IDefaultScale {
    from: Date;
    to: Date;
}