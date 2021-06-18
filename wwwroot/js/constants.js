"use strict";

(function (root, factory) {
    if (typeof exports === 'object' && typeof module === 'object')
        module.exports = factory();
    else if (typeof define === 'function' && define.amd)
        define([], factory);
    else if (typeof exports === 'object')
        exports["appConstants"] = factory();
    else
        root["appConstants"] = factory();

}(window, function () {
    var AppConstants = (function () {
        function AppConstants() {       
        }

        AppConstants.create = function () {
            return new AppConstants();
        };

        AppConstants.indexPage = {
            //
            // note that the variable name and value 
            // must match otherwise, the dom element
            // could not be found due the fact
            // that our dom object declares propery with the 
            // same name
            //
            sendButton: 'sendButton',
            messageInput: 'messageInput',
            receiverInput: 'receiverInput',
            senderInput: 'senderInput',
            recepientList: 'recepientList',
            messagesList: 'messagesList',
            userTypingMarkerFormat: 'userTypingMarkerFormat',
            userTypingMarker: 'userTypingMarker',
            recepientItemTemplate: 'recepientItemTemplate',
            chatMessageTemplate: 'chatMessageTemplate',
            chatMessageDeliveredMarker: 'chatMessageDeliveredMarker',
            selectedUserDisplay: 'selectedUserDisplay'
            
        };

        return AppConstants;
    }());
    return AppConstants;

}))

