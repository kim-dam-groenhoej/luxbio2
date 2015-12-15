USE dmaa0914_3Sem_3

CREATE TABLE Reserve
(
ID int IDENTITY(1,1) NOT NULL  PRIMARY KEY,
LatestPickupDate datetime NOT NULL,
UserID int NOT NULL,
SaleID int,
MoviePlayTimeID INT NOT NULL,
Active BIT NOT NULL DEFAULT 0
);

CREATE TABLE CinemaHalls
(
	Number int IDENTITY(1,1) NOT NULL  PRIMARY KEY,
	Title nvarchar(MAX) NOT NULL
);

CREATE TABLE RowTypes
(
	ID int IDENTITY(1,1) NOT NULL  PRIMARY KEY,
	Name nvarChar(MAX) NOT NULL,
	Price Decimal  NOT NULL
);

CREATE TABLE [Rows]
(
	ID int IDENTITY(1,1) NOT NULL  PRIMARY KEY,
	Number int NOT NULL,
	CinemaHallNumber int FOREIGN KEY REFERENCES CinemaHalls(Number),
	RowTypeID int FOREIGN KEY REFERENCES RowTypes(ID)
);

Create TABLE Chairs
(
	ID int IDENTITY(1,1) NOT NULL  PRIMARY KEY,
	Number int NOT NULL,
	RowID int NOT NULL,
	FOREIGN KEY (RowID) REFERENCES [Rows](ID),
);


CREATE TABLE ReserveChairRelations
(
ReserveID int FOREIGN KEY REFERENCES Reserve(ID) NOT NULL,
ChairID int FOREIGN KEY REFERENCES Chairs(ID) NOT NULL,
primary key (ReserveID, ChairID)
);

Create TABLE LockedChairs
(
	ID int IDENTITY(1,1) NOT NULL  PRIMARY KEY,
	ChairID int NOT NULL,
	LockedDateTime datetime NOT NULL,
	UserID int NOT NULL,
	MoviePlayTimeID int NOT NULL,
	FOREIGN KEY (ChairID) REFERENCES [Chairs](ID)
);

INSERT INTO CinemaHalls (Title)
	VALUES ('Cinema1');

INSERT INTO RowTypes (Name,Price)
VALUES ('Sofa',150);

DECLARE @Row INT = 1;
DECLARE @Chair INT = 1;
DECLARE @Chair2 INT = 2;
DECLARE @RowID INT = 0;

WHILE @Row < 6
BEGIN
	INSERT INTO [Rows]  (Number, CinemaHallNumber,RowTypeID) 
		VALUES (@Row, 1,1);

	SET @RowID = SCOPE_IDENTITY();

	WHILE @Chair < 11
	BEGIN
		INSERT INTO Chairs (Number, RowID) VALUES (@Chair, @RowID);

		SET @Chair = @Chair + 2;
END;

	WHILE @Chair2 < 11
	BEGIN
		INSERT INTO Chairs (Number, RowID) VALUES (@Chair2, @RowID);
	
		SET @Chair2 = @Chair2 + 2;
	END;

	SET @Chair = 1;

	SET @Row = @Row + 1;
END;

INSERT INTO Reserve (LatestPickupDate,UserID,SaleID,MoviePlayTimeID,Active)
VALUES (DateAdd(d,1,GETDATE()),1,NULL,1,1);

INSERT INTO Reserve (LatestPickupDate,UserID,SaleID,MoviePlayTimeID,Active)
VALUES (DateAdd(d,1,GETDATE()),1,NULL,1,1);

INSERT INTO ReserveChairRelations (ReserveID, ChairID)
VALUES (1, 1);

INSERT INTO ReserveChairRelations (ReserveID, ChairID)
VALUES (1, 2);

INSERT INTO ReserveChairRelations (ReserveID, ChairID)
VALUES (2, 3);
