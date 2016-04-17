mlearningApp.controller('seccionController', function ($scope, $compile, $sce, sectionService) {

    $scope.aviso = 'no se a Añadido Ninguna Seccion';
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
        console.log('unidadActual :::',$scope.unidadActual);
    };

    $scope.trustedHtml = function (plainText) {
        return $sce.trustAsHtml(plainText);
    }
   
    $scope.addSeccion = function (data) {
       $scope.aviso= ''; 
       data.id = null;
       $scope.sections.push(data);
        console.log( $scope.sections);
    };
    
    $scope.ocultarMostar =  function () {
        console.log('ocultar o Mostrar ');
    };
    
     $scope.visualizarSeccion =  function () {
        console.log('visualizar seccion');
    };
    
     $scope.editarSeccion =  function () {
        console.log('editar seccion ');
    };
    
    
     $scope.eliminarSeccion =  function (section, index) {
        if (section.id == null || section.id == undefined || section.id == "") {
            toastr.info("No puedes eliminar una seccion que no ha sido creada!");
            return;
        }
        console.log('eliminar seccion ');
        var r = confirm("Esta seguro de eliminar la sección?");
        if (r == true) {
            loading.show();
            sectionService.removeSection(section)
            .success(function (data) {
                loading.hide();
                if (data.success) {
                    $scope.sections.splice(index, 1);
                    toastr.success("", "Se elimino correctamente");
                } else {
                    toastr.error("", "No se puno eliminar, actualize la pagina e intentelo nuevamente.");
                }
            })
            .error(function (err) {
                console.log(err);
                loading.hide();
                toastr.error("", err);
            });
        }
    };
      
    $scope.nuevaPagina = function (section) {
        if (section.id == null)
        {
            section.LO_id = $scope.unidadActual.id;
            section.id = 0;

            sectionService.createSection(section).
                success(function (data) {
                    if (data.errors.length == 0) {
                        $scope.redireccionar(data.url);
                    }
                });
        }
        else
            $scope.redireccionar('/Page/?sectionId=' + section.id);
    };
    
    $scope.removePage = function (index, section, page) {
        sectionService.removePage(page.id)
        .success(function (data) {
            if (data.success) {
                console.log(index);
                section.pages.splice(index, 1);
            }
        });
    }
    $scope.guardarSeccion = function (index) {
        if ($scope.sections[index].id == null || $scope.sections[index].id == undefined || $scope.sections[index].id == "")
        {
            $scope.sections[index].LO_id = $scope.unidadActual.id;
            $scope.sections[index].id = 0;
            if ($scope.sections[index].name == "" || $scope.sections[index].name == undefined || $scope.sections[index].name == null) {
                toastr.info("", "Escriba el nombre de la sección.");
                return;
            }
            console.log("creando seccion");
            loading.show();
            sectionService.createSection($scope.sections[index]).
                success(function (data) {
                    loading.hide();
                    if (data.errors.length == 0) {
                        $scope.sections[index].id = data.result_id;
                        toastr.success("", "La operación se realizó con éxito");
                    }
                })
                .error(function (err) {
                    loading.hide();
                    toastr.error("", err);
                });
        }
        else {
            console.log("guardando seccion");
            loading.show();
            sectionService.guardarSection($scope.sections[index])
            .success(function (data) {
                loading.hide();
                if (data.success) {
                    toastr.success("", "La operación se realizó con éxito");
                }
            })
            .error(function (err) {
                loading.hide();
                toastr.error("", err);
            });
        }
    }

    $scope.collapseSection = function (section, expanded) {
        if (!expanded || section.id==null) return;
        //console.log(section);
        sectionService.getSectionPages(section)
            .success(function (response) {
                console.log("Response sectionPages => ", response);
                section.pages = response;
                for (var i = 0; i < response.length; i++) {
                    section.pages[i].datos = {};
                    section.pages[i].datos = JSON.parse(response[i].content);
                    console.log(section.pages[i].datos);
                };
            }).error(function (error){
                console.log(error);
            });
    }

    ///funciones q se ejecutan primero 
    
    $scope.getUnidad();
    
    
}).directive('seDirective', function () {
   return {
        templateUrl: '/Scripts/app/directives/seccion-editor.html'        
    };
}).directive('slSeccion', function () {
    return {
        templateUrl: '/Scripts/app/directives/seccionDirective.html'
      
    };
}).directive('pagePreview', function () {
    return {
        scope: true,
        templateUrl: '/Scripts/app/directives/pagePreview.html'
    };
}).directive('dynamicElement', ['$compile', function ($compile) {
      return { 
        restrict: 'E', 
        scope: {
            message: "="
        },
        replace: true,
        link: function(scope, element, attrs) {
            var template = $compile(scope.message)(scope);
            element.replaceWith(template);               
        },
        controller: ['$scope', function($scope) {
           $scope.clickMe = function(){
                alert("hi")
           };
        }]
      }
  }]);
