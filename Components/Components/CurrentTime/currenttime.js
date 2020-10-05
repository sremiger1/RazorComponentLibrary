import 'jquery';
import 'jquery-ui';


$(function() {
    $(".widget input[type=submit], .widget a, .widget button").button();
    $("button, input, a").click(function(event) {
        event.preventDefault();
    });
    $("button").click(function() {
        alert("Handler for .click() called.");
    });
});