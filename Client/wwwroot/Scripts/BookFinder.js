var table;
$(document).ready(function () {
    debugger;
    $('#BookData').on('keyup', function () {
        table
            .columns(6)
            .search(this.value)
            .draw();
    });
    table = $("#BookData").DataTable({
        serverSide: true,
        ajax: "/BookFinders/PageData",
        "columns": [
            { "data": "isbn" },
            { "data": "title" },
            { "data": "author" },
            { "data": "publisher" },
            {
                "render": function (data, type, row) {
                    return moment(row.published).format('MMMM Do YYYY');
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
function GetById(Id) {
    debugger;
    $.ajax({
        url: "/BookFinders/GetByID/",
        type: "GET",
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        data: { id: Id },
        success: function (result) {
            debugger;
            $('#Id').val(result[0]['id']);
            $('#Isbn').val(result[0]['isbn']);
            $('#Title').val(result[0]['title']);
            $('#Author').val(result[0]['author']);
            $('#Publisher').val(result[0]['publisher']);
            $('#Published').val(moment(result[0]['published']).format("L"));
            //$('#Published').val(result[0]['published']);
            $('#URL').val(result[0]['url']);
            $('#Modal').modal('show');
            $('#Update').show();
            $('#Add').hide();

        }
    })
}
function clearTextBox() {
    $('#Id').val("");
    $('#Isbn').val("");
    $('#Title').val("");
    $('#Author').val("");
    $('#Publisher').val("");
    $('#Published').val("");
    $('#Update').hide();
    $('#Add').show();
    $('#Name').css('border-color', 'lightgrey');
}
function Add() {
    debugger;
    var book = new Object();
    book.Id = $('#Id').val();
    book.Isbn = $('#Isbn').val();
    book.Title = $('#Title').val();
    book.Author = $('#Author').val();
    book.Publisher = $('#Publisher').val();
    book.published = $('#Published').val();
    book.file = $('#file').val();

    $.ajax({
        url: "/BookFinders/Insert/",
        data: book,
        type: "POST"
    }).then((result) => {
        if (result.statusCode == 200) {
            debugger;
            clearTextBox();
            $('#Modal').modal('hide');
            Swal.fire({
                position: 'center',
                type: 'success',
                title: 'Insert Successfully',
                showConfirmButton: false,
                timer: 1500
            });
            table.ajax.reload();
        }
        else {
            Swal.fire('Error', 'Insert Failed', 'error');
        }
    });
}
function Update() {
    debugger;
    var book = new Object();
    book.Id = $('#Id').val();
    book.Isbn = $('#Isbn').val();
    book.Title = $('#Title').val();
    book.Author = $('#Author').val();
    book.Publisher = $('#Publisher').val();
    book.Published = $('#Published').val();
    book.URL = $('#URL').val();
    $.ajax({
        url: "/BookFinders/Update/",
        data: book,
        type: "POST"
    }).then((result) => {
        if (result.statusCode == 200) {
            debugger;
            $('#Modal').modal('hide');
            clearTextBox();
            Swal.fire({
                position: 'center',
                type: 'success',
                title: 'Update Successfully',
                showConfirmButton: false,
                timer: 6000
            });
            table.ajax.reload();
        }
        else {
            Swal.fire('Error!', 'Update Failed.', 'error');
        }
    })
}
function Delete(id) {
    debugger;
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                url: "/BookFinders/Delete/",
                type: "DELETE",
                data: { id: id },
                success: function (result) {
                    Swal.fire({
                        position: 'center',
                        type: 'success',
                        title: 'Delete Successfully'
                    });
                    tableitem.ajax.reload();
                },
                error: function (result) {
                    Swal.fire('Error', 'Failed to Delete', 'error');
                    ClearScreen();
                }
            });
        }
    })
}
//Valdidation using jquery
function validate() {
    var isValid = true;
    if ($('#Name').val().trim() == "") {
        $('#Name').css('border-color', 'Red');
        $('#Name').focus();
        isValid = false;
    }
    else {
        $('#Name').css('border-color', 'lightgrey');
    }

    return isValid;
}
$(function () {
    $("[id*=btnSweetAlert]").on("click", function () {
        var id = $(this).closest('tr').find('[id*=id]').val();
        swal({
            title: 'Are you sure?' + ids,
            text: "You won't be able to revert this!" + id,
            type: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes',
            cancelButtonText: 'No',
            confirmButtonClass: 'btn btn-success',
            cancelButtonClass: 'btn btn-danger',
            buttonsStyling: false
        }).then(function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "Default.aspx/DeleteClick",
                    data: "{id:" + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (r) {
                        if (r.d == "Deleted") {
                            location.reload();
                        }
                        else {
                            swal("Data Not Deleted", r.d, "success");
                        }
                    }
                });
            }
        },
            function (dismiss) {
                if (dismiss == 'cancel') {
                    swal('Cancelled', 'No record Deleted', 'error');
                }
            });
        return false;
    });
});