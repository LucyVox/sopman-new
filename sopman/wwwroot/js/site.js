// Write your JavaScript code.


$(document).ready(function() {
    $(".close-menu").click(function(){
        $(".side-menu").removeClass("show");
        $(".side-menu").addClass("hide");
    });

    $(".shaded-bg").click(function(){
        $(".side-menu").removeClass("show");
        $(".side-menu").addClass("hide");
    });

    $('input[name=radiolimit]').click(function(){
        if ($(this).is(':checked')) {
            $('.limit-form').removeAttr("disabled"); 
        }
    });
     $('input[name=auto-gen]').click(function(){
        if ($(this).is(':checked')) {
            $('.prefix-form').removeAttr("disabled"); 
            $('.suffix-form').removeAttr("disabled"); 
            $('.gen-radio-suff').addClass("show-label"); 
            $('.gen-radio-pre').addClass("show-label");
        }
    });
    $('input[name=radioallowlimitcode]').click(function(){
        if ($(this).is(':checked')) {
            $('.limit-form-lim').removeAttr("disabled"); 
        }
    });

    $('input[name=auto-gen-lim-char]').click(function(){
        if ($(this).is(':checked')) {
            $('.limit-form-char').removeAttr("disabled"); 
        }
    });



    $('.tick-img.true img').attr('src', '/images/tick.png');

    $(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });
});  
    




    


