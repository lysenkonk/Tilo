
var categoriesSelect = productEdit.categories;
function changeOption() {

    var selection = document.getElementById("selection");

    var selectedOption = categoriesSelect.options[categoriesSelect.selectedIndex];
    if (selectedOption.text == "Комплекты") {
        selection.style.visibility = "visible";
    }
}

categoriesSelect.addEventListener("change", changeOption);
 