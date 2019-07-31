CREATE TABLE Employees (EmployeeId INTEGER IDENTITY (1,1) PRIMARY KEY, FirstName VARCHAR(50), LastName VARCHAR(50), UserName VARCHAR(50), Password VARCHAR(50), EmployeeNumber INTEGER, Email VARCHAR(50));
CREATE TABLE Categories (CategoryId INTEGER IDENTITY (1,1) PRIMARY KEY, Name VARCHAR(50));
CREATE TABLE DietPlans(DietPlanId INTEGER IDENTITY (1,1) PRIMARY KEY, Name VARCHAR(50), FoodType VARCHAR(50), FoodAmountInCups INTEGER);
CREATE TABLE Animals (AnimalId INTEGER IDENTITY (1,1) PRIMARY KEY, Name VARCHAR(50), Weight INTEGER, Age INTEGER, Demeanor VARCHAR(50), KidFriendly BIT, PetFriendly BIT, Gender VARCHAR(50), AdoptionStatus VARCHAR(50), CategoryId INTEGER FOREIGN KEY REFERENCES Categories(CategoryId), DietPlanId INTEGER FOREIGN KEY REFERENCES DietPlans(DietPlanId), EmployeeId INTEGER FOREIGN KEY REFERENCES Employees(EmployeeId));
CREATE TABLE Rooms (RoomId INTEGER IDENTITY (1,1) PRIMARY KEY, RoomNumber INTEGER, AnimalId INTEGER FOREIGN KEY REFERENCES Animals(AnimalId));
CREATE TABLE Shots (ShotId INTEGER IDENTITY (1,1) PRIMARY KEY, Name VARCHAR(50));
CREATE TABLE AnimalShots (AnimalId INTEGER FOREIGN KEY REFERENCES Animals(AnimalId), ShotId INTEGER FOREIGN KEY REFERENCES Shots(ShotId), DateReceived DATE, CONSTRAINT AnimalShotId PRIMARY KEY (AnimalId, ShotId));
CREATE TABLE USStates (USStateId INTEGER IDENTITY (1,1) PRIMARY KEY, Name VARCHAR(50), Abbreviation VARCHAR(2));
CREATE TABLE Addresses (AddressId INTEGER IDENTITY (1,1) PRIMARY KEY, AddressLine1 VARCHAR(50), City VARCHAR(50), USStateId INTEGER FOREIGN KEY REFERENCES USStates(USStateId),  Zipcode INTEGER); 
CREATE TABLE Clients (ClientId INTEGER IDENTITY (1,1) PRIMARY KEY, FirstName VARCHAR(50), LastName VARCHAR(50), UserName VARCHAR(50), Password VARCHAR(50), AddressId INTEGER FOREIGN KEY REFERENCES Addresses(AddressId), Email VARCHAR(50));
CREATE TABLE Adoptions(ClientId INTEGER FOREIGN KEY REFERENCES Clients(ClientId), AnimalId INTEGER FOREIGN KEY REFERENCES Animals(AnimalId), ApprovalStatus VARCHAR(50), AdoptionFee INTEGER, PaymentCollected BIT, CONSTRAINT AdoptionId PRIMARY KEY (ClientId, AnimalId));

INSERT INTO Categories VALUES('Sick and Looking');
INSERT INTO Categories VALUES('Healthy and Looking');
INSERT INTO Categories VALUES('Healthy and Adopted');
INSERT INTO Categories VALUES('Newly Sheltered');
INSERT INTO Categories VALUES('Elderly');

INSERT INTO DietPlans VALUES('Vegetarian','Pure Vegetables', 5);
INSERT INTO DietPlans VALUES('Carnivore','Pure Meat', 2);
INSERT INTO DietPlans VALUES('Pescatarian','Pure Fish', 4);
INSERT INTO DietPlans VALUES('Keto','No carbohydrates', 3);
INSERT INTO DietPlans VALUES('Vegan','All plant based produce', 6);

INSERT INTO Employees VALUES('John','Smith','JSmith01', 'HeresJohnny', 12345, 'JSmith01@gmail.com');
INSERT INTO Employees VALUES('Martha','May','MarthaMay', 'MayistheWay', 23456, 'MarthaMayWay@gmail.com');
INSERT INTO Employees VALUES('Tim','Kook','TKAllDay', 'CantStopTKNoWay', 23457, 'TKooksNoCrook@gmal.com');
INSERT INTO Employees VALUES('Sarah','Johnson','SJohnson', 'SJ212121', 23458, 'SJAllDay@hotmal.com');
INSERT INTO Employees VALUES('Mike','Fiers','MikeF', 'HikewithMike',10000, 'MikeWithABike@bing.com');

INSERT INTO Animals VALUES ('Eduardo', 120, 6,'Friendly and Loveable',0,1,'Female','Ready To Adopt', 4, 3, 4)
INSERT INTO Animals VALUES ('Jackson', 10, 2,'Guarding',1,1,'Female','Needs Vaccinations', 2, 1, 4)
INSERT INTO Animals VALUES ('Percy', 63, 7,'Young and Energetic',0,0,'Male','Ready To Adopt', 5, 2, 2)
INSERT INTO Animals VALUES ('Copper', 29, 3,'Loud and Controlling',1,1,'Male','Ready To Adopt', 1, 2, 1)
INSERT INTO Animals VALUES ('Mayvis', 18, 1,'Anxious and Skiddish',0,1,'Female','Needs Neutering', 1, 3, 4)

