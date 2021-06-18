'use strict';
(function (root, factory) {
   
    if (typeof exports === 'object' && typeof module === 'object')
        module.exports = factory();
    else if (typeof define === 'function' && define.amd)
        define([], factory);
    else if (typeof exports === 'object')
        exports["endpoints"] = factory();
    else
        root["endpoints"] = factory();

}(window, function () {
    var _endpoints = (function () {
        class Endpoints {
            constructor(apiRoot, hubRoot) {
                this.apiRoot = apiRoot;
                this.hubRoot = hubRoot;

                console.log('API-ROOT: %s, HUB-ROOT: %s', this.apiRoot, this.hubRoot);
            }
        }
        Object.defineProperty(Endpoints.prototype, "baseUrl", {

            get: function () {
                return this.apiRoot;
            },

            set: function (url) {
                this.apiRoot = url;
            },
            enumerable: true,
            configurable: true
        });

        Object.defineProperty(Endpoints.prototype, "sendMessage", {
            get: function () {
                return this.apiRoot + 'Home/SendMessage';
            }
        });

        Object.defineProperty(Endpoints.prototype, "ackMessage", {
            get: function () {
                return this.apiRoot + 'Home/AckMessage';
            }
        });

        Object.defineProperty(Endpoints.prototype, "getUserMessages", {
            get: function () {
                return this.apiRoot + 'Home/GetAllMessagesByUserName';
            }
        });

        Object.defineProperty(Endpoints.prototype, "getAllMessagesAllUsers", {
            get: function () {
                return this.apiRoot + 'Home/GetAllMessagesAllUsers';
            }
        });

        Object.defineProperty(Endpoints.prototype, "getFriendsList", {
            get: function () {
                return this.apiRoot + 'Home/GetFriendsList';
            }
        });

        Object.defineProperty(Endpoints.prototype, "fakeLogin", {
            get: function () {
                return this.apiRoot + 'Home/FakeLogin';
            }
        });

        Object.defineProperty(Endpoints.prototype, "getFriendsStatus", {
            get: function () {
                return this.apiRoot + 'Home/GetFriendsStatus';
            }
        });

        Object.defineProperty(Endpoints.prototype, "sendMessageHub", {
            get: function () {
                return "SendMessage";
            }
        });

        Object.defineProperty(Endpoints.prototype, "ackMessageHub", {
            get: function () {
                return "AckMessage";
            }
        });

        return Endpoints;
    }());

    return _endpoints;
}))