<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WindowsMobileGPSLogger.WebUI._Default" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no">
    <meta charset="UTF-8">
    <title>GPSLogger Positions on Google Maps V3</title>
    <style type="text/css">
        .olLayerGoogleCopyright
        {
            visibility: hidden !important;
        }
    </style>

    <script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.3.min.js" type="text/javascript"></script>

    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?sensor=false"></script>

    <script type="text/javascript">
    var map;
    var markersArray = [];
    
    function deleteOverlays() {
    	 if (markersArray) {
    	 	 for (i in markersArray) {
    	 	 	 markersArray[i].setMap(null);
    	 	 } 
    	 	 markersArray.length = 0;
    	 }
    }
    
    function addMarker(x,y, posTime) { 
    	var marker = new google.maps.Marker({position: new google.maps.LatLng(x, y), 
    			                                map: map, 
    			                                title: posTime });
        markersArray.push(marker); 
    }

    function init() {
    	var myOptions = {
      		zoom: 13,
      		center: new google.maps.LatLng(41.047331, 28.999494),
      		mapTypeId: google.maps.MapTypeId.ROADMAP
      	};
      	
      	map = new google.maps.Map(document.getElementById('map'), myOptions);
    }

    function getMarkers() {
    	
    	$.get(  
            "GetPositions.ashx",  
            {device: "MyDevice"},  
            function(data){

            	try {
            		
            		var obj=data.split('#');

            	    for (var i = 0; i < obj.length; i++) {
            		    var xy = obj[i].split('-');
            	    	addMarker(xy[1],xy[0],xy[2]);
            	    }
            	} catch(e) { } 
            },  
            "html"  
        ); 
    }
    </script>

</head>
<body onload="init()">
    <form id="form1" runat="server">
    <div id="map" style="width: 500px; height: 500px; margin: auto;">
    </div>
    <div style="text-align: center; margin: 5px;">
        <input type="button" onclick="getMarkers()" value="Get Markers" />
    </div>
    </form>
</body>
</html>
