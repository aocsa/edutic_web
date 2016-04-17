var appServices = angular.module('mlearningApp');

appServices.factory('consumerServices', function ($http) {
    return {
        GetInstitution: function () {
            return $http({
                url: "/Consumer/GetInstitution",
                method: "POST"
            });
        },
        GetCirclesByUser: function () {

            return $http({
                url: "/Consumer/GetCirclesByUser",
                method: "POST"
            });
        },
        GetCirclesByInstitution: function () {

            return $http({
                url: "/Consumer/GetCirclesByInstitution",
                method: "POST"
            });
        },
        GetCircleById: function (idCircle) {
            return $http({
                url: "/Consumer/GetCircleById",
                method: "GET",
                params: {
                    idCircle: idCircle
                }
            });
        },
        GetPostsByCircle: function (idCircle) {
            return $http({
                url: "/Consumer/GetPostsByCircle",
                method: "GET",
                params: {
                    idCircle: idCircle
                }
            });
        },
        GetCommnetsByLO: function (idLO) {
            return $http({
                url: "/Consumer/GetCommnetsByLO",
                method: "GET",
                params: {
                    idLO: idLO
                }
            });
        },
        GetLOByCircle: function (idCircle) {
            return $http({
                url: "/Consumer/GetLOByCircle",
                method: "GET",
                params: {
                    idCircle: idCircle
                }
            });
        },
        GetQuizzesByCircle: function (idCircle) {
            return $http({
                url: "/Consumer/GetQuizzesByCircle",
                method: "GET",
                params: {
                    idCircle: idCircle
                }
            });
        },
        GetSectionByLO: function (idLO) {
            return $http({
                url: "/Consumer/GetSectionByLO",
                method: "GET",
                params: {
                    idLO: idLO
                }
            });
        },
        GetPagesBySection: function (idSection) {
            return $http({
                url: "/Consumer/GetPagesBySection",
                method: "GET",
                params: {
                    idSection: idSection
                }
            });
        }
    }
});
