mlearningApp.service('sectionService', ['$http', function ($http) {
    var urlBase = '/Publisher';

    this.getSectionPages = function (section) {
        return $http.post(urlBase + '/Read_SectionPages/'+section.id);
    };

    this.createSection = function (section) {
        return $http.post(urlBase + '/CreateLOsection',section);
    };

    this.getLOPages = function (lo_id) {
        return $http.post(urlBase + '/Read_LOPages/'+lo_id);
    }
    this.removePage = function (pageId) {
        return $http.post("/Page/DeletePage/" + pageId);
    }
    this.guardarSection = function (section) {
        return $http.post("/Publisher/UpdateSection", section);
    }
    this.removeSection = function (section) {
        return $http.post("/Publisher/DeleteSection", section);
    }
}]);