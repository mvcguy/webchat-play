'use strict';

(function (root, factory) {

    if (typeof exports === 'object' && typeof module === 'object')
        module.exports = factory();
    else if (typeof define === 'function' && define.amd)
        define([], factory);
    else if (typeof exports === 'object')
        exports["homeController"] = factory();
    else
        root["homeController"] = factory();


}(window, function () {

    var HomeController = (function () {
        class HomeController {
            constructor(hubService, endpoints, dom) {
                this.hubService = hubService;
                this.endpoints = endpoints;
                this.dom = dom;
                this.dom.init();
                this.isUserTypingFlag = false;
                this.cacheService = new browserCacheService();
                this.cacheService.removeAll(HomeController.chatPrefix);
                this.useCache = false;
            }

            static chatPrefix = "chat-";
            static friendsPrefix = "friends-";


            static create = function (hubService, endpoints, dom) {
                return new HomeController(hubService, endpoints, dom);
            }

            invokeWithRetry(attempts, delay, callback, callbackArgs, breakCondition, breakConditionArgs) {
                var i = 1;

                function repeat() {
                    setTimeout(function () {
                        callback(callbackArgs);
                        var brk = breakCondition(breakConditionArgs);
                        if (brk === true) return;

                        i++;
                        if (i < attempts) {
                            repeat();
                        }
                    }, delay)
                }

                repeat();
            }

            updateDeliveryReport(response) {

                var _this = this;

                var breakCondition = function (data) {

                    // check if user is selected
                    var selectedUser = _this.getSelectedUser();
                    if (data.sender && selectedUser !== data.sender) return true;

                    var elem = $('#' + data.messageId);
                    return elem && elem.length > 0;
                };

                var callback = function (data) {

                    // check if user is selected
                    var selectedUser = _this.getSelectedUser();
                    if (data.sender && selectedUser === data.sender) return;

                    var existingElement = $('#' + data.messageId);
                    var child = existingElement.find('.fa-check-circle');
                    var hasChild = child && child.length > 0;
                    if (existingElement && existingElement.length > 0 && !hasChild) {
                        existingElement.append(_this.dom.cloneChatMessageDeliveredMarker());
                    }
                    else {
                        console.error('Element not found. Retrying... ID: ', data.messageId);
                    }
                };

                this.invokeWithRetry(10, 1000, callback, response, breakCondition, response);
            }

            onMessageReceived(sender, messageModel) {

                // console.log('hub event arrived: ', sender, messageModel);
                var isDeliveryReport = sender === 'SystemUser' && messageModel.isDeliveryReport === true;

                if (isDeliveryReport === true) {

                    this.updateDeliveryReport(messageModel);
                    return;
                }


                else if (messageModel.isUserStatusReport === true) {
                    this.updateFriendOnlineStatus(messageModel);
                }
                else if (messageModel.isUserTypingStatusReport === true) {
                    this.updateUserTypingMarker(messageModel);

                }
                else if (this.shouldAppendMessage(messageModel)) {
                    this.appendMessageToList(messageModel);

                    //
                    // if message was already ack'ed then, just update the visual appearance
                    //
                    if (messageModel.delivered === true || messageModel.seen === true) {
                        this.updateDeliveryReport(messageModel);
                        return;
                    }

                    //
                    // Inform the server you have received the message
                    //       
                    this.ackMessage(messageModel);

                }
                else {
                    // debugger;
                    //
                    // show a message counter infront of the username if the message is not read yet
                    //
                    this.updateMessageCounterBadge(messageModel);

                }
            }

            updateUserTypingMarker(messageModel) {
                var currentUser = this.getCurrentUser();
                var selectedUser = this.getSelectedUser();

                if (!currentUser || currentUser === '' || !selectedUser || selectedUser === '') return;
                if (selectedUser !== messageModel.sourceUser) return;

                var textFormat = this.dom.userTypingMarkerFormat.text().trim();
                if (messageModel.isTyping === true) {
                    this.dom.userTypingMarker.text(textFormat).show();
                }
                else {
                    this.dom.userTypingMarker.text('').hide();
                }
            }

            updateMessageCounterBadge(messageModel) {
                var selectedUser = '';
                var currentUser = this.getCurrentUser();

                if (currentUser === messageModel.sender) {
                    return;
                }

                if (currentUser === messageModel.sender) {
                    selectedUser = messageModel.recepient;
                }
                else {
                    selectedUser = messageModel.sender;
                }

                var badge = this.getMessageCounterBadge(selectedUser);

                if (!badge || badge.length <= 0) return;

                var text = badge.text();

                if (messageModel.seen === true && text === '') {
                    return;
                }

                if (text !== '') {
                    var nbr = parseInt(text) + 1;
                    badge.text(nbr);
                }
                else {
                    badge.text(1);
                }
            }

            getMessageCounterBadge(selectedUser) {
                var tag = '#' + $.escapeSelector(selectedUser);
                var target = $(tag);
                var badge = target.find(tag + '_badge');
                return badge;
            }

            clearMessageCounterBadge(selectedUser) {
                if (!selectedUser || selectedUser === '') return;
                var badge = this.getMessageCounterBadge(selectedUser);
                if (badge && badge.length > 0) {
                    badge.text('');
                }

            }

            throwIfEmpty(value, propName) {
                if (!value || value.length === 0 || value.trim().length === 0)
                    throw propName + ' is required !'
            }

            onUserLoggedIn(userName) {

                //
                // load friends list
                //
                var _this = this;
                _this.getFriendsList(userName, function (friends) {
                    _this.createFriendsList(friends);
                    _this.getFriendsOnlineStatuses();
                });

                _this.hubService.start((connection) => {
                    _this.onHubConnected(connection);
                });
            }

            createFriendsList(friendsModel) {
                var _this = this;
                $.each(friendsModel, function (index, friend) {
                    _this.dom.cloneRecepientItem(friend);
                });
            }

            shouldAppendMessage(model) {
                //
                // verify that the current user is matching the sender
                //
                var selectedUser = this.getSelectedUser();
                var currentUser = this.getCurrentUser();

                // console.log(model);
                return (model.sender === selectedUser && model.recepient === currentUser)
                    || (model.sender === currentUser && model.recepient === selectedUser);
            }

            getSelectedUser() {
                var selectedUser = this.dom.messagesList.data('selecteduser');
                return selectedUser;
            }

            getUserFullName(user) {

                var currentUser = this.getCurrentUser();
                if (user === currentUser) return 'You';

                var elem = $('#' + $.escapeSelector(user) + '_fullName');
                return elem.text();
            }

            validateModel(model) {
                this.throwIfEmpty(model.sender, 'Sender name');
                this.throwIfEmpty(model.recepient, 'Receiver name');
                this.throwIfEmpty(model.message, 'Message body');

            }

            appendMessageToList(model) {
                var li = this.dom.cloneChatMessageTemplate(model, this.getCurrentUser());
                li[0].scrollIntoView();
                // console.log(li);
                return li;

            }

            getRequestVerificationToken() {
                return document.getElementsByName("__RequestVerificationToken")[0].value;
            }

            getMessageModel() {
                var senderInput = document.getElementById("senderInput").value;
                var receiverInput = document.getElementById("receiverInput").value;
                var message = document.getElementById("messageInput").value;

                return {
                    recepient: receiverInput,
                    sender: senderInput,
                    message
                };
            }

            sendMessage() {

                try {

                    var model = this.getMessageModel();
                    this.validateModel(model);
                    if (this.shouldAppendMessage(model)) {
                        var element = this.appendMessageToList(model);

                        //this.sendMessageInternal(model, url, element);
                        this.sendMessageInternal2(model, element);
                    }
                }
                catch (error) {
                    console.error(error);
                    alert(error);
                }

            }

            /** obsolete (use: this.sendMessageInternal2)
             * Sends the message to server using http
             * @param {*} data - message payload
             * @param {*} url - server ULR
             * @param {*} element - ui element to attach the message contents to
             */
            sendMessageInternal(data, url, element) {

                var validationToken = this.getRequestVerificationToken();
                var headers = { "__RequestVerificationToken": validationToken };

                this.post(url, (response) => {
                    //console.log(data);
                    element.attr('id', response.messageId);
                }, data, headers);
            }

            /**
             * Sends the message to server using signal-R hub
             * @param {*} data - message payload
             * @param {*} element - ui element to attach the message contents to
             */
            sendMessageInternal2(data, element) {
                this.hubService.sendMessage(this.endpoints.sendMessageHub, (messageId) => {
                    element.attr('id', messageId);
                    // console.log('message delivered, ID: %s', messageId);
                }, data);
            }

            ackMessage(messageModel) {
                if (messageModel.sender === this.getCurrentUser()) {
                    return;
                }
                var ackModel = {
                    messageId: messageModel.messageId,
                    seen: true
                };
                this.hubService.sendMessage(this.endpoints.ackMessageHub, (messageId) => {
                    // console.log('message Ack\'d, ID: %s', messageId);
                }, ackModel);
            }

            getAllMessages(userName, friendsName, callback) {

                //
                // serve messages from local cache first
                //

                if (this.useCache) {
                    var _this = this;
                    var key = _this.getChatCacheKey(userName, friendsName);
                    var messages = _this.cacheService.getItem(key);
                    if (messages) {
                        callback(messages);
                        console.log('messages fetched from local cache');
                        return;
                    }
                }
                
                //
                // load from server and store in cache
                //
                var url = this.endpoints.getUserMessages +
                    "?page=1&userName=" + userName + "&friendsName=" + friendsName;
                this.get(url, function (response) {
                    var currentTime = new Date().now();
                    try {
                        _this.cacheService.addItem(key, response, currentTime.addMinutes(30));
                    } catch (error) {
                        console.log(error);
                    }
                    callback(response);
                });
            }

            getAllMessagesAllUsers(userName, callback) {
                this.get(this.endpoints.getAllMessagesAllUsers + "?page=1&userName=" + userName, callback);
            }

            getFriendsList(userName, callback) {
                console.log('getting friends list');
                this.get(this.endpoints.getFriendsList + "?page=1&userName=" + userName, callback);
            }

            get(url, callback) {
                $.ajax({
                    url: url,
                    method: 'GET'
                }).then(function (response) {
                    callback(response);
                }, function (error) {
                    console.error(error);
                });
            }

            post(url, callback, data, headers) {
                var ajaxOptions = {
                    url: url,
                    method: 'POST',
                    data: data ? JSON.stringify(data) : {},
                    contentType: 'application/json',
                    headers: headers ? headers : {}
                };

                $.ajax(ajaxOptions).then(function done(response) {
                    callback(response);
                }, function error(error) {
                    console.error(error);
                });
            }

            getCurrentUser() {

                var userName = this.dom.senderInput.val();
                if (userName) {
                    userName = userName.trim();
                }

                return userName;
            }

            subscribeChat() {
                var _this = this;

                _this.dom.sendButton.removeAttr('disabled');
                var userName = _this.getCurrentUser();
                _this.throwIfEmpty(userName, 'Sender name');

                _this.hubService.subscribeToChatSignals(userName, function (sender, response) {
                    _this.onMessageReceived(sender, response);

                    //
                    // cache the received message
                    //
                    _this.cacheMessage(response);

                });

                _this.dom.senderInput.attr('readonly', true);

            }

            cacheMessage(response) {
                if (!response.recepient || !response.sender) return;
                var key = this.getChatCacheKey(response.recepient, response.sender);
                this.cacheService.appendItem(key, function (currentValue) {

                    var newValue = [...currentValue];
                    if (currentValue) {
                        newValue.push(response);
                    }
                    return newValue;
                });
            }

            getChatCacheKey(sender, receiver) {
                return HomeController.chatPrefix + sender + '|' + receiver;
            }

            onHubConnected(connection) {
                var userName = this.getCurrentUser();
                console.log('Hub is connected. User: %s, ConnectionID: %s, ConnectionStatus: %s', userName
                    , connection.connectionId, connection.state);
                this.subscribeChat();
            }

            getFriendsOnlineStatuses() {
                //
                // get friends online statuses
                //
                var _this = this;
                var userName = this.getCurrentUser();
                _this.get(_this.endpoints.getFriendsStatus + "?userName=" + userName, function (friendStatuses) {

                    $.each(friendStatuses, function (index, friendStatus) {
                        _this.updateFriendOnlineStatus(friendStatus);

                    });

                });
            }

            updateFriendOnlineStatus(friendStatus) {
                // console.log(friendStatus);
                var ele = this.dom.recepientList.find('#' + $.escapeSelector(friendStatus.userName));
                var onlineStatusMark = ele.find('#' + $.escapeSelector(friendStatus.userName) + "_status");
                //console.log('Friends status %o', onlineStatusMark);

                if (friendStatus.isOnline === true) {
                    onlineStatusMark.removeClass('fa-circle-o').addClass('fa-circle');
                }
                else {
                    onlineStatusMark.removeClass('fa-circle').addClass('fa-circle-o');
                }
            }

            resetUserTypingFlag(delay) {
                var _this = this;
                setTimeout(function () {
                    _this.notifyUserTyping(false);
                }, delay);
            }

            notifyUserTyping(userTyping) {

                if ((this.isUserTypingFlag === true && userTyping === true)
                    || (this.isUserTypingFlag === false && userTyping === false)) return;

                var currentUser = this.getCurrentUser();
                var selectedUser = this.getSelectedUser();

                if (!currentUser || currentUser === '' || !selectedUser || selectedUser === '') return;

                var event = {
                    targetUser: selectedUser,
                    sourceUser: currentUser,
                    isTyping: userTyping,
                };

                this.isUserTypingFlag = userTyping;
                var _this = this;

                this.hubService.sendMessage("OnUserTypingEvent", function (response) {
                    /* not using the response for now */
                    console.log('User is typing event sent...');

                    /* reset the flag after a while */
                    if (userTyping === true)
                        _this.resetUserTypingFlag(3000);

                }, event);

            }

            onFriendSelected(selectedUser) {

                this.dom.receiverInput.val(selectedUser);
                this.dom.messageInput.focus().val('');
                this.dom.messagesList.data('selecteduser', selectedUser);
                //
                // clear the counter badge
                //
                this.clearMessageCounterBadge(selectedUser);

                //
                // update the selected user display
                //
                this.dom.selectedUserDisplay.text(selectedUser).show();

                //
                // load the current conversation between the users
                //
                this.dom.messagesList.empty();
                var model = this.getMessageModel();
                var _this = this;
                this.getAllMessages(model.sender, model.recepient, function (messages) {
                    $.each(messages, function (index, message) {
                        // console.log(message);
                        _this.onMessageReceived('SystemUser', message);
                    });
                });


            }

        }

        return HomeController;
    }());

    return HomeController;

}))