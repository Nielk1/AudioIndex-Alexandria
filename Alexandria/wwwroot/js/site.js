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

    var audioPlayer = null;

    new MediaElementPlayer('#audioPlayer', {
        features: ['playpause', 'progress', 'current', 'duration', 'volume'/*, 'fullscreen'*/],
        loop: false,
        type: 'audio/mp3',
        success: function (mediaElement, originalNode) {
            audioPlayer = mediaElement;
        }
    });

    $(document).on('click', '.audio-play', function (event) {
        event.preventDefault();

        audioPlayer.setSrc('/API/Stream?id=' + $(event.target).data('file-id'));
        audioPlayer.play();

        return false; //for good measure
    });
});