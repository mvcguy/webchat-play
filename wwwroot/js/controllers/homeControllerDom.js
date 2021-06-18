"use strict";

(function (root, factory) {
    if (typeof exports === 'object' && typeof module === 'object')
        module.exports = factory(root);
    else if (typeof define === 'function' && define.amd)
        define([root], factory);
    else if (typeof exports === 'object')
        exports["homeControllerDom"] = factory(root);
    else
        root["homeControllerDom"] = factory(root);

}(window, function (root) {
    var HomeControllerDom = (function () {
        function HomeControllerDom() {
            this.controls = [];
        }

        //
        // alias for index page constants
        //

        var appConstants = root.appConstants.indexPage;

        //
        // helpers
        //
        HomeControllerDom.prototype.findControl = function (id) {
            var index = this.controls.findIndex(({ key }) => key === id);
            if (index !== -1) {
                return this.controls[index].control;
            } else {
                return undefined;
            }
        }

        HomeControllerDom.prototype.addControl = function (id) {
            this.controls.push({ key: id, control: $('#' + id) });
        }

        HomeControllerDom.prototype.addProp = function (prop) {
            Object.defineProperty(HomeControllerDom.prototype, prop, {

                get: function () {
                    return this.findControl(prop);
                }
            });
        }

        HomeControllerDom.create = function () {
            return new HomeControllerDom();
        };

        HomeControllerDom.prototype.init = function () {
            this.addControl(appConstants.sendButton);
            this.addProp(appConstants.sendButton);

            this.addControl(appConstants.messageInput);
            this.addProp(appConstants.messageInput);

            this.addControl(appConstants.receiverInput);
            this.addProp(appConstants.receiverInput);

            this.addControl(appConstants.senderInput);
            this.addProp(appConstants.senderInput);

            this.addControl(appConstants.recepientList);
            this.addProp(appConstants.recepientList);

            this.addControl(appConstants.messagesList);
            this.addProp(appConstants.messagesList);

            this.addControl(appConstants.userTypingMarkerFormat);
            this.addProp(appConstants.userTypingMarkerFormat);

            this.addControl(appConstants.userTypingMarker);
            this.addProp(appConstants.userTypingMarker);

            this.addControl(appConstants.recepientItemTemplate);
            this.addProp(appConstants.recepientItemTemplate);

            this.addControl(appConstants.chatMessageTemplate);
            this.addProp(appConstants.chatMessageTemplate);

            this.addControl(appConstants.chatMessageDeliveredMarker);
            this.addProp(appConstants.chatMessageDeliveredMarker);

            this.addControl(appConstants.selectedUserDisplay);
            this.addProp(appConstants.selectedUserDisplay);


        }

        HomeControllerDom.prototype.countControls = function () {
            return this.controls.length;
        };

        HomeControllerDom.prototype.cloneRecepientItem = function (model) {
            var clone = this.recepientItemTemplate.clone(true);
            var un = model.userName;
            clone.attr('id', un);
            clone.data('id', un);
            clone.find('#userName_badge').attr('id', un + '_badge');
            clone.find('#userName_status').attr('id', un + '_status');
            clone.find('#fullname')
                .attr('id', un + '_fullName')
                .text(model.fullName);
            clone.show();
            this.recepientList.append(clone);

        };

        HomeControllerDom.prototype.cloneChatMessageTemplate = function (model, currentUser) {
            var clone = this.chatMessageTemplate.clone(true);
            var encodedMsg = model.message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
            //var encodedMsg = model.message;
            var itemClass = 'chat-message chat-message-left';
            if (model.sender === currentUser) {
                itemClass = 'chat-message chat-message-right';
            }
            
            if (model.messageId && model.messageId !== '') {
                clone.attr('id', model.messageId);
                encodedMsg = model.message; // if message ID is resolved, then the message is already encoded by server
            }
            clone.html(encodedMsg).addClass(itemClass);
            clone.show();
            this.messagesList.append(clone);
            return clone;

        };

        HomeControllerDom.prototype.cloneChatMessageDeliveredMarker = function () {
            return this.chatMessageDeliveredMarker.clone(true).show();
        };

        return HomeControllerDom;
    }());
    return HomeControllerDom;

}))

