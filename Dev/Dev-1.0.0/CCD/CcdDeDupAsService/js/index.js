var sessionGuid;

function S4() {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

  $(document).ajaxStart(function () {
      $.blockUI({
          theme: true,
          title: 'CCD Deduplication in process...',
          message: '<p>Your consolidated CCD is being prepared...</p><br><p>You will be redirected momentarily...</p>',
          backgroundColor: '#00f'
      });
  });
  
  $(document).ajaxStop(function () {
      $.unblockUI();
  });
  
$(function () {
    

  

    $(".ccdButton").click(function () {
        sendRule();
    });
    sessionGuid = (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
    var fileTemplate = "<div id=\"{{id}}\" class=\"files\">";
    //fileTemplate += "<div class=\"progressbar\"></div>";
    //fileTemplate += "<div class=\"preview\"></div>";
    fileTemplate += "<div class=\"filename\">{{filename}}</div>";
    fileTemplate += "</div>";

    function slugify(text) {
        text = text.replace(/[^-a-zA-Z0-9,&\s]+/ig, '');
        text = text.replace(/-/gi, "_");
        text = text.replace(/\s/gi, "-");
        return text;
    }

    $("#dropbox").html5Uploader({
        onClientLoadStart: function (e, file) {
            var upload = $("#upload");
            if (upload.is(":hidden")) {
                upload.show();
            }
            upload.append(fileTemplate.replace(/{{id}}/g, slugify(file.name)).replace(/{{filename}}/g, file.name));
        }
        //onClientLoad: function (e, file) { $("#" + slugify(file.name)).find(".preview").append("<img src=\"" + e.target.result + "\" alt=\"\">"); },
        //onServerLoadStart: function (e, file) { $("#" + slugify(file.name)).find(".progressbar").progressbar({ value: 0 }); }
        //onServerProgress: function (e, file) {
        //    if (e.lengthComputable) {
        //        var percentComplete = (e.loaded / e.total) * 100;
        //        $("#" + slugify(file.name)).find(".progressbar").progressbar({ value: percentComplete });
        //    }
        //},
        //onServerLoad: function (e, file) { $("#" + slugify(file.name)).find(".progressbar").progressbar({ value: 100 }); }
    });
});

