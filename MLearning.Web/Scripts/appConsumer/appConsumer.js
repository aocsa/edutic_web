'use strict';

/**
 * @ngdoc overview
 * @name mlearningApp
 * @description
 * # mlearningApp
 *
 * Main module of the application.
 */
angular
    .module('mlearningApp', [
    'ngResource',
    'ngRoute',
    'ngAnimate',
    'ngCookies',
    'ui.bootstrap',
    'ngYoutubeEmbed'
])
    .run(function($rootScope, $location, $cookieStore, $http){
        $rootScope.$on('$routeChangeStart', function (event,next,current) {
            if ($cookieStore.get('isAuthenticated') == false || $cookieStore.get('isAuthenticated') == null) {
                //$location.path("/Login");
                window.location = "/Login";
            }
        });
    })
    .config(function ($routeProvider) {
    $routeProvider
        .when('/', {
            controller: 'consumerCtrl',
            templateUrl: '/Consumer/Institution'
        })
        .when('/addCourses', {
            controller: "addCoursesCtrl",
            templateUrl: "/Consumer/AddCourses"
        })
        .when('/circle/:idCircle', {
            controller: "coursesCtrl",
            templateUrl: "/Consumer/Circle"
        })
        .when('/circle/:idCircle/comments', {
            controller: "coursesCtrl",
            templateUrl: "/Consumer/Circle"
        })
        .when('/circle/:idCircle/learningobjects', {
            controller: "loCtrl",
            templateUrl: "/Consumer/LearningObjects"
        })
        .when('/circle/:idCircle/quizzes', {
            controller: "quizzesCtrl",
            templateUrl: "/Consumer/Quizzes"
        })
        .otherwise({
        redirectTo: '/'
    });
}).factory('authHttpResponseInterceptor',['$q','$location',function($q,$location){
    return {
        response: function(response){
            if (response.status === 401) {
                console.log("Response 401");
            }
            return response || $q.when(response);
        },
        responseError: function(rejection) {
            if (rejection.status === 401) {
                console.log("Response Error 401",rejection);
                //$location.path('/Login').search('returnTo', $location.path());
                window.location = "/Login";
            }
            return $q.reject(rejection);
        }
    }
}])
.config(['$httpProvider',function($httpProvider) {
    //Http Intercpetor to check auth failures for xhr requests
    $httpProvider.interceptors.push('authHttpResponseInterceptor');
}]);


var mlearningApp = angular.module('mlearningApp');

angular.module('mlearningApp').factory('flickr', function ($resource) {
    return $resource('http://api.flickr.com/services/feeds/photos_public.gne',
                     { format: 'json',
                      jsoncallback: 'JSON_CALLBACK'
                     }, {
        'load': { 'method': 'JSONP' } });
});



angular.module('mlearningApp')
.controller('indexCtrl', function ($scope, $sce, $location, $cookies, $cookieStore, $window, flickr, appServices, consumerServices) {
    $scope.is_loaded = 0;
    $scope.title = "";
    $scope.sub_title = "";
    $scope.sub_title2 = "";
    $scope.img_class_title = "ml-icon-casa";
    $scope.mensaje = "MLearning";
    $scope.showMenu = true;
    $scope.user_session_ = {
        id: "",
        name: "",
        photo: "",
        role: "",
        isAuthenticated: false
    };
    console.log($window.getCookies("ASP.NET_SessionId"));
    console.log($window.getCookies());
    // BEGIN GLOBAL MODELS
    $scope.institution = {};
    $scope.circlesByInstitution = {};
    $scope.consumerByInstitution = {};
    $scope.ciclesByUser = [];
    $scope.onlineToday = 0;
    // END GLOBAL MODELS
    $scope.is_loaded++;

    
   

    consumerServices.GetInstitution()
    .success(function (data) {
        $scope.is_loaded--;
        $scope.isLoaded();
        console.log(data);
        $scope.institution = data.institution;
        $scope.circlesByInstitution = data.circlesByInstitution;
        $scope.consumerByInstitution = data.consumerByInstitution;
        $scope.title = $scope.institution.name;
        $scope.sub_title = data.circlesByInstitution.length + " Cursos";
        $scope.sub_title2 = data.consumerByInstitution.length + " Estudiantes";
        for (var i = 0; i < $scope.consumerByInstitution.length; i++) {
            if ($scope.consumerByInstitution[i].is_online) {
                $scope.onlineToday++;
            }
        };
    });
    $scope.is_loaded++;
    consumerServices.GetCirclesByUser()
    .success(function (data) {
        $scope.is_loaded--;
        $scope.isLoaded();
        $scope.ciclesByUser = data.ciclesByUser;
    });

    // $scope.is_loaded++;
    // consumerServices.GetCirclesByInstitution()
    // .success(function (data) {
    //     console.log(data);
    //     $scope.is_loaded--;
    //     $scope.isLoaded();
    //     $scope.circlesByInstitution = data.circlesByInstitution;
    // });


    $scope.is_loaded++;
    appServices.GetUserLogged()
        .success(function (data) {
            $scope.is_loaded--;
            $scope.isLoaded();
            if (data.id == 0)
                window.location = "/Login"; 
        	//console.log(data);
            $scope.user_session_ = {
                id: data.id,
                name: data.name,
                photo: data.photo,
                role: data.role,
                isAuthenticated: true
            };            
            $cookieStore.put('isAuthenticated', true);
        });
    $scope.items = [
        /*{codigo: 0},
        {codigo: 1},
        {codigo: 2},
           {codigo: 3},
        {codigo: 4},
        {codigo: 5},
        {codigo: 6},*/
    ];
    $scope.redireccionar = function(path){
        //$location.path(path);
        window.location.href = path;
    };

    $scope.openProfile = function () {
        //console.log("openProfile");
        editProfile();
    }
    $scope.logout = function () {
        $scope.user_session_ = {
            id: "",
            name: "",
            photo: "",
            role: "",
            isAuthenticated: false
        };
        $cookieStore.remove('isAuthenticated');
        //logOutFacebook();
        $("#logoutForm").submit();
    }

    $scope.crearUnidad = function () {
        console.log('crear Unidad 222', $scope.unidadActual);
    }
    $scope.isLoaded = function () {
        if ($scope.is_loaded == 0) {
            // loading principal
            $("#loading").fadeOut(1000,function(){
                $("#loading").remove();  
                $('html').removeClass('ml-overflow');      
            });
        }

    }
    //var favoriteCookie = getCookies();
    var favoriteCookie = $cookieStore.get("ASP.NET_SessionId");
    //console.log(favoriteCookie);
    //console.log(document.cookie);

   // value = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
}).filter('capitalize', function() {
    return function(input, all) {
        return (!!input) ? input.replace(/([^\W_]+[^\s-]*) */g, function(txt)
        {
            return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
        }) : '';
    }
  })
.filter('formatterDate1', function () {
    return function(date, all) {
        //console.log('riakrdocorp');
        //console.log(date.replace("/Date(", "").replace(")/", ""));
        date = new Date(parseInt(date.replace("/Date(", "").replace(")/", "")));
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear() + "  " + strTime;
    }
}).directive('pageContact', function () {
   return {
       templateUrl: '/Scripts/appConsumer/directives/contacts.html'
   };
});

var getCookies = function(){
  var pairs = document.cookie.split(";");
  console.log(pairs);
  var cookies = {};
  for (var i=0; i<pairs.length; i++){
    var pair = pairs[i].split("=");
    cookies[pair[0]] = unescape(pair[1]);
  }
  return cookies;
}