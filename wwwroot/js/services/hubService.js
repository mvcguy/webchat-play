"use strict";

(function (root, factory) {
    if (typeof exports === 'object' && typeof module === 'object')
        module.exports = factory();
    else if (typeof define === 'function' && define.amd)
        define([], factory);
    else if (typeof exports === 'object')
        exports["hubService"] = factory();
    else
        root["hubService"] = factory();

}(window, function () {
    var HubService = (function () {
        function HubService(endpoints) {
            this.endpoints = endpoints
            this.hubConnection = new signalR.HubConnectionBuilder()
                .withUrl(endpoints.hubRoot)
                .withAutomaticReconnect()
                .build();
            this.subscriptions = [];

            console.log('hub-srv root: %s', endpoints.hubRoot);

        }

        HubService.create = function (endpoints) {
            return new HubService(endpoints);
        };

        HubService.prototype.subscribeToChatSignals = function (subscriptionId, callback) {
            if (!this.hubConnection) throw "Hub connection is needed";
            var _this = this;
            var signalRHub = _this.hubConnection;
            // TODO: use more secure way!
            //  debugger;
            var index = _this.subscriptions.findIndex(({ key }) => key === subscriptionId);
            if (index !== -1) {
                // delete the existing subscription
                _this.subscriptions.splice(index, 1)[0];
                // console.log('existing subscription removed');
            };

            // TODO: add callback for errors and complete
            _this.subscriptions.push({ key: subscriptionId, subscription: callback });
            // remove the handler if exist from before
            signalRHub.off(subscriptionId);
            signalRHub.on(subscriptionId, function (sender, response) {
                // console.log("SignalR: Signal received from server. subscriptionId: %s, Sender: %s, Data: %o",
                //     subscriptionId, sender, response);
                // console.log('Subscriptions: ', _this.subscriptions);
                var index = _this.subscriptions.findIndex(({ key }) => key === subscriptionId);
                if (index !== -1) {
                    var item = _this.subscriptions[index];
                    item.subscription(sender, response);
                }
            });
        };

        HubService.prototype.start = function (connectCallback) {
            var _this = this;
            _this.hubConnection.start().then(function () {
                connectCallback(_this.hubConnection);
            }).catch(function (err) {
                return console.error(err.toString());
            });
        };


        HubService.prototype.sendMessage = function (methodName, callback) {
            var args = [];
            // skip the first two arguments
            for (var _i = 2; _i < arguments.length; _i++) {
                args[_i - 2] = arguments[_i];
            }

            this.hubConnection.invoke(methodName, ...args).then(callback).catch(function (err) {
                return console.error(err);
            });
        };

        return HubService;
    }());
    return HubService;

}))

