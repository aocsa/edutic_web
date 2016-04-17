mlearningApp.service('loService', ['$http', function ($http) {

    var urlBase = '/Publisher';

    this.createLO = function (lo) {
        //return $http.post(urlBase + '/CreateLO', lo);
        ///*
        console.log(lo);
        var fd = new FormData();
		fd.append('id', lo.id);
		fd.append('__description', lo.description);
		//fd.append('tags', angular.toJson(lo.tags));
		fd.append('title', lo.title);
		fd.append('type', lo.type);
		fd.append('fileCover', lo.fileCover);
		fd.append('fileBackground', lo.fileBackground);

		return $http.post(
			urlBase + '/CreateLO',
			//server + service + "/subirImagen",
			//"http://localhost:8080/upload-image-brand",
			fd,
			{
				headers: {'Content-Type': undefined},
				transformRequest: angular.identity
			}
		);
		//*/
    };

    this.updateLO = function (lo) {
        //return $http.post(urlBase + '/UpdateLO', lo);

        var fd = new FormData();
        fd.append('id', lo.id);
        fd.append('__description', lo.description);
        fd.append('title', lo.title);
        fd.append('type', lo.type);
        fd.append('url_cover', lo.url_cover);
		fd.append('url_background', lo.url_background);
        fd.append('fileCover', lo.fileCover);
        fd.append('fileBackground', lo.fileBackground);
        fd.append('created_at', lo.created_at);
       // fd.append('tags', angular.toJson(lo.tags));

        return $http.post(
			urlBase + '/UpdateLO',
			fd,
			{
			    headers: { 'Content-Type': undefined },
			    transformRequest: angular.identity
			});
    };

}]);