declare global {
    // tslint:disable-next-line:interface-name
    interface Array<T> {
        copy<T>(): this;
    }
}

Array.prototype.copy = function () {
    let i = this.length;
    let array = [];
    while(i--) { array[i] = this[i]; }
    return array;
};

export {};