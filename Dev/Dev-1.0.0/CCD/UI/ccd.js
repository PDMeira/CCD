function loadCounter() {
    //console.log("Load Counter");
    $(".bodyRight .txtArea").appendTo('body').addClass('loader_overlay');
    $('.bodyRight .txtArea').appendTo('body').addClass('loader').html('<div><span></span></div><div><span></span></div><div><span></span></div><div><span></span></div><div><span></span></div><div><span></span></div><div><span></span></div><div><span></span></div><div><span></span></div>');
};

function finishLoading() {
    //console.log("Unloading Counter");
    $('.loader_overlay').remove();
    $('.loader').remove();
}

$(function () {
    $(".mergeccd").click(function () {
        $.blockUI.defaults.css = {};
        $.blockUI();
        setTimeout($.unblockUI, 3000);
    });
});

