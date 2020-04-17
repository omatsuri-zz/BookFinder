var table;
$(document).ready(function () {
    debugger;
    $('#BookData').on('keyup', function () {
        table
            .columns(6)
            .search(this.value)
            .draw();
    });
    //tableitem = $("#BookData").DataTable({
    //    serverSide: true,
    //    //ajax: "/BookFinders/PageData",
    //    "columns": [
    //        { "data": "isbn" },
    //        { "data": "title" },
    //        { "data": "author" },
    //        { "data": "publisher" },
    //        {
    //            data: "published",
    //            "render": function (data) {
    //                return moment(data).format('DD/MM/YYYY');
    //            }
    //        },
    //        { "data": "url" },
    //        {
    //            "render": function (data, type, row) {
    //                return '<button type="button" class="btn btn-warning" id="Edit" onclick="return GetById(' + row.id + ')"><i class="material-icons" data-toggle="tooltip" title="Edit">&#xE254;</i></button> ' +
    //                    '<button type = "button" class="btn btn-danger" id="Delete" onclick="return Delete(' + row.id + ')" ><i class="material-icons" data-toggle="tooltip" title="Delete">&#xE872;</i></button'
    //            }
    //        }
    //    ]
    //});
});
function getJSessionId() {
    var jsId = document.cookie.match(/JSESSIONID=[^;]+/);
    if (jsId != null) {
        if (jsId instanceof Array)
            jsId = jsId[0].substring(11);
        else
            jsId = jsId.substring(11);
    }
    return jsId;
}
function clearTextBox() {
    $('#Id').val("");
    $('#Isbn').val("");
    $('#Title').val("");
    $('#Author').val("");
    $('#Publisher').val("");
    $('#Update').hide();
    $('#Add').show();
    $('#Name').css('border-color', 'lightgrey');
}
function Search() {
    debugger;
    var book = new Object();
    book.Id = $('#Id').val();
    book.Isbn = $('#Isbn').val();
    book.Title = $('#Title').val();
    book.Author = $('#Author').val();
    book.Publisher = $('#publisher').val();
    $.ajax({
        url: "/BookFinders / FindBook / " + book.Title + " / " + book.Author + " / " + book.publisher + " /" + book.Isbn,
        type: "POST"
    }).then((result) => {
        if (result.statusCode == 200) {
            debugger;
            clearTextBox();
            $('#Modal').modal('hide');
            table.visible = true;
            //table.ajax.reload();
            //table.ajax.load();
            table = $("#BookData").DataTable({
                serverSide: true,
                ajax: "/BookFinders/FindBook/" + $('#Title').val() + "/" + $('#Author').val() + "/" + $('#publisher').val() + "/" + $('#Isbn').val(),
                "columns": [
                    { "data": "isbn" },
                    { "data": "title" },
                    { "data": "author" },
                    { "data": "publisher" },
                    {
                        data: "published",
                        "render": function (data) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                    },
                    { "data": "url" },
                    {
                        "render": function (data, type, row) {
                            return '<button type="button" class="btn btn-warning" id="Edit" onclick="return GetById(' + row.id + ')"><i class="material-icons" data-toggle="tooltip" title="Edit">&#xE254;</i></button> ' +
                                '<button type = "button" class="btn btn-danger" id="Delete" onclick="return Delete(' + row.id + ')" ><i class="material-icons" data-toggle="tooltip" title="Delete">&#xE872;</i></button'
                        }
                    }
                ]
            });
        }
        else {
            Swal.fire('Error', 'Insert Failed', 'error');
        }
    });
}