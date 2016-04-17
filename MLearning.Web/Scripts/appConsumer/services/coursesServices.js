var appServices = angular.module('mlearningApp');

appServices.factory('coursesServices', function ($http) {
    return {
        GetListCourse: function () {
            return $http({
                url: "/Consumer/GetListCourse",
                method: "POST"
            });
        },
        GetListCoursesByUser: function (idUser) {

            return $http({
                url: "/"
            });
        },
        AddPostToCircle: function (circleId, comment) {

            return $http({
                url: "/Consumer/AddPostToCircle",
                method: "GET",
                params: {
                    circleId: circleId,
                    text: comment
                }
            });
        },
        AddCourse: function (model) {
            return $http({
                url: "/Consumer/AddCourse",
                method: "POST",
                data: model
            });
        }
    }
});
