CREATE DATABASE `fileuploaddb` /*!40100 DEFAULT CHARACTER SET latin2 */;

CREATE TABLE `uploadfile` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) DEFAULT NULL,
  `FileType` varchar(45) DEFAULT NULL,
  `FileSize` int(11) DEFAULT NULL,
  `TotalRecords` int(11) DEFAULT NULL,
  `CreatedDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=46 DEFAULT CHARSET=utf8;

CREATE TABLE `uploadfiledetail` (
  `Id` int(20) NOT NULL AUTO_INCREMENT,
  `FileId` int(11) DEFAULT NULL,
  `TransactionId` varchar(50) DEFAULT NULL,
  `Amount` decimal(18,2) DEFAULT NULL,
  `Currency` varchar(10) DEFAULT NULL,
  `TransactionDate` datetime DEFAULT NULL,
  `Status` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FileId_idx` (`FileId`),
  CONSTRAINT `FileId` FOREIGN KEY (`FileId`) REFERENCES `uploadfile` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=19103 DEFAULT CHARSET=utf8;
