using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace nUpdate.Administration
{
    internal static class Program
    {
        /// <summary>
        ///     The root path of nUpdate Administration.
        /// </summary>
        public static string Path =
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nUpdate Administration");

        /// <summary>
        ///     The path of the config file for all projects.
        /// </summary>
        public static string ProjectsConfigFilePath = System.IO.Path.Combine(Path, "projconf.txt");

        /// <summary>
        ///     The SQL-Script for the setup.
        /// </summary>
        public static string SqlSetupScript = @"DROP DATABASE IF EXISTS nUpdate;
CREATE DATABASE IF NOT EXISTS nUpdate;

USE nUpdate;

CREATE TABLE IF NOT EXISTS `nUpdate`.`Application` (
  `ID` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`ID`))
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `nUpdate`.`Version` (
  `ID` INT NOT NULL AUTO_INCREMENT,
  `Version` VARCHAR(40) NOT NULL,
  `Application_ID` INT NOT NULL,
  `PackageFilename` VARCHAR(300) NOT NULL,
  PRIMARY KEY (`ID`),
  INDEX `fk_Version_Application_idx` (`Application_ID` ASC),
  CONSTRAINT `fk_Version_Application`
    FOREIGN KEY (`Application_ID`)
    REFERENCES `nUpdate`.`Application` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

CREATE TABLE IF NOT EXISTS `nUpdate`.`Download` (
  `ID` INT NOT NULL AUTO_INCREMENT,
  `IP` VARCHAR(100) NULL,
  `Version_ID` INT NOT NULL,
  PRIMARY KEY (`ID`),
  INDEX `fk_Download_Version1_idx` (`Version_ID` ASC),
  CONSTRAINT `fk_Download_Version1`
    FOREIGN KEY (`Version_ID`)
    REFERENCES `nUpdate`.`Version` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

ALTER TABLE Download ADD DownloadDate DATETIME;";

        /// <summary>
        ///     The currently existing projects.
        /// </summary>
        public static Dictionary<string, string> ExisitingProjects = new Dictionary<string, string>();

        /// <summary>
        ///     The FTP-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public static SecureString FtpPassword = new SecureString();

        /// <summary>
        ///     The MySQL-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public static SecureString SqlPassword = new SecureString();

        /// <summary>
        ///     The proxy-password. Set as SecureString for deleting it out of the memory after runtime.
        /// </summary>
        public static SecureString ProxyPassword = new SecureString();

        /// <summary>
        ///     The path of the languages directory.
        /// </summary>
        public static string LanguagesDirectory { get; set; }

        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            bool firstInstance = false;
            var mtx = new Mutex(true, "MainForm", out firstInstance);

            if (firstInstance)
            {
                Application.EnableVisualStyles();
                //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainDialog());
            }
        }
    }
}