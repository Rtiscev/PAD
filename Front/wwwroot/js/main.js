let ytLink
let fileName;
function GetInformation() {
    ytLink = $('#youtubeURL').val();
    console.log(ytLink);
    $('.loader').toggleClass('dontShow');
    $('.enterUrlDiv button').prop('disabled', true);

    $.ajax({
        url: '/Home/GetGeneralData',
        type: 'POST',
        data: JSON.stringify(ytLink),
        contentType: 'application/json',
        success: function (result) {
            $('.loader').toggleClass('dontShow');
            $('.loadedDataDiv').css('background-color', 'lightblue');

            $('.dataDiv').toggleClass('dontShow');
            $('.dataDiv').html(result);
        }
    });
}

function ListAllFormats() {
    let audioClicked = document.querySelector('#c1-13').checked;
    let videoClicked = document.querySelector('#c1-14').checked;

    if (audioClicked && videoClicked) {
        $.ajax({
            url: '/Home/Get2BestFormats',
            type: 'POST',
            data: JSON.stringify(ytLink),
            contentType: 'application/json',
            success: function (result) {
                $('.formatsDiv').html(result);
                console.log(result);
            }
        });
    }
    else if (audioClicked) {
        $.ajax({
            url: '/Home/GetAudioFormats',
            type: 'POST',
            data: JSON.stringify(ytLink),
            contentType: 'application/json',
            success: function (result) {
                $('.formatsDiv').html(result);
                console.log(result);
            }
        });
    }
    else if (videoClicked) {
        $.ajax({
            url: '/Home/GetVideoFormats',
            type: 'POST',
            data: JSON.stringify(ytLink),
            contentType: 'application/json',
            success: function (result) {
                $('.formatsDiv').html(result);
                console.log(result);
            }
        });
    }
    else {
        console.log("error");
    }
}

function downloadFile() {
    var selectElement = document.getElementById("quality");

    var selectedValue = selectElement.options[selectElement.selectedIndex].value;
    var selectedText = selectElement.options[selectElement.selectedIndex].text;

    console.log(selectedValue);
    console.log(selectedText);

    let dataJSON = {
        "selectedValue": selectedValue,
        "ytLink": ytLink,
    };


    $.ajax({
        url: '/Home/GetVideoById',
        type: 'POST',
        data: JSON.stringify(dataJSON),
        contentType: 'application/json',
        success: function (result) {
            //$('.availableFormatsDiv').html(result);
            console.log(result);
        }
    });
}