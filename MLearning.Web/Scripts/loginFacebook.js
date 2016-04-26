    function callbackFaceStatus(response)
    {
        //console.log("Rcorp");
        //console.log(response);
        if(response.status === 'connected'){
            //$("#login").hide();
            //$("#logout").show();

            testAPI();
        }else if(response.status === 'not_authorized'){
            $("#status").append("<p>No autorizado</p>");
            $("#logout").remove();
        }else{
            $("#status").append("<p>No login Facebook</p>");
        }
    }

    window.fbAsyncInit = function() {
        FB.init({
            appId: '350678345101158',
            cookie     : true,  // enable cookies to allow the server to access 
            xfbml      : true,  // parse social plugins on this page
            version    : 'v2.2' // use version 2.2
        });

        //loginStatus();
    };

    // (function(d, s, id){
    //     var js, fjs = d.getElementsByTagName(s)[0];
    //     if (d.getElementById(id)) {return;}
    //     js = d.createElement(s); 
    //     js.id = id;
    //     js.src = '@Url.Content("~/Scripts/sdk.js")';
    //     fjs.parentNode.insertBefore(js, fjs);
    // }(document, 'script', 'facebook-jssdk'));

    
    function loginStatus(){
        FB.getLoginStatus(function(response){
            //console.log('Rick TOkensssss');
            console.log('rick');
            console.log(response);
            //console.log("Token : "+response.authResponse.accessToken);
            if (response.authResponse) {
                $("#logout").show();
                $("#socialId").val(response.authResponse.userID);
                $("#token").val(response.authResponse.accessToken);
                //$("#idInstitution").val($("#institution").find('option:selected').val());
                $("#idInstitution").val(1);
                $("#formLogin").submit();
            } else {
                $("#login").show();
                // do something...maybe show a login prompt
            }
            //scallbackFaceStatus(response);
        });   
    }
    
    function testAPI(){
        FB.api("/me?fields=first_name, last_name, picture, email", function(response){
            console.log("<p>ola "+response.name+"</p>");
            console.log(response);
            //console.log(JSON.stringify(response));
        });
    }

    // function logOut(){
    //     FB.logout(function(response){
    //         callbackFaceStatus(response);
    //         $("#login").show();
    //         $("#logout").hide();
    //         $('#status').append("<p>Logout</p>");
    //     });
    // }

    function logOutFacebook(){
        FB.getLoginStatus(function(response){
            //console.log('Rick TOkensssss');
            console.log(response);
            //console.log("Token : "+response.authResponse.accessToken);
            if (response.authResponse) {
                FB.logout(function(response){
                    console.log(response);
                    $("#logoutForm").submit();
                    //callbackFaceStatus(response);
                });
            } else {
                console.log("no login");
                $("#logoutForm").submit();
                // do something...maybe show a login prompt
            }
            //scallbackFaceStatus(response);
        });  
    }

    function login() {
        console.log("Ricardo");
        //var idInstitution = $("#institution").find('option:selected').val();
        var idInstitution = 1;
        if (idInstitution == null || idInstitution == undefined || idInstitution == "") {
            alert("Seleccione una institucion");
            return false;
        }
        console.log();
        FB.login(function(response){
            console.log(response);
            callbackFaceStatus(response);
            loginStatus();
        },{scope: "public_profile,email"});
    }