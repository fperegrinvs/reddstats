'use strict'; // http://stackoverflow.com/questions/1335851/what-does-use-strict-do-in-javascript-and-what-is-the-reasoning-behind-it


angular.module('reddstats.controllers')
.controller('AddressCtl', ['$scope', '$location', '$state', 'BlockChainService', function ($scope, $location, $state, BlockChainService) {
    var addressId = $state.params.addressId;

    $scope.loaded = false;

    BlockChainService.GetAddress(addressId, function (item) {
        if (!item) {
            $location.path('/404');
            return;
        }

        $scope.Address = item;

        $scope.loaded = true;

        $scope.ready();
    });
}]);