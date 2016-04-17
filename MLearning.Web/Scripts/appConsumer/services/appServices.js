var appServices = angular.module('mlearningApp');

appServices.factory('appServices', function ($http) {
    return {
        GetUserLogged: function () {
            return $http({
                url: "/Login/GetUserLogged",
                method: "POST"
            });
        }
    }
});
