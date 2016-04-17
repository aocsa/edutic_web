loading = new function () {
	this.showLine = function () {
		$( "#loading_bar" ).slideDown("fast",function(){
			$("#loading_bar").addClass("loader");
			var content = "<div class='yellow sd0'></div>"
	        	+ "<div class='red sd05'></div>"
	        	+ "<div class='green sd1'></div>"
	        	+ "<div class='blue sd15'></div>";
	        $("#loading_bar").html(content);
	    });
	};
	this.hideLine = function () {
		$( "#loading_bar" ).slideUp("slow",function(){
			$("#loading_bar").removeClass();
			$("#loading_bar").html("");
		});
	};
};
