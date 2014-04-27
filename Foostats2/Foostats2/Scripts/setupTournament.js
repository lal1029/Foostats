var tournamentMaker = (function (jQuery) {

    var myPrivateVar, myPrivateMethod, self, tournamentUsers, rows,
        tournamentContainer, button, submitTournament, teamMapping;

    // A private counter variable
    myPrivateVar = 0;

    rows = 1;

    var minimalData = {
        teams: [
          ["Team 1", "Team 2"], /* first matchup */
          ["Team 3", "Team 4"]  /* second matchup */
        ],
        results: [
          [[1, 2], [3, 4]],       /* first round */
          [[4, 6], [2, 1]]        /* second round */
        ]
    }

    // A private function which logs any arguments
    myPrivateMethod = function (foo) {
        console.log(foo);
    };

    AddRow = function () {
        rows++;
        var name = "Team " + rows;
        var placeholder1 = "Player 1";
        var placeholder2 = "Player 2";
        var player1Name ="Team" + rows + "Player1";
        var player2Name = "Team" + rows + "Player2";

        var row = jQuery('<div> </div>').addClass('row').appendTo(tournamentUsers);
        var teamName = jQuery('<div> </div>').addClass('col-md-2').text(name).appendTo(row);
        var player1Col = jQuery('<div> </div>').addClass('col-md-5').appendTo(row);
        var player1Input = jQuery('<input list="aliases" type="text" autocomplete="on" class="form-control"  />').attr('id', player1Name).attr('name', player1Name).attr('placeholder', placeholder1).appendTo(player1Col);
        var player2Col = jQuery('<div> </div>').addClass('col-md-5').appendTo(row);
        var player2Input = jQuery('<input list="aliases" type="text" autocomplete="on" class="form-control"  />').attr('id', player2Name).attr('name', player2Name).attr('placeholder', placeholder2).appendTo(player2Col);

        button.appendTo(tournamentContainer);

    }

    GetUsers = function()
    {
        var TeamArray = new Array();
        for (var i = 1 ; i <= rows; i++) {
            var player1Name = "#Team" + i + "Player1";
            var player2Name = "#Team" + i + "Player2";
            var teamNumber = "Team " + i;

            var player1 = jQuery(player1Name).val();
            var player2 = (jQuery(player2Name).val())? jQuery(player2Name).val() : null ;

            var team = { "Player1": player1, "Player2": player2 , "TeamName": teamNumber};
            TeamArray.push(team);

        }

        jQuery.ajax({
            type: 'POST',
            url: 'Tournament/CreateBracket',
            data:  JSON.stringify(TeamArray) ,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (r) {
                self.CreateBracket(r);

            },
            error: function () {
                alert("Failure");
            }
        });

    }

    CreateBracket = function (teams) {
        var bracket = {};
        var Teams = new Array();
        this.teamMapping = {};

        for (var j = 0; j < teams.length; j++)
        {
            this.teamMapping[teams[j].TeamName] = [teams[j].Player1, teams[j].Player2];
        }

        for (var i = 0 ; i < teams.length; i+=2) {
            var round = [teams[i].TeamName, teams[i+1].TeamName];
            Teams.push(round);
        }

        var minimalData = {
            teams: Teams,
            results: []
        }

        $('#minimal .demo').bracket({
            init: minimalData,/* data to initialize the bracket with */
            save: function () { }
        })

        self.ShowMatch();
    }

    ShowMatch = function ()
    {
        submitTournament.show();
    }

    GetMatchData = function () {
        var matches = jQuery(".match");
        var outer = this;
        var MatchArray = new Array();
        matches.each(function (i) {
            var matchData = jQuery(this).find(".editable");
            var team1Name = matchData[0].innerText;
            var team1Score = matchData[1].innerText;
            var team2Name = matchData[2].innerText;
            var team2Score = matchData[3].innerText;

            var team1Players = outer.teamMapping[team1Name];
            var team2Players = outer.teamMapping[team2Name];

            var match = { "Team1Players": team1Players, "Team2Players": team2Players, "Team1Score": team1Score, "Team2Score": team2Score };
            MatchArray.push(match);

        }
        );

        jQuery.ajax({
            type: 'POST',
            url: 'Tournament/AddMatches',
            data: JSON.stringify(MatchArray),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (r) {
                alert("Sucess");
                //self.CreateBracket(r);

            },
            error: function () {
                alert("Failure");
            }
        });

    }

    //assign reference to current object to self
    self = this;

    return {

        // A public function utilizing privates
        myPublicFunction: function (bar) {

            // Increment our private counter
            myPrivateVar++;

            // Call our private method using bar
            myPrivateMethod(bar);

        },

        Submit : function()
        {
            self.GetUsers();
            button.hide();
            
        },

        SubmitTournament : function () {
            submitTournament.hide();
            self.GetMatchData();
        },

        onEnter: function (key) {
            if (key.which === jQuery.ui.keyCode.ENTER) {
                self.AddRow();
            }
        },

        init: function () {
            tournamentUsers = jQuery("#tournamentUsers");
            tournamentContainer = jQuery("#tournamentContainer");
            button = jQuery("#submit");
            jQuery(tournamentUsers).bind("keyup", this.onEnter);
            button.bind("click", this.Submit);
            submitTournament = jQuery("#submitTournament");
            submitTournament.bind("click", this.SubmitTournament);
        }
    };

})($);


$(document).ready(function ($) {
    tournamentMaker.init();
});

