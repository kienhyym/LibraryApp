function initSelect2(selector, placeholder) {

    $(selector).select2({

        placeholder: placeholder,

        allowClear: true,

        width: "100%",

        language: {
            noResults: function () {
                return "Không tìm thấy dữ liệu";
            }
        }

    });

}