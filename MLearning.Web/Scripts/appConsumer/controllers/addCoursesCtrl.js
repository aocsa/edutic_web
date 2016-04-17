'use strict';

/**
 * @ngdoc function
 * @name mlearningApp.controller:AdminAdministradorCtrl
 * @description
 * # consumerCtrl
 * Controller of the mlearningApp
 */
angular.module('mlearningApp')
    .controller('addCoursesCtrl', function ($scope, coursesServices, consumerServices) {
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

        $scope.circles = [];
        consumerServices.GetCirclesByInstitution().success(function(data){
            $scope.circles = data.circlesByInstitution;
            console.log($scope.circles);
            //var options = {valueNames: [ 'name', 'born' ]};
            //var userList = new List('users', options);
        });
        $scope.addCourse = function (model) {
            loading.showLine();
            coursesServices.AddCourse(model)
            .success(function (data) {
                loading.hideLine();
                console.log(data);
                if (data.success) {
                    $scope.$parent.ciclesByUser.push(model);
                }
            })
            .error(function (err) {
                loading.hideLine();
            });
        };
    });
