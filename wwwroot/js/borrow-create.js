$(function () {

    //--------------------------------------------------------
    // Select2 - Resident
    //--------------------------------------------------------

    $("#ResidentId").select2({

        placeholder: "Chọn cư dân",

        allowClear: true,

        ajax: {

            url: "/Admin/Borrow/SearchResidents",

            dataType: "json",

            delay: 300,

            data: function (params) {

                return {
                    term: params.term
                };

            },

            processResults: function (data) {

                return {
                    results: data
                };

            }

        }

    });


    //--------------------------------------------------------
    // Select2 - Book
    //--------------------------------------------------------

    $("#BookSearch").select2({

        placeholder: "Chọn sách",

        allowClear: true,

        ajax: {

            url: "/Admin/Borrow/SearchBooks",

            dataType: "json",

            delay: 300,

            data: function (params) {

                return {
                    term: params.term
                };

            },

            processResults: function (data) {

                return {
                    results: data
                };

            }

        }

    });


    //--------------------------------------------------------
    // Danh sách sách đã chọn
    //--------------------------------------------------------

    let books = [];


    //--------------------------------------------------------
    // Add Book
    //--------------------------------------------------------

    $("#btnAddBook").click(function () {

        const id = $("#BookSearch").val();

        if (!id) {

            alert("Vui lòng chọn sách.");

            return;
        }

        if (books.some(x => x.bookId == id)) {

            alert("Sách đã được chọn.");

            return;
        }

        const data = $("#BookSearch").select2("data")[0];

        const text = data.text;

        const parts = text.split("|");

        books.push({

            bookId: parseInt(id),

            title: parts[0].trim(),

            authorName: parts[1].trim(),

            categoryName: parts[2]
                .replace(/\(Còn:.+\)/, "")
                .trim()

        });

        renderTable();

        $("#BookSearch")
            .val(null)
            .trigger("change");

    });


    //--------------------------------------------------------
    // Remove Book
    //--------------------------------------------------------

    $(document).on("click", ".btn-remove-book", function () {

        const id = $(this).data("id");

        books = books.filter(x => x.bookId != id);

        renderTable();

    });


    //--------------------------------------------------------
    // Render Table
    //--------------------------------------------------------

    function renderTable() {

        let html = "";

        if (books.length === 0) {

            html = `
<tr>
    <td colspan="5"
        class="text-center text-muted">

        Chưa chọn sách

    </td>
</tr>`;

            $("#bookTableBody").html(html);

            $("#hiddenBooks").html("");

            return;
        }

        books.forEach(function (item, index) {

            html += `
<tr>

    <td>${index + 1}</td>

    <td>${item.title}</td>

    <td>${item.authorName}</td>

    <td>${item.categoryName}</td>

    <td class="text-center">

        <button
            type="button"
            class="btn btn-danger btn-sm btn-remove-book"
            data-id="${item.bookId}">

            <i class="bi bi-trash"></i>

        </button>

    </td>

</tr>`;

        });

        $("#bookTableBody").html(html);

        renderHiddenInputs();

    }


    //--------------------------------------------------------
    // Hidden Inputs
    //--------------------------------------------------------

    function renderHiddenInputs() {

        let html = "";

        books.forEach(function (item, index) {

            html += `
<input
type="hidden"
name="Books[${index}].BookId"
value="${item.bookId}" />

<input
type="hidden"
name="Books[${index}].Title"
value="${item.title}" />

<input
type="hidden"
name="Books[${index}].AuthorName"
value="${item.authorName}" />

<input
type="hidden"
name="Books[${index}].CategoryName"
value="${item.categoryName}" />`;

        });

        $("#hiddenBooks").html(html);

    }

});