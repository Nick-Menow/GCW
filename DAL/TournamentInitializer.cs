using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using WorkDB.Models;

namespace WorkDB.DAL
{
    public class TournamentInitializer : DropCreateDatabaseIfModelChanges<TournamentContext>
    {
        protected override void Seed(TournamentContext context)
        {


            var players = new List<Player>
            {
                new Player{Firstname="Nick",LastName="Summers"},
                new Player{Firstname="Dick",LastName="Vig"},
                new Player{Firstname="Den",LastName="Ju"},
                new Player{Firstname="Cold",LastName="Weather"},
                new Player{Firstname="Theaser",LastName="Metal"},
                new Player{Firstname="Wendel",LastName="Viking"},
                new Player{Firstname="Torn",LastName="God"}
            };

            players.ForEach(p => context.Players.Add(p));
            context.SaveChanges();

            var tournaments = new List<Tournament>
            {
                new Tournament{TournamentID=1050, Title="First_Champ",Country="Russland"},
                new Tournament{TournamentID=2010, Title="Second_Champ",Country="Germany"},
                new Tournament{TournamentID=3080, Title="Third_Champ",Country="Poland"},
                new Tournament{TournamentID=4000, Title="Jumbo_Champ",Country="Spain"},
                new Tournament{TournamentID=5090, Title="Crist_Champ",Country="Portland"},
                new Tournament{TournamentID=6050, Title="Colding_Champ",Country="Great Britain"},
                new Tournament{TournamentID=7050, Title="Pirst_Champ",Country="Turkey"}
            };
            tournaments.ForEach(p => context.Tournaments.Add(p));
            context.SaveChanges();
            var enrollments = new List<Enrollment>
            {
                new Enrollment{ PlayerID=1, TournamentID=1},
                new Enrollment{ PlayerID=1, TournamentID=2},
                new Enrollment{ PlayerID=1, TournamentID=3},
                new Enrollment{ PlayerID=2, TournamentID=4},
                new Enrollment{ PlayerID=2, TournamentID=6},
                new Enrollment{ PlayerID=3, TournamentID=1},
                new Enrollment{ PlayerID=3, TournamentID=7},
                new Enrollment{ PlayerID=4, TournamentID=1},
                new Enrollment{ PlayerID=5, TournamentID=3},
                new Enrollment{ PlayerID=6, TournamentID=5},
                new Enrollment{ PlayerID=7, TournamentID=1}
            };
            enrollments.ForEach(s => context.Enrollments.Add(s));
            context.SaveChanges();
        }
    }
}