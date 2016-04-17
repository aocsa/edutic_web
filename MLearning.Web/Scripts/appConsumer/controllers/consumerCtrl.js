'use strict';

/**
 * @ngdoc function
 * @name mlearningApp.controller:AdminAdministradorCtrl
 * @description
 * # consumerCtrl
 * Controller of the mlearningApp
 */
angular.module('mlearningApp')
    .controller('consumerCtrl', function ($scope) {
        $scope.$parent.title = $scope.$parent.institution.name;
        $scope.$parent.sub_title = $scope.$parent.circlesByInstitution.length + " Cursos";
        $scope.$parent.sub_title2 = $scope.$parent.consumerByInstitution.length + " Estudiantes";
        $scope.$parent.img_class_title = "ml-icon-casa";
        $scope.$parent.onlineToday = 0;
        for (var i = 0; i < $scope.$parent.consumerByInstitution.length; i++) {
            if ($scope.$parent.consumerByInstitution[i].is_online) {
                $scope.$parent.onlineToday++;
            }
        };
    });
