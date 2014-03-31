'use strict'; // http://stackoverflow.com/questions/1335851/what-does-use-strict-do-in-javascript-and-what-is-the-reasoning-behind-it

angular.module('reddstats.controllers')
.controller('homeCtl', ['$scope',function($scope) {
    $scope.ready();
}]);