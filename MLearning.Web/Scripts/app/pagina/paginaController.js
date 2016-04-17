
mlearningApp.controller('paginaController', function ($scope,$timeout,globales,lopageService) {

    $scope.pagina = 'soy controlador de pagina';
    $scope.unidadActual = null;
    $scope.seccionActual = null;
    $scope.loslide = null;
    $scope.isNew = false;

    // $scope.onDropComplete = function (index, obj, evt) {
    //     var otherObj = $scope.draggableObjects[index];
    //     var otherIndex = $scope.draggableObjects.indexOf(obj);
    //     $scope.draggableObjects[index] = obj;
    //     $scope.draggableObjects[otherIndex] = otherObj;
    // }

    $scope.getUnidadSeccion = function () {
        $scope.unidadActual = currentLO;
        $scope.seccionActual = currentLOSection;
        if(currentPage != null){
            $scope.currentPage = currentPage;
            try {
                $scope.loslide = JSON.parse(currentPage.content).lopage.loslide;
                //currentPageTag.id = currentPageTag.tag_id;
                //$scope.loslide[0].pagetag = currentPageTag;
            }
            catch (err) {
                $scope.loslide = [];
                console.log(err.message);
                alert('JSON invalido');
            }
            
        }
        else {
            $scope.isNew = true;
            $scope.currentPage = {};
            $scope.loslide = [{lotype:0}];
        }
        console.log('actual seccion::', $scope.seccionActual);

    };

    $scope.editText = function(index){
        var a = '#'+index;
        console.log(a);
        $timeout(function(){
            console.log($(a).length);
            if($(a).length){
                var editor = new MediumEditor(a, {
                    buttonLabels: 'fontawesome'
                });  
            }
        }, 0);
    };
   
    ////////////////////funciones////////////////////
    $scope.addPagina = function (pagina) {
        console.log('Tag 2');
        $scope.loslide.push(pagina);
    };

    $scope.onUploadPageCoverSuccess = function (e) {
        $scope.$apply(function () {
            $scope.currentPage.url_img = e.response.url;
        });
    };

    $scope.addLista = function (model) {
        model.push({});
    };

    $scope.addImageItem = function(slide)
    {
        slide.loitemize.loitem.push({});
    };
    /*$scope.onUploadSlideImageSuccess = function (e) {
        $scope.$apply(function () {
            $scope.slide.loimage =  e.response.url;
        });
    }*/
    //botones
    $scope.guardarPagina = function () {
        
        var textareas = $("textarea");
        for (var i = textareas.length - 1; i >= 0; i--) {
            var area = textareas[i];
            console.log(area.id);
            var div = $("#" + area.id).siblings('div.editable');
            console.log(area.id);
            var index = $("#" + area.id).data("index");
            $scope.loslide[index].loparagraph = div[0].innerHTML;
            $("#" + area.id).html(div[0].innerHTML);
        };
        console.log('isNew? =>', $scope.isNew);

        setTimeout(function() {

        /*
        var tag = $("input[name='tag_input'").val();
        if (tag == undefined || tag == "" || tag == null) {
        //if ($scope.loslide[0].pagetag == undefined || $scope.loslide[0].pagetag == "" || $scope.loslide[0].pagetag == null) {
            toastr.info("", "Seleccione un Tag");
            return;
        }
        */
        if (!$scope.pageForm.$valid) {
        //    // Submit as normal
            toastr.info("", "Campos invalidos");
            console.log("Invalid fields in form!");
            return;
        }

        $scope.currentPage.title = $scope.loslide[0].lotitle;
        $scope.currentPage.description = $scope.loslide[0].loparagraph;
        $scope.currentPage.url_img = $scope.loslide[0].loimage;
        //$scope.currentPage.tag = $scope.loslide[0].pagetag;

        //delete $scope.loslide[0].pagetag;

        if ($scope.seccionActual != null)
        {
            $scope.currentPage.lo_id = $scope.seccionActual.LO_id;
            $scope.currentPage.LOsection_id = $scope.seccionActual.id;
        }
        /*else {

        }*/

        var content = {};
        content.lopage = {};
        content.lopage.loslide = $scope.loslide;

        $scope.currentPage.content = JSON.stringify(content);


        if ($scope.isNew)
        {
            $scope.currentPage.id = 0;
            /*
            if ($scope.currentPage.tag == null || $scope.currentPage.tag == undefined) {
                var tag_input = $("input[name=tag_input]").val();
                if (tag_input == null || tag_input == "" || tag_input == undefined) {
                    toastr.warning("", "Seleccione un tag");
                    return;
                }
                var tag = {};
                tag.name = tag_input;
                btnAction.disable("savePage");
                loading.show();
                lopageService.createTag(tag)
                .success(function (data) {

                    tag.id = data.resultId;
                    $scope.currentPage.tag = tag;


                    $scope.loslide[0].pagetag = tag;

                    var content = {};
                    content.lopage = {};
                    content.lopage.loslide = $scope.loslide;

                    $scope.currentPage.content = JSON.stringify(content);

                    lopageService.createPage($scope.currentPage).success(function (data) {
                        console.log(data);
                        if (data.errors == null) {
                            loading.hide();
                            btnAction.enable("savePage");
                            toastr.success("OK.", "La operación se realizó con éxito");
                            $scope.redireccionar('/Publisher/LODetail/'+ $scope.seccionActual.LO_id);
                            //$scope.redireccionar(data.url);
                        }
                    });
                });
            }
            else {
                */
                btnAction.disable("savePage");
                loading.show();
                lopageService.createPage($scope.currentPage).success(function (data) {
                    console.log(data);
                    if (data.errors == null) {
                        loading.hide();
                        btnAction.enable("savePage");
                        toastr.success("OK.", "La operación se realizó con éxito");
                        $scope.redireccionar('/Publisher/LODetail/'+ $scope.seccionActual.LO_id);
                        //$scope.redireccionar(data.url);
                    }
                });
            //}
        }
        else {
            btnAction.disable("savePage");
            loading.show();

            /*
            if ($scope.currentPage.tag == null || $scope.currentPage.tag == undefined) {
                var tag_input = $("input[name=tag_input]").val();
                if (tag_input == null || tag_input == "" || tag_input == undefined) {
                    toastr.warning("", "Seleccione un tag");
                    return;
                }
                var tag = {};
                tag.name = tag_input;
                btnAction.disable("savePage");
                loading.show();
                lopageService.createTag(tag)
                .success(function (data) {

                    tag.id = data.resultId;
                    $scope.currentPage.tag = tag;


                    $scope.loslide[0].pagetag = tag;

                    var content = {};
                    content.lopage = {};
                    content.lopage.loslide = $scope.loslide;

                    $scope.currentPage.content = JSON.stringify(content);

                    lopageService.updatePage($scope.currentPage).success(function (data) {
                        loading.hide();
                        btnAction.enable("savePage");
                        toastr.success("OK.", "La operación se realizó con éxito");
                        console.log(data);

                        var newdataSource = new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: "/Resources/Read_tags",
                                    type:'post', 
                                    datatype: 'json',
                                    data: {
                                        Value: "0"
                                    }
                 
                                }
                            }
                        });
                 
                        var ddl = $('select[name=tag').data("kendoComboBox");
                        ddl.setDataSource(newdataSource);
                        ddl.refresh();
                    });
                });
            }
            else {
                */
                btnAction.disable("savePage");
                loading.show();
                lopageService.updatePage($scope.currentPage).success(function (data) {
                    loading.hide();
                    btnAction.enable("savePage");
                    toastr.success("OK.", "La operación se realizó con éxito");
                    console.log(data);
                    /*if (data.errors != null && isNew) {
                        $scope.redireccionar(data.url);
                    }*/
                });
            //}
        }

        console.log('saving page:::', $scope.currentPage);
        $scope.loslide[0].pagetag = $scope.currentPage.tag;
        }, 100);
    };


    $scope.cancelarPagina= function () {
        $scope.redireccionar('/Publisher/LODetail/'+ $scope.seccionActual.LO_id);
    };




    ////////////funcion q se ejecutan al iniciar /////////

    $scope.getUnidadSeccion();
});

mlearningApp.directive('pgEditor', function () {
    return {
        scope: true,
        templateUrl: '/Scripts/app/directives/pagina-editor.html',
        link: function (scope, element, attrs) {
            //element.text('this is the slidesEditor directive');
        },
        controller: function($scope){
            $scope.saludar = function(){
                console.log("Hola :D");
            }
        }
    };
});
    
mlearningApp.directive('pgSlide', function () {
    return {
        scope: true,
        /*scope: {
            ngModel: '='
        },*/
        templateUrl: function (element, attrs) {
            var tipo = attrs.tipo || 'pagina-slide';
            return '/Scripts/app/directives/secciones/' + tipo + '.html';
        },
        link: function (scope, element, attrs) {
            //console.log('#',attrs);
            scope.hola = function () {
                scope.saludar();
            };
            scope.onUploadSlideImageSuccess = function (e)
            {
                scope.$apply(function () {
                    console.log("XD: " + e.response.url)
                    scope.slide.loimage = e.response.url;
                    scope.slide.width = e.response.errors[0];
                    scope.slide.height = e.response.errors[1];
                });
            };

            scope.finishLoading = function() {
                console.log('oscar');
            };


            scope.onUploadSlideImageSuccess2 = function (e)
            {
                console.log(this.name);
                var index = document.getElementsByName(this.name)[0].dataset.index;
                var indexparent = document.getElementsByName(this.name)[0].dataset.indexparent;
                scope.$apply(function () {
                    //scope.slide.loimage = e.response.url;
                    console.log(index);
                    console.log(indexparent);
                    console.log(scope.$parent.loslide[indexparent]);
                    scope.$parent.loslide[indexparent].loitemize.loitem[index].loimage = e.response.url;
                });
            };
        }
    };
});

mlearningApp.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        //console.log(attrs);
        element.bind("keydown keypress", function (event) {

            if(event.which === 13 && scope.newItem.length>0) {
                scope.$apply(function (){
                    scope.$eval(attrs.ngEnter);
                    scope.newItem='';
                });

                event.preventDefault();
            }
        });
    };
});

// NO SE USA

// ejecuta script finalizado  un ng-repeat
// mlearningApp.directive('ngcDone', function ($timeout) {  
//     return function (scope, element, attrs) {  
//         scope.$watch(attrs.ngcDone, function (callback) {  
  
//             if (scope.$last === undefined) {  
//                 scope.$watch('htmlElement', function () {  
//                     if (scope.htmlElement !== undefined) {  
//                         $timeout(eval(callback), 1);  
//                     }  
//                 });  
//             }  
//             if (scope.$last) {  
//                 eval(callback)();  
//             }  
//         });  
//     }  
// }); 


// mlearningApp.directive('repeatDirective', function() {
//     return function(scope, element, attrs) {
//         if (scope.$last){
//         // iteration is complete, do whatever post-processing
//         // is necessary
//             var editor = new MediumEditor('textarea.editable', {
//                 buttonLabels: 'fontawesome'
//             });
//             console.log('ya cargo');
//             // element.parent().css('border', '1px solid black');
//         }
//     }
// }); 


