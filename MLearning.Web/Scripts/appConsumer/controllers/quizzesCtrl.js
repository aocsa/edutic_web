'use strict';

/**
 * @ngdoc function
 * @name mlearningApp.controller:AdminAdministradorCtrl
 * @description
 * # AdminAdministradorCtrl
 * Controller of the mlearningApp
 */
angular.module('mlearningApp')
    .controller('quizzesCtrl', function ($scope, $routeParams, coursesServices, consumerServices) {
        $scope.is_loaded = 0;
        $scope.showLoading = function () {
            $scope.is_loaded++;
            if ($scope.is_loaded > 0 && $scope.$parent.is_loaded == 0)
                loading.showLine();
        }
        $scope.hideLoading = function () {
            $scope.is_loaded--;
            if ($scope.is_loaded == 0 && $scope.$parent.is_loaded == 0)
                loading.hideLine();
        }
        $scope.$parent.img_class_title = "ml-icon-curso";
        $scope.idCircle = $routeParams.idCircle;

        $scope.circle ={};
        $scope.circle = {};
        $scope.learningObjectsByCircle = {};
        $scope.consumerByCircle = {};

        $scope.quizzesByCircle = {};
        $scope.showLoading();
        consumerServices.GetCircleById($scope.idCircle)
        .success(function (data) {
            if (data.noCircle) {
                window.location = "/Consumer";
            }
            $scope.hideLoading();
        	$scope.circle = data.circle;
			$scope.learningObjectsByCircle = data.learningObjectsByCircle;
			$scope.consumerByCircle = data.consumerByCircle;

			$scope.$parent.title = $scope.circle.name;
		    $scope.$parent.sub_title = $scope.learningObjectsByCircle.length + " Unidades";
		    $scope.$parent.sub_title2 = $scope.consumerByCircle.length + " Estudiantes";

        });
        $scope.showLoading();
        consumerServices.GetQuizzesByCircle($scope.idCircle)
        .success(function (data) {
            $scope.hideLoading();
        	$scope.quizzesByCircle = data.quizzesByCircle;
        });

    });
