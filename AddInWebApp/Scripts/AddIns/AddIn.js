function ReloadAll() {
    $.post("/AddInsDomain/AddInsApi/ReloadAll", function (result) {
        console.log(result);
        alert(result.errmsg);
    });
}

function Load(ID) {
    $.post("/AddInsDomain/AddInsApi/Load", 'Params={"AppDomainID":"' + ID + '"}', function (result) {
        console.log(result);
        alert(result.errmsg);
    });
}

function Unload(ID) {
    $.post("/AddInsDomain/AddInsApi/Unload", 'Params={"AppDomainID":"' + ID + '"}', function (result) {
        console.log(result);
        alert(result.errmsg);
    })
}

function Append(Name, Description) {
    $.post("/AddInsDomain/AddInsApi/AddIns", 'Params={"Name":"' + Name + '","Description":"' + Description + '"}', function (result) {
        console.log(result);
        alert(result.errmsg);
    });
}



function upload(files, id, name) {
    var xhr = new XMLHttpRequest();
    var formData = new FormData();
    FormData.append('Params', 'AppDomainID=' + id + '&DllName=' + name);
    for (var i = 0; i < files.length; i++) {
        formData.append(files[i].name + i.toString(), files[i]);
    }
    // index 为第 n 个文件的索引
    xhr.open('post', '/AddInsDomain/AddInsService/FileUpload'); // url 为提交的后台地址
    xhr.send(formData);
    xhr.upload.addEventListener("progress", uploadProgress, false); // 处理上传进度
    xhr.addEventListener("load", uploadComplete, false); // 处理上传完成
    xhr.addEventListener("error", uploadFailed, false); // 处理上传失败
    xhr.addEventListener("abort", uploadCanceled, false); // 处理取消上传
}

function uploadProgress(event) {
    if (event.lengthComputable) {
        var percentComplete = event.loaded / event.total;
        console.log(percentComplete);
        //$("#rat").html(percentComplete);
    }
}

function uploadComplete(e) { console.log(e) }
function uploadFailed(e) { console.log(e) }
function uploadCanceled(e) { console.log(e) }

function fileSelected(file) {
    //var MAXWIDTH = 79;  // 最大图片宽度
    //var MAXHEIGHT = 79;  // 最大图片高度
    var f = Array();
    f.push(file.files);
    if (file.files && file.files[0]) {
        console.log(f);
        //return f;
        //var img = document.getElementById('preview');
        //var i = parseInt($("#hid").val()) + 1;
        //$("#hid").val(i.toString());
        //$("#form1").append("<img src='' id='" + i.toString() + "' />");
        //$("#imgBox").append("<img class=\"weui_uploader_file\" src='' id='" + i.toString() + "' />");

        //var img = document.getElementById(i.toString());
        //img.onload = function () {
        //    var rect = clacImgZoomParam(MAXWIDTH, MAXHEIGHT, img.offsetWidth, img.offsetHeight);
        //    img.width = rect.width;
        //    img.height = rect.height;
        //}
        //var reader = new FileReader();
        //reader.onload = function (evt) {
        //    img.src = evt.target.result;
        //}
        //reader.readAsDataURL(file.files[0]);
    }
    else {
        console.log("没有选择文件！");
    }
}




