async function initCountrySelect(selectId, selectedValue = "") {

    const response = await fetch("/data/countries.json");

    const countries = await response.json();

    const select = document.getElementById(selectId);

    if (!select)
        return;

    select.innerHTML = "";

    const firstOption = document.createElement("option");

    firstOption.value = "";

    firstOption.text = "-- Chọn quốc tịch --";

    select.appendChild(firstOption);

    countries
        .sort((a, b) => a.name.localeCompare(b.name))
        .forEach(country => {

            const option = document.createElement("option");

            option.value = country.name;

            option.text = country.name;

            if (country.name === selectedValue)
                option.selected = true;

            select.appendChild(option);

        });

    $("#" + selectId).select2({

        placeholder: "Chọn hoặc tìm quốc tịch",

        allowClear: true,

        width: "100%",

        matcher: function (params, data) {

            if ($.trim(params.term) === "")
                return data;

            if (typeof data.text === "undefined")
                return null;

            const keyword = params.term
                .toLowerCase()
                .trim();

            const text = data.text
                .toLowerCase();

            // bắt đầu bằng ký tự nhập

            if (text.startsWith(keyword))
                return data;

            return null;
        }

    });

}