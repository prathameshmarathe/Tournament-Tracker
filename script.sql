use tracker;
create table Tournaments (
id int not null AUTO_INCREMENT,
TournamentName varchar(40),
EntryFee decimal(10,0),
PRIMARY KEY(id)
);


create table Teams (
id int not null AUTO_INCREMENT,
TeamName varchar(40),
PRIMARY KEY(id)
);

create table Prizes (
id int not null AUTO_INCREMENT,
PlaceNumber int,
PlaceName varchar(40),
PrizeAmount decimal (10,0),
PricePercentage decimal (3,0),
PRIMARY KEY(id)
);

create table TournamentPrizes(
id int not null AUTO_INCREMENT,
TournamentId int,
PrizeId int,
PRIMARY KEY(id),
FOREIGN KEY(TournamentId) REFERENCES Tournaments(id),
FOREIGN KEY(PrizeId) REFERENCES Prizes(id)
);

create table Person(
id int not null AUTO_INCREMENT,
FirstName varchar(40),
LastName varchar(40),
EmailAddress varchar(40),
CellphoneNumb varchar (40),
PRIMARY KEY(id)
);


create table TeamMembers(
id int not null AUTO_INCREMENT,
TeamId int,
PersonId int,
PRIMARY KEY(id),
FOREIGN KEY(TeamId) REFERENCES Teams(id),
FOREIGN KEY(PersonId) REFERENCES Person(id)
);

create table Matchups(
id int not null AUTO_INCREMENT,
WinnerId int,
MatchupRound int,
PRIMARY KEY(id),
FOREIGN KEY(WinnerId) REFERENCES Teams(id)
);

create table MatchupEntries(
id int not null AUTO_INCREMENT,
MatchupId int,
ParentMatchupId int,
TeamCompetingId int,
Score int,
PRIMARY KEY(id),
FOREIGN KEY(MatchupId) REFERENCES Matchups(id),
FOREIGN KEY(ParentMatchupId) REFERENCES Matchups(id),
FOREIGN KEY(TeamCompetingId) REFERENCES Teams(id)
);


create table TournamentEntries(
id int not null AUTO_INCREMENT,
TournamentId int,
TeamId int,
PRIMARY KEY(id),
FOREIGN KEY(TournamentId) REFERENCES Tournaments(id),
FOREIGN KEY(TeamId) REFERENCES Teams(id)
);


