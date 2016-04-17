angular.module('mlearningApp').controller('unidadController', function ($scope, globales,loService) {
    $scope.unidad = 'unidadController';
    $scope.unidades = [];
    $scope.pags = [];
    $scope.statusMsg = "";

    $scope.loading = false;

    if (currentLO != null)
    {
        $scope.unidadActual = currentLO;
        $scope.message = "Editar";
        $scope.unidadActual.tags = LOtagsIds;
        var textarea = $("#text_description");
        var div = $("#" + textarea[0].id).siblings('div.editable');
        //setTimeout(function() {
            console.log(currentLO.description);
            div[0].innerHTML = currentLO.description;
        //}, 500);
    }
    else
    {
        $scope.unidadActual = {};
        $scope.unidadActual.id = null;
        $scope.unidadActual.description = "";
        $scope.message = "Crear una nueva ";
        $scope.unidadActual.tags = [];
    }
    
    

    ///////combobox/////////
    //$scope.etiquetas = [];
  
    ///funciones
    
    $scope.onCoverUploadSuccess = function (e) {
        console.log(e.response);
        $scope.$apply(function () {
            $scope.unidadActual.url_cover = e.response.url;
        });
    }

    $scope.onBackgroundUploadSuccess = function (e) {
        $scope.$apply(function () {
            $scope.unidadActual.url_background = e.response.url;
        });
    }

    $scope.onAdditionalData = function(){
        return {
            text: $("#loTags").val()
        };
    }

    $scope.crearUnidad = function () {
        $scope.loading = true;
        var area = $("textarea.editable")[0];
        var div = $("#" + area.id).siblings('div.editable');
        $scope.unidadActual.description = div[0].innerHTML;

        //$scope.statusMsg = "Enviando...";
        if (!$scope.unitForm.$valid) {
            console.log("Invalid fields in form!");
            return;
        }
        //console.log($scope.unidadActual.tags);
        console.log('rcorp');
        //console.log($scope.unidadActual.description);
        console.log($scope.unidadActual);
        loading.show();
        btnAction.disable("btnCrearUnidad");
        loService.createLO($scope.unidadActual)
        .success(function (data) {
            btnAction.enable("btnCrearUnidad");
            loading.hide();
            $scope.loading = false;

            if (data.errors == null) {
                $scope.redireccionar(data.url);
            } else {
                console.log(data.errors);
            }
        })
        .error(function (data) {
            btnAction.enable("btnCrearUnidad");
            loading.hide();
            $scope.loading = false;
            console.log(data);
        });
       
        $scope.status = "";
        console.log('crear Unidad', $scope.unidadActual);         
    }

    $scope.updateLO = function () {

        var area = $("textarea.editable")[0];
        var div = $("#" + area.id).siblings('div.editable');
        $scope.unidadActual.description = div[0].innerHTML;
        
        if (!$scope.unitForm.$valid) {
            console.log("Invalid fields in form!");
            return;
        }

        $scope.loading = true;

        console.log($scope.unidadActual);
        loading.show();
        btnAction.disable("btnSaveUnidad");
        loService.updateLO($scope.unidadActual)
        .success(function (data) {
            loading.hide();
            btnAction.enable("btnSaveUnidad");
            $scope.loading = false;

            if (data.errors == null) {
                $scope.redireccionar('/Publisher/LearningObjects/'+circleID);
            } else {

                $scope.loading = false;
                console.log(data.errors);
            }
        })
        .error(function (data) {
            loading.hide();
            btnAction.enable("btnSaveUnidad");
            console.log(data);
        });
    }

    $scope.cancelarUnidad = function () {
        //console.log('cancelar Unidad', 'Regresar a crear Unidad');
        window.history.back();
    }
}).directive('fileModel', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var model = $parse(attrs.fileModel);
                var modelSetter = model.assign;
                element.bind('change', function () {
                    scope.$apply(function () {
                        modelSetter(scope, element[0].files[0]);
                    });
                });
            }
        };
    }]);

