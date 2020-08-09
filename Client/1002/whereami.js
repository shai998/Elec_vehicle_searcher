//-----------------맵 초기화-------------------------------------
var map;
//마커 배열
var markers = [];
var selecteMarker;
var selecteMarkerImage;

function whereami() {

    var container = document.getElementById('map');
    var options =
    {
        center: new kakao.maps.LatLng(37.50554, 126.96071),
        level: 3
    };
    map = new daum.maps.Map(container, options);

    var markerPosition = new kakao.maps.LatLng(37.50554, 126.96071);
    map.setCenter(markerPosition)

    return map;
}
//---------------------------------------------------------------

function MakeMarker(lat, lng,stat) {

    if (stat == 3) {
        var imageSrc = 'C:\\Users\\kmh\\Desktop\\공모전 및 경진대회\\오픈소스 경진대회 자료\\오픈소스 경진대회\\1002\\bin\\Debug\\Marker_Image_Yellow.png',
            imageSize = new kakao.maps.Size(30, 45), // 마커이미지의 크기입니다
            imageOption = { offset: new kakao.maps.Point(27, 69) };

        // 마커이미지의 옵션입니다. 마커의 좌표와 일치시킬 이미지 안에서의 좌표를 설정합니다.
        var markerImage = new kakao.maps.MarkerImage(imageSrc, imageSize, imageOption);
    }
    else if (stat == 1 || stat == 5) {
        var imageSrc = 'C:\\Users\\kmh\\Desktop\\공모전 및 경진대회\\오픈소스 경진대회 자료\\오픈소스 경진대회\\1002\\bin\\Debug\\Marker_Image_Red.png',
            imageSize = new kakao.maps.Size(30, 45), // 마커이미지의 크기입니다
            imageOption = { offset: new kakao.maps.Point(27, 69) };

        // 마커이미지의 옵션입니다. 마커의 좌표와 일치시킬 이미지 안에서의 좌표를 설정합니다.
        var markerImage = new kakao.maps.MarkerImage(imageSrc, imageSize, imageOption);
    }
    else if (stat == 2) {
        var imageSrc = 'C:\\Users\\kmh\\Desktop\\공모전 및 경진대회\\오픈소스 경진대회 자료\\오픈소스 경진대회\\1002\\bin\\Debug\\Marker_Image_Blue.png',
            imageSize = new kakao.maps.Size(30, 45), // 마커이미지의 크기입니다
            imageOption = { offset: new kakao.maps.Point(27, 69) };

        // 마커이미지의 옵션입니다. 마커의 좌표와 일치시킬 이미지 안에서의 좌표를 설정합니다.
        var markerImage = new kakao.maps.MarkerImage(imageSrc, imageSize, imageOption);
    }
    else
        return;

    var markerPosition = new kakao.maps.LatLng(lat, lng);

    var marker = new kakao.maps.Marker({
        position: markerPosition,
        map: map,
        image: markerImage,
        clickable: true
    });

    markers.push(marker);
    marker.setMap(map);

    kakao.maps.event.addListener(marker, 'click', function () {
        
          var latlng = marker.getPosition();
          var lat = latlng.getLat();
          var lng = latlng.getLng();

          if (selecteMarker == marker)
              return;
        ClickImage(marker)
          
        window.external.MarkerInfo(lat, lng);

    });
}

function ClickImage(marker) {
    if (selecteMarker == null) {
        selecteMarker = marker;
        selecteMarkerImage = marker.getImage();
        var Image = new kakao.maps.MarkerImage(
            'C:\\Users\\kmh\\Desktop\\공모전 및 경진대회\\오픈소스 경진대회 자료\\오픈소스 경진대회\\1002\\bin\\Debug\\Marker_Image_Grean.png',
            new kakao.maps.Size(35, 50), new kakao.maps.Point(27, 70));
        marker.setImage(Image);


            }
          else {
              selecteMarker.setImage(selecteMarkerImage);
              selecteMarker = marker;
              selecteMarkerImage = marker.getImage();
              var Image = new kakao.maps.MarkerImage(
                  'C:\\Users\\kmh\\Desktop\\공모전 및 경진대회\\오픈소스 경진대회 자료\\오픈소스 경진대회\\1002\\bin\\Debug\\Marker_Image_Grean.png',
                  new kakao.maps.Size(35, 50), new kakao.maps.Point(27, 69));
                marker.setImage(Image);
            }
}

function setCenter(lat, lng) {
    var markerPosition = new kakao.maps.LatLng(lat, lng);
    map.setCenter(markerPosition);
    kakao.maps.event.trigger(map, 'dragend', '사용자 이벤트');
}

function EraseMarker() {

    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);
    }
    markers = [];
     positions = [],
        selectedMarker = null;
}

function Serch(addr) {
    var geocoder = new kakao.maps.services.Geocoder();

    // 주소로 좌표를 검색합니다
    geocoder.addressSearch(addr, function (result, status) {

        // 정상적으로 검색이 완료됐으면 
        if (status === kakao.maps.services.Status.OK) {

            //var coords = new kakao.maps.LatLng(result[0].y, result[0].x);

            setCenter(result[0].y, result[0].x)
        }
    });    
}
