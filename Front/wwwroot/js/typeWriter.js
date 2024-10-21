function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

let timer = 0;

async function run() {
    await sleep(6 * 1000 + 500);
    timer += 6 * 1000;

    for (let i = 0; i < insertableTxt.length; i++) {
        let string = insertableTxt[i];
        // toggle on animation
        elements[i].toggleClass("blinkingAnimation");

        for (let j = 0; j <= string.length; j++) {
            elements[i].text(string.slice(0, j));
            timer += delay;
            await sleep(delay);
        }

        // toggle off animation
        elements[i].toggleClass("blinkingAnimation");
    }

    $('.line').toggleClass('lineAnimation');
    $('.txtScroller span').css('color', 'skyblue');
}

const delay = 100;

const lineDiv = $('.projectTopicDiv .wrapper .txtScroller .line').eq(0);
const continuationID = $('#continuationID');
const downloadID = $('#downloadID');

const insertableTxt =
    ['Download',
        'Video',
        'from youtube and change its quality'
    ];
const elements = [downloadID, lineDiv, continuationID];


run();