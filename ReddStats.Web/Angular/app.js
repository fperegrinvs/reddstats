"use strict"; // http://stackoverflow.com/questions/1335851/what-does-use-strict-do-in-javascript-and-what-is-the-reasoning-behind-it

var app = angular.module('reddstats', [
  'ngRoute',
  'ngTouch',
  'ngAnimate',
  'reddstats.controllers',
  'reddstats.services',
  'ui.router'
])
.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    // sempre começar a aplicação em /home, as rotas aninhadas ficam mais claras assim
    $urlRouterProvider
        .when('', '/home')
        .when('/', '/home');

    var createState = function (name, url, partial, controllerName, view) {
        var views = {};
        views[view + "@"] = { templateUrl: "partials/" + partial + ".html", controller: controllerName + "Ctl" };
        return $stateProvider.state(name, { url: url, views: views });
    };

    var stateTree = [
        'home',
        'address',
        'block',
        'transaction',
        'day',
        'week'
    ];

    var stateConfigs = {
    'home':         { url: '/home',                             partial: 'home',        controller: 'Home',         target: 'miolo' },
    'address':      { url: '/address/{addressId:.*}',           partial: 'address',     controller: 'Address',      target: 'miolo' },
    'block':        { url: '/block//{blockId:.*}',              partial: 'block',       controller: 'Block',        target: 'miolo' },
    'transaction':  { url: '/transaction/{transactionId:.*}',   partial: 'transaction', controller: 'Transaction',  target: 'miolo' },
    'day':          { url: '/day/{dayId:.*}',                   partial: 'day',         controller: 'Day',          target: 'miolo' },
    'week':         { url: '/week/{weekId:.*}',                 partial: 'week',        controller: 'Week',         target: 'modal' },
    };

    var stateoptions = createStateOptions(stateTree, stateConfigs);

    stateoptions.forEach(function (state) { createState(state.name, state.url, state.partial, state.controller, state.target); });
}])
.config(['$locationProvider', function ($location) {
    $location.hashPrefix('!');
}]);;

angular.module("reddstats.controllers", []);
angular.module("reddstats.services", []);

//angular.module("centauro.filters", []);
//angular.module("centauro.services", []);
//angular.module("centauro.directives", []);
//angular.module("centauro.animations", []);

// config pra avisar que a pagina carregou, necessário para o phantomJs saber quando parar de esperar
app.run(['$rootScope', function ($rootScope) {
    var _getTopScope = function () {
        return $rootScope;
        //return angular.element(document).scope();
    };

    $rootScope.ready = function () {
        var $scope = _getTopScope();
        $scope.status = 'ready';
        if (!$scope.$$phase) $scope.$apply();
    };
    $rootScope.loading = function () {
        var $scope = _getTopScope();
        $scope.status = 'loading';
        if (!$scope.$$phase) $scope.$apply();
    };
    $rootScope.$on('$routeChangeStart', function () {
        _getTopScope().loading();
    });
}]);

app.run(['$rootScope', function ($rootScope) {
    $rootScope.showError = function(error) {
        $rootScope.errorMsg = error;
    };
}])
.run(['$rootScope', '$state', '$stateParams', function($rootScope, $state, $stateParams) {
    $rootScope.$on('$stateChangeSuccess', function(event, toState, toParams, fromState, fromParams) {
        if (!!toState.views && !!toState.views['modal@']) {
            toState.data = toState.data || {};
            toState.data.showModal = true;
        }

        $rootScope.showModal = toState.data && !!toState.data.showModal;
    });
}])
.run(['$rootScope', '$state', '$stateParams', function ($rootScope, $state, $stateParams) {
    // It's very handy to add references to $state and $stateParams to the $rootScope
    // so that you can access them from any scope within your applications.For example,
    // <li ui-sref-active="active }"> will set the <li> // to active whenever
    // 'contacts.list' or one of its decendents is active.
    $rootScope.$state = $state;
    $rootScope.$stateParams = $stateParams;

    $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
        console.log('$stateChangeStart to ' + toState.to + '- fired when the transition begins. toState,toParams : \n', toState, toParams);
    });
    $rootScope.$on('$stateChangeError', function (event, toState, toParams, fromState, fromParams) {
        console.log('$stateChangeError - fired when an error occurs during transition.');
        console.log(arguments);
    });
    $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
        console.log('$stateChangeSuccess to ' + toState.name + '- fired once the state transition is complete.');
    });
    // $rootScope.$on('$viewContentLoading',function(event, viewConfig){
    //   // runs on individual scopes, so putting it in "run" doesn't work.
    //   console.log('$viewContentLoading - view begins loading - dom not rendered',viewConfig);
    // });
    $rootScope.$on('$viewContentLoaded', function (event) {
        console.log('$viewContentLoaded - fired after dom rendered', event);
    });
    $rootScope.$on('$stateNotFound', function (event, unfoundState, fromState, fromParams) {
        console.log('$stateNotFound ' + unfoundState.to + '  - fired when a state cannot be found by its name.');
        console.log(unfoundState, fromState, fromParams);
    });
}])

// amarração para baralho exibir automaticamente ao trocar de estado
.run(['$rootScope', '$state', '$stateParams', function ($rootScope, $state, $stateParams) {
    $rootScope.$on('$stateChangeSuccess', function(event, toState, toParams, fromState, fromParams) {
        var getKeys = function(obj) {
            var keys = [];
            for (var key in obj) {
                keys.push(key);
            }
            return keys;
        };

        var views = getKeys(toState.views);
        if (!views || views.length == 0)
            return;

        var layer = views[0].match(/\d+/);
        if (layer && layer.length == 1)
            $rootScope.layer = layer[0] - 0;
    });
}]);

function createStateOptions(stateTree, stateConfigs) {
    return stateTree
    .map(function (name) {
        var hierarchy = name.split('.');

        // caso algum dos subestados nao exista, tiramos esse estado da reta
        var notFound = hierarchy.filter(function (part) { return !stateConfigs[part]; });
        if (notFound.length > 0)
            return null;

        var clone = JSON.parse(JSON.stringify(stateConfigs[hierarchy[hierarchy.length - 1]]));
        clone.name = name;

        // colocamos o layer referente ao nível hierárquico do state, caso o target não seja modal (modal é sempre modal)
        if (clone.target != 'modal')
            clone.target = hierarchy.length < 2 ? clone.target : 'layer' + (hierarchy.length - 1);

        return clone;
    })
    .filter(function (obj) { return !!obj; });
}