INSERT INTO Rooms VALUES(1,1);
INSERT INTO Rooms VALUES(2,1);
INSERT INTO Rooms VALUES(3,1);
INSERT INTO Rooms VALUES(4,1);
INSERT INTO Rooms VALUES(5,1);
INSERT INTO Rooms VALUES(1,2);
INSERT INTO Rooms VALUES(2,3);
INSERT INTO Rooms VALUES(3,4);
INSERT INTO Rooms VALUES(4,5);
INSERT INTO Rooms VALUES(1,3);

INSERT INTO Shots VALUES('Measles');
INSERT INTO Shots VALUES ('Rabies');
INSERT INTO Shots VALUES ('Anti-Worms');
INSERT INTO Shots VALUES ('Extreme Wisdom');
INSERT INTO shots VALUES ('Extreme stupidity');

INSERT INTO AnimalShots VALUES (1, 2, '12/12/2019');
INSERT INTO AnimalShots VALUES (2, 4, '12/12/2019');
INSERT INTO AnimalShots VALUES (3, 1, '12/12/2019');
INSERT INTO AnimalShots VALUES (4, 5, '12/12/2019');
INSERT INTO AnimalShots VALUES (5, 3, '12/12/2019');

INSERT INTO USStates VALUES('Alabama','AL');
INSERT INTO USStates VALUES('Alaska','AK');
INSERT INTO USStates VALUES('Arizona','AZ');
INSERT INTO USStates VALUES('Arkansas','AR');
INSERT INTO USStates VALUES('California','CA');
INSERT INTO USStates VALUES('Colorado','CO');
INSERT INTO USStates VALUES('Connecticut','CT');
INSERT INTO USStates VALUES('Delaware','DE');
INSERT INTO USStates VALUES('Florida','FL');
INSERT INTO USStates VALUES('Georgia','GA');
INSERT INTO USStates VALUES('Hawaii','HI');
INSERT INTO USStates VALUES('Idaho','ID');
INSERT INTO USStates VALUES('Illinois','IL');
INSERT INTO USStates VALUES('Indiana','IN');
INSERT INTO USStates VALUES('Iowa','IA');
INSERT INTO USStates VALUES('Kansas','KS');
INSERT INTO USStates VALUES('Kentucky','KY');
INSERT INTO USStates VALUES('Louisiana','LA');
INSERT INTO USStates VALUES('Maine','ME');
INSERT INTO USStates VALUES('Maryland','MD');
INSERT INTO USStates VALUES('Massachusetts','MA');
INSERT INTO USStates VALUES('Michigan','MI');
INSERT INTO USStates VALUES('Minnesota','MN');
INSERT INTO USStates VALUES('Mississippi','MS');
INSERT INTO USStates VALUES('Missouri','MO');
INSERT INTO USStates VALUES('Montana','MT');
INSERT INTO USStates VALUES('Nebraska','NE');
INSERT INTO USStates VALUES('Nevada','NV');
INSERT INTO USStates VALUES('New Hampshire','NH');
INSERT INTO USStates VALUES('New Jersey','NJ');
INSERT INTO USStates VALUES('New Mexico','NM');
INSERT INTO USStates VALUES('New York','NY');
INSERT INTO USStates VALUES('North Carolina','NC');
INSERT INTO USStates VALUES('North Dakota','ND');
INSERT INTO USStates VALUES('Ohio','OH');
INSERT INTO USStates VALUES('Oklahoma','OK');
INSERT INTO USStates VALUES('Oregon','OR');
INSERT INTO USStates VALUES('Pennsylvania','PA');
INSERT INTO USStates VALUES('Rhode Island','RI');
INSERT INTO USStates VALUES('South Carolina','SC');
INSERT INTO USStates VALUES('South Dakota','SD');
INSERT INTO USStates VALUES('Tennessee','TN');
INSERT INTO USStates VALUES('Texas','TX');
INSERT INTO USStates VALUES('Utah','UT');
INSERT INTO USStates VALUES('Vermont','VT');
INSERT INTO USStates VALUES('Virginia','VA');
INSERT INTO USStates VALUES('Washington','WA');
INSERT INTO USStates VALUES('West Virgina','WV');
INSERT INTO USStates VALUES('Wisconsin','WI');
INSERT INTO USStates VALUES('Wyoming','WY');

INSERT INTO Addresses VALUES ('1234 Wallaby Way','Muskego',1, 53150);
INSERT INTO Addresses VALUES ('1818 Donald Duck Avenue','Disney',2, 31316);
INSERT INTO Addresses VALUES ('1969 GoToTheMoon Road','Houston',3, 21334);
INSERT INTO Addresses VALUES ('1688 N Franklin Place','Milwaukee',4, 53202);
INSERT INTO Addresses VALUES ('1889 Lombardi Avenue','Green Bay',5, 51001);

INSERT INTO Clients VALUES('Toby','McGuire','TMAC','12345', 1, 'TOBYMACATTACK@gmail.com');
INSERT INTO Clients VALUES('Trisha','McGuire','TRISHMAC','23456', 2, 'TMACATTACK@gmail.com');
INSERT INTO Clients VALUES('Danny','Frawley','Dfrawl1234','BigGuy', 3, 'DFRAWL@gmail.com');
INSERT INTO Clients VALUES('Medusa','McDire','TMedusDonalds','72342', 4, 'ManyBraids@gmail.com');
INSERT INTO Clients VALUES('Salina','Severson','SSquared','RainbowFish', 5, 'SallySev@gmail.com');









