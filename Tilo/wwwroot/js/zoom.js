
$(function () {
    var bgImg = document.getElementById("img_Bg");
    let smImages = document.getElementsByClassName("sm_img");
    //smImages.forEach(function (element) { element.addEventListener("click", tabbedImage(e), false) });
    for (let i = 0; i < smImages.length; i++) {
        smImages[i].addEventListener("click", function (e) { tabbedImage(e) }, false);
    }

    function tabbedImage(e) {

        let imgName = e.target.getAttribute("name");
        //let imgName = imgTitle.slice(0, imgTitle.indexOf("."));
        let imgSrc = "/Files/Bg/" + imgName;
        bgImg.setAttribute('src', imgSrc);

    }
})