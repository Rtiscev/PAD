let ytLink
let fileName;
function GetInformation() {
    ytLink = document.getElementById('youtube-url').value;
    console.log(ytLink);


    $.ajax({
        url: '/Home/GetD',
        type: 'POST',
        data: JSON.stringify(ytLink),
        contentType: 'application/json',
        success: function (result) {
            $('.ytData').html(result);
            console.log(result);
        }
    });
}

function ListAllFormats() {
    fileName = document.getElementById('ModelTitle').value;

    const selectedOptionFileType = document.querySelector('input[name="fileGroup"]:checked');

    if (selectedOptionFileType) {
        console.log("Selected option:", selectedOptionFileType.value); // For debugging
    } else {
        console.log("No option selected");
    }

    if (selectedOptionFileType.value == "Video") {
        $.ajax({
            url: '/Home/GetVideoFormats',
            type: 'POST',
            data: JSON.stringify(ytLink),
            contentType: 'application/json',
            success: function (result) {
                $('.availableFormatsDiv').html(result);
                console.log(result);
            }
        });
    }
    else if (selectedOptionFileType.value == "Audio") {
        $.ajax({
            url: '/Home/GetAudioFormats',
            type: 'POST',
            data: JSON.stringify(ytLink),
            contentType: 'application/json',
            success: function (result) {
                $('.availableFormatsDiv').html(result);
                console.log(result);
            }
        });
    }
    else {

    }

    //$.ajax({
    //    url: '/Home/DownloadFile',
    //    type: 'POST',
    //    data: JSON.stringify(fileName),
    //    contentType: 'application/json',
    //    success: function (result) {
    //        $('.availableFormatsDiv').html(result);
    //        console.log(result);
    //    }
    //});
    // call both of them ez clap
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