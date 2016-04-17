'use strict';

/**
 * @ngdoc function
 * @name mlearningApp.controller:AdminAdministradorCtrl
 * @description
 * # AdminAdministradorCtrl
 * Controller of the mlearningApp
 */
angular.module('mlearningApp')
    .controller('coursesCtrl', function ($scope, $routeParams, $modal, coursesServices, consumerServices) {
        $scope.parametros = {};
        $scope.nuevoModal = function () {
            var date = new Date();
            var modalInstance = $modal.open({
                templateUrl: '/Consumer/SectionDetail',
                controller: 'SectionDetailModalCtrl',
                size: "lg",
                resolve: {
                    items: function () {
                        return $scope.parametros;
                    }
                }
            });
        };
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
        $scope.comment = {};

        $scope.postsByCircle = {};
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

            $scope.$parent.onlineToday = 0;
            for (var i = 0; i < $scope.consumerByCircle.length; i++) {
                if ($scope.consumerByCircle[i].is_online) {
                    $scope.$parent.onlineToday++;
                }
            };

        });
        $scope.showLoading();
        consumerServices.GetPostsByCircle($scope.idCircle)
        .success(function (data) {
            $scope.hideLoading();
        	$scope.postsByCircle = data.postsByCircle;
        });
        $scope.addPost = function () {
            if ($scope.comment.text == undefined || $scope.comment.text == null || $scope.comment.text == "")
                return;
            console.log('enviandi post');
            $scope.showLoading();
            coursesServices.AddPostToCircle($scope.idCircle, $scope.comment.text)
            .success(function (data) {
                if (data.success)
                {
                    $scope.hideLoading();
                    $scope.newPost = {
                        name: $scope.$parent.user_session_.name,
                        text: $scope.comment.text,
                        image_url: $scope.$parent.user_session_.photo,
                        created_at: data.post.created_at,
                    };
                    $scope.postsByCircle.push($scope.newPost);
                    $scope.comment.text = "";
                }
            });
        }
    }).controller('SectionDetailModalCtrl', function($scope, $modalInstance, items) {
        $scope.modal_title = "Modal de prueba";
        $scope.inicio = function () {
            $("#prueba").html("Hola mundo XD");
            $(function () {
                console.log("ejecutando script!!!!");
            });
        };
        $scope.guardar = function () {
            console.log("guardar");
        };
        $scope.cerrar = function () {
            $modalInstance.dismiss('cancel');
        };
    });
