'use strict';

angular.module('mlearningApp')
    .controller('unidadCtrl', function ($scope, sectionService) {

        $scope.sections = [];
        $scope.unidadActual = null;

        $scope.getUnidad = function () {
            $scope.unidadActual = currentLO;
            $scope.sections = currentLOsections;

            sectionService.getLOPages($scope.unidadActual.id)
                 .success(function (response) {
                     console.log("Response sectionPages => ", response);
                     $scope.unidadActual.pages = response;
                 }).error(function (error) {
                     console.log(error);
                 });

            console.log('unidadActual :::', $scope.unidadActual);
        };


        //Se ejecuta primero
        $scope.getUnidad();


    });
