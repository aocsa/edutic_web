'use strict';

/**
 * @ngdoc function
 * @name mlearningApp.controller:AdminAdministradorCtrl
 * @description
 * # AdminAdministradorCtrl
 * Controller of the mlearningApp
 */
angular.module('mlearningApp')
    .controller('loCtrl', function ($scope, $routeParams, $modal, coursesServices, consumerServices) {
        $scope.parametros = {};
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

        $scope.learningObjects = {};
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
        consumerServices.GetLOByCircle($scope.idCircle)
        .success(function (data) {
            $scope.hideLoading();
        	$scope.learningObjects = data.learningObjects;
        });
        $scope.loadSections = function (index) {
            $scope.lo = $scope.learningObjects[index];
            //console.log($("#collapse" + $scope.lo.id).is(":visible"));
            if (!$("#collapse" + $scope.lo.id).is(":visible")) {
                $scope.showLoading();
                consumerServices.GetSectionByLO($scope.lo.id)
                .success(function (data) {
                    $scope.hideLoading();
                    //console.log(data.sections);
                    $scope.learningObjects[index].sections = data.sections;
                    //console.log($scope.learningObjects[index].sections);
                });
            }
        };
        $scope.loadPages = function (idSection) {
            $scope.showLoading();
            consumerServices.GetPagesBySection(idSection)
            .success(function (data) {
                $scope.hideLoading();
                //console.log(data);
                $scope.parametros.pages = data.pages;
                if (data.pages.length == 0) {
                    alert("No pages");
                    return;
                }

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
            
            });
        };
    }).controller('SectionDetailModalCtrl', function($scope, $sce, $modalInstance, items) {
        $scope.modal_title = "Modal de prueba";
        //var auxData = [];
        var auxData = new Object();
        auxData.datos = [];
        console.log(items.pages);

        for (var i = 0; i < items.pages.length; i++) {
            auxData.datos[i] = JSON.parse(items.pages[i].content);
            //console.log("Pagina " + parseInt(i + 1));
            //console.log($scope.pages[i].datos);
        };
        $scope.pages = auxData;
        console.log($scope.pages);

        $scope.trustedHtml = function (plainText) {
            console.log(plainText);
            return $sce.trustAsHtml(plainText);
        };

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


