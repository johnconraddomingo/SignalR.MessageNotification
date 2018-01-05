var app = angular.module("notificationApp",[]);
var notificationController = app.controller("notificationController", function($scope) {

    //
    // Initialise
    //

    // The hubUrl has been hardcoded.
    // You'd probably want to call $http to get the configurable URL value
     
    $.connection.hub.url = "http://localhost:8080/signalr";
    $scope.notifHub = $.connection.notificationHub; 
    $.connection.hub.start(); 

    //
    // Handler Methods
    //

    $scope.notifHub.client.addNotification = function (message) {
        
        var parsedMessage = JSON.parse(message);
        $scope.notifications.push(parsedMessage);
        $scope.$apply();
    };

    $scope.notifHub.client.removeNotification = function (id) {
        // Get the index based on the Id
        var thisId = 0;
        angular.forEach($scope.notifications,
            function (notification) {
                if (thisId === notification.id) {
                    thisId = notification.id;
                }
            });

        $scope.notifications.splice(thisId, 1);  
        $scope.$apply();
    };

    //
    // Send Method
    //

    $scope.readMessage = function (id) {

        // Call the Server to tell it that a message has been read.
        // The Server will then call removeNotification.
        $scope.notifHub.server.removeNotification(id);
    };

    $.connection.hub.disconnected(function () {
        setTimeout(function () { $.connection.hub.start(); }, 5000);
    });

    // Startup.
    // Normally you would want to get all existing messages
    // We won't do that for this demo

    // For now, let's declare $scope.notifications as an empty array
    $scope.notifications = [];
});
