
var categoriesSelect = productEdit.categories;
function changeOption() {

    var selection = document.getElementById("selection");

    var selectedOption = categoriesSelect.options[categoriesSelect.selectedIndex];
    if (selectedOption.text == "Комплекты" || selectedOption.text == "Ролевое бельё"){
        selection.style.visibility = "visible";
    }
}

categoriesSelect.addEventListener("change", changeOption);
 