let ytLink;
let fileName;
let selectDiv;
let buttonDiv;
let audioClicked;
let videoClicked;

let fileID;
let audioType;

$('#youtubeURL').on('keydown', function (event) {
    // Check if the pressed key is Enter
    if (event.key === 'Enter') {
        GetInformation(); // Call your function
    }
});

function GetInformation() {
    ytLink = $('#youtubeURL').val();
    console.log(ytLink);
    $('.loader').toggleClass('dontShow');
    $('.loader').toggleClass('loaderAnim');
    $('.enterUrlDiv button').prop('disabled', true);

    $.ajax({
        url: '/Home/GetGeneralData',
        type: 'POST',
        data: JSON.stringify(ytLink),
        contentType: 'application/json',
        success: function (result) {
            $('.loader').toggleClass('dontShow');
            //$('.loadedDataDiv').css('background-color', 'lightblue');
            $('.loadedDataDiv').toggleClass('dataDivAnim');

            $('.dataDiv').toggleClass('dontShow');
            $('.dataDiv').html(result);
        }
    });
}

function ListAllFormats() {
    audioClicked = document.querySelector('#c1-13').checked;
    videoClicked = document.querySelector('#c1-14').checked;

    selectDiv = $('.select');
    buttonDiv = $('.downloadDiv');
    buttonDiv.on('click', function () {
        let str = $('.select input:checked + label').attr('for');
        let result = str.replace("opt", "");
        console.log(result);
        downloadFile(result);
    });

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

function downloadFile(id) {
    let dataJSON = {
        "id": id,
        "ytLink": ytLink
    };
    console.log("downloadFile:", dataJSON);

    $.ajax({
        url: '/Home/DownloadFileById',
        type: 'POST',
        data: JSON.stringify(dataJSON),
        contentType: 'application/json',
        success: function (result) {
            if (result != "already downloaded") {
                console.log("downloadFile result:", result);
                if (result != "file exists!" && result != "yt-dlp fail") {
                    fileID = result["response"];
                }
                console.log("fileID:", fileID);
                loadOriginalAudioSource();
            }
        }
    });
}

function loadOriginalAudioSource() {
    $.ajax({
        url: '/Home/GetBytesFromFile',
        type: 'POST',
        data: JSON.stringify(fileID),
        contentType: 'application/json',
        success: function (result) {
            loadAudioInformation();

            let audio = $('#audioOriginal source');
            let _type = result['contentType'];
            let _src = `data:${result['contentType']};base64,${result['bytes']}`;

            let tempik = result['contentType'];
            if (tempik.includes("video/")) {
                tempik = tempik.replace("video/", "");
            }
            else if (tempik.includes("audio/")) {
                tempik = tempik.replace("audio/", "");
            }
            console.log(tempik);

            audioType = tempik;
            audio.attr('src', _src);
            audio.attr('type', _type);
            $('#audioOriginal')[0].load();
            console.log(result);

            $('.formatsBox').children().each(function () {
                if ($(this).text().toLowerCase() === audioType.toLowerCase()) {
                    console.log("found");
                    $(this).remove(); // This line removes the matched item
                    //$('.formatsBox').val($(this).text().toLowerCase());       
                }
            });
        }
    });
}
function loadAudioInformation() {
    $.ajax({
        url: '/Home/GetAudioInformation',
        type: 'POST',
        data: JSON.stringify(fileID),
        contentType: 'application/json',
        success: function (result) {
            loadAudioVisual();

            let sliced = result;
            // Remove the first and last character
            if (sliced[0] == "\"" && sliced[sliced.length - 1] == "\"") {
                sliced = result.slice(1, -1);
            }
            let delimiter = "\\n";
            let output = sliced.split(delimiter);

            let audioInfoDiv = $(".audioInformation");
            for (let i = 0; i < output.length; i++) {
                audioInfoDiv.append($('<p></p>').text(output[i]));
            }
            console.log(result);
        }
    });
}
function loadAudioVisual() {
    $.ajax({
        url: '/Home/GetAudioVisual',
        type: 'POST',
        data: JSON.stringify(fileID),
        contentType: 'application/json',
        success: function (result) {
            let visualDiv = $('.visualRepresentation img');

            // Create the <source> element using jQuery
            let src = `data:image/png;base64,${result['bytes']}`;

            visualDiv.attr('src', src);

            //.text(output);
            console.log(result);
        }
    });
}
