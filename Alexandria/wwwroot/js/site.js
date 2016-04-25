// Write your Javascript code.


$(document).ready(function () {
    $(document).on('click',"a:not(.noAjax)",function (event) {
        event.preventDefault();

        var href = $(event.target).attr('href');
        $.get(href, function (data) {
            $('#library').html(data);
            history.pushState(null, document.title, href);
        }, "html");

        return false; //for good measure
    });

    $('video,audio').mediaelementplayer(/* Options */);

    $(document).on('click', '.audio-play', function (event) {
        event.preventDefault();

        $('#audioPlayer').attr('src', '/API/Stream?id=' + $(event.target).data('file-id'));

        return false; //for good measure
    });
});