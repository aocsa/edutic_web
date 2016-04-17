mlearningApp.service('lopageService', ['$http', function ($http) {

        var urlBase = '/Page';

        this.createPage = function (page) {
            page.id = 0;
            return $http.post(urlBase+'/Create', page);
        };
        this.createPages = function (pages) {
        	return $http.post(urlBase+'/CreatePages', pages);
        }
        this.updatePage = function (page) {
            return $http.post(urlBase + '/Update', page)
        };
        this.createTag = function (tag) {
            return $http.post(urlBase+'/CreateTag', tag);
        }
    }]);