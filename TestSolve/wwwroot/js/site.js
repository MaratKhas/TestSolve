$(document).ready(function () {
    $("#StudentCountForm").validate({
        rules: {
            CountStudent: {
                required: true,
                range: [2,100]
            },
            
        },
        messages: {
            CountStudent: {
                required: "Это поле обязательно для заполнения",
                range : "Количество студентов должно быть от 2 до 100"
            },
           
        }   
   });
});