'use strict';

(function (root, factory) {

    // create a global instance to add datetime extentions
    var inst = factory();

    if (typeof exports === 'object' && typeof module === 'object')
        module.exports = new inst();
    else if (typeof define === 'function' && define.amd)
        define([], factory);
    else if (typeof exports === 'object')
        exports["DateTimeService"] = new inst();
    else
        root["DateTimeService"] = new inst();


}(window, function () {
    return class DateTimeService {
        constructor() {

            Date.prototype.addMilliSeconds = function (ms) {
                return new Date(Date.now() + ms);
            }

            Date.prototype.addSeconds = function (seconds) {
                // return new Date(Date.now() + seconds * 1000);
                return this.addMilliSeconds(seconds * 1000);

            };

            Date.prototype.addMinutes = function (minutes) {
                // return new Date(Date.now() + minutes * 60 * 1000);
                return this.addSeconds(minutes * 60);
            }

            Date.prototype.now = function () {
                return new Date(Date.now());
            }
        }

        



    };
}))