$(document).ready(function ($) {
    /*updateRankings();
    populateAutocomplete();*/
    $('.login').hover(
        function () {
            $(this).popover('show');
        },
        function () {
        }
        );
});

function validateMatch() {
    var team1player1 = $("#Team1Player1").val();
    var team2player1 = $("#Team2Player1").val();
    var team1score = $('#Score1').val();
    var team2score = $('#Score2').val();
    if (!$.isNumeric(team1score) || !$.isNumeric(team2score)) {
        $('#warnings').html('       <div id="warnings" class="alert alert-danger"> <a class="close" data-dismiss="alert" href="#">&times;</a>Score must be a number</div>');
        return false;
    }
    
    if (!team1player1 || !team2player1) {
        $('#warnings').html('       <div id="warnings" class="alert alert-danger"> <a class="close" data-dismiss="alert" href="#">&times;</a>Both teams require a player</div>');
        return false;
    }

    return true;
}

/*
function updateRankings() {
    var request = $.get("get_rankings", function (data) {
        $('#rankings').empty();
        $('#rankings').append(data);
        console.log(data);
    });
}

function populateAutocomplete() {
    var request = $.get("get_aliases", function (data) {
        $('#aliases').empty();
        $('#aliases').append(data);
        console.log(data);
    });
}
*/