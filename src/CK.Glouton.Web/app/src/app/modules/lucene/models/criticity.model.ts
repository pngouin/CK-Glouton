export enum ECriticityLevel {
    trace = 1 << 0,
    info = 1 << 1,
    warn = 1 << 2,
    error = 1 << 3,
    fatal = 1 << 4,
    debug = 1 << 5
}