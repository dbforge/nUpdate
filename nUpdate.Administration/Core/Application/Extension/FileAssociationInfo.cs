// Copyright © Dominic Beger 2018

using System;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace nUpdate.Administration.Core.Application.Extension
{
    #region Public Enums

    /// <summary>
    ///     Broad categories of system recognized file format types.
    /// </summary>
    public enum PerceivedTypes
    {
        /// <summary>
        ///     No
        /// </summary>
        None,

        /// <summary>
        ///     Image file
        /// </summary>
        Image,

        /// <summary>
        ///     Text file
        /// </summary>
        Text,

        /// <summary>
        ///     Audio file
        /// </summary>
        Audio,

        /// <summary>
        ///     Video file
        /// </summary>
        Video,

        /// <summary>
        ///     Compressed file
        /// </summary>
        Compressed,

        /// <summary>
        ///     System file
        /// </summary>
        System
    }

    #endregion

    /// <summary>
    ///     Provides instance methods for the creation, modification, and deletion of file extension associations in the
    ///     Windows registry.
    /// </summary>
    public class FileAssociationInfo
    {
        private readonly RegistryWrapper _registryWrapper = new RegistryWrapper();

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileAssociationInfo" />FileAssociationInfo class, which acts as a
        ///     wrapper for a file extension within the registry.
        /// </summary>
        /// <param name="extension">The dot prefixed extension.</param>
        /// <example>
        ///     FileAssociationInfo(".mp3")
        ///     FileAssociationInfo(".txt")
        ///     FileAssociationInfo(".doc")
        /// </example>
        public FileAssociationInfo(string extension)
        {
            Extension = extension;
        }

        /// <summary>
        ///     Gets or sets a value that determines the MIME type of the file.
        /// </summary>
        public string ContentType
        {
            get => GetContentType(this);
            set => SetContentType(this, value);
        }

        /// <summary>
        ///     Gets a value indicating whether the extension exists.
        /// </summary>
        public bool Exists
        {
            get
            {
                var root = Registry.ClassesRoot;
                try
                {
                    var key = root.OpenSubKey(Extension);

                    if (key == null)
                        return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        ///     Gets the name of the extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        ///     Gets or sets array of containing program file names which should be displayed in the Open With List.
        /// </summary>
        /// <example>notepad.exe, wordpad.exe, othertexteditor.exe</example>
        public string[] OpenWithList
        {
            get => GetOpenWithList(this);
            set => SetOpenWithList(this, value);
        }

        /// <summary>
        ///     Gets or sets a value that determines the <see cref="PerceivedType" /> of the file.
        /// </summary>
        public PerceivedTypes PerceivedType
        {
            get => GetPerceivedType(this);
            set => SetPerceivedType(this, value);
        }

        /// <summary>
        ///     Gets or sets a value that indicates the filter component that is used to search for text within documents of this
        ///     type.
        /// </summary>
        public Guid PersistentHandler
        {
            get => GetPersistentHandler(this);
            set => SetPersistentHandler(this, value);
        }

        /// <summary>
        ///     Gets or set a value that indicates the name of the associated application with the behavior to handle this
        ///     extension.
        /// </summary>
        [XmlAttribute]
        public string ProgId
        {
            get => GetProgId(this);
            set => SetProgId(this, value);
        }

        /// <summary>
        ///     Creates the extension key.
        /// </summary>
        public void Create()
        {
            Create(this);
        }

        /// <summary>
        ///     Creates actual file extension entry in registry.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> instance that contains specifics on extension to be created.</param>
        protected void Create(FileAssociationInfo file)
        {
            if (file.Exists)
                file.Delete();

            var root = Registry.ClassesRoot;
            root.CreateSubKey(file.Extension);
            var subKey = root.CreateSubKey("nUpdate Administration\\shell\\open\\command");
            if (subKey != null)
                subKey.SetValue("", $"{System.Windows.Forms.Application.ExecutablePath} \"%1\" ",
                    RegistryValueKind.String);
        }

        /// <summary>
        ///     Deletes the extension key.
        /// </summary>
        public void Delete()
        {
            Delete(this);
        }

        /// <summary>
        ///     Deletes actual file extension entry in registry.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> instance that contains specifics on extension to be deleted.</param>
        protected void Delete(FileAssociationInfo file)
        {
            if (!file.Exists)
                throw new Exception("Key not found.");

            var root = Registry.ClassesRoot;
            root.DeleteSubKeyTree(file.Extension);
        }

        /// <summary>
        ///     Gets array containing known file extensions from HKEY_CLASSES_ROOT.
        /// </summary>
        /// <returns>String array containing extensions.</returns>
        public static string[] GetExtensions()
        {
            var root = Registry.ClassesRoot;
            var subKeys = root.GetSubKeyNames();
            return subKeys.Where(subKey => subKey.StartsWith(".")).ToArray();
        }

        /// <summary>
        ///     Verifies that given extension exists and is associated with given program id
        /// </summary>
        /// <param name="extension">Extension to be checked for.</param>
        /// <param name="progId">progId to be checked for.</param>
        /// <returns>True if association exists, false if it does not.</returns>
        public bool IsValid(string extension, string progId)
        {
            var fai = new FileAssociationInfo(extension);

            if (!fai.Exists)
                return false;

            return progId == fai.ProgId;
        }

        #region Public Functions - Creators

        /// <summary>
        ///     Creates actual extension association key in registry for the specified extension and supplied attributes.
        /// </summary>
        /// <param name="progId">Name of expected handling program.</param>
        /// <returns>FileAssociationInfo instance referring to specified extension.</returns>
        public FileAssociationInfo Create(string progId)
        {
            return Create(progId, PerceivedTypes.None, string.Empty, null);
        }

        /// <summary>
        ///     Creates actual extension association key in registry for the specified extension and supplied attributes.
        /// </summary>
        /// <param name="progId">Name of expected handling program.</param>
        /// <param name="perceivedType"><see cref="PerceivedTypes" />PerceivedType of file type.</param>
        /// <returns>FileAssociationInfo instance referring to specified extension.</returns>
        public FileAssociationInfo Create(string progId, PerceivedTypes perceivedType)
        {
            return Create(progId, perceivedType, string.Empty, null);
        }

        /// <summary>
        ///     Creates actual extension association key in registry for the specified extension and supplied attributes.
        /// </summary>
        /// <param name="progId">Name of expected handling program.</param>
        /// <param name="perceivedType"><see cref="PerceivedTypes" />PerceivedType of file type.</param>
        /// <param name="contentType">MIME type of file type.</param>
        /// <returns>FileAssociationInfo instance referring to specified extension.</returns>
        public FileAssociationInfo Create(string progId, PerceivedTypes perceivedType, string contentType)
        {
            return Create(progId, PerceivedTypes.None, contentType, null);
        }

        /// <summary>
        ///     Creates actual extension association key in registry for the specified extension and supplied attributes.
        /// </summary>
        /// <param name="progId">Name of expected handling program.</param>
        /// <param name="perceivedType"><see cref="PerceivedTypes" />PerceivedType of file type.</param>
        /// <param name="contentType">MIME type of file type.</param>
        /// <param name="openwithList"></param>
        /// <returns>FileAssociationInfo instance referring to specified extension.</returns>
        public FileAssociationInfo Create(string progId, PerceivedTypes perceivedType, string contentType,
            string[] openwithList)
        {
            var fai = new FileAssociationInfo(Extension);

            if (fai.Exists)
                fai.Delete();

            fai.Create();
            fai.ProgId = progId;

            if (perceivedType != PerceivedTypes.None)
                fai.PerceivedType = perceivedType;

            if (contentType != string.Empty)
                fai.ContentType = contentType;

            if (openwithList != null)
                fai.OpenWithList = openwithList;

            return fai;
        }

        #endregion

        #region Private Functions - Property backend

        /// <summary>
        ///     Gets array of containing program file names which should be displayed in the Open With List.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> that provides specifics of the extension to be changed.</param>
        /// <returns>Program file names</returns>
        protected string[] GetOpenWithList(FileAssociationInfo file)
        {
            if (!file.Exists)
                throw new Exception("Extension does not exist");

            var root = Registry.ClassesRoot;
            var key = root.OpenSubKey(file.Extension);

            if (key != null)
            {
                key = key.OpenSubKey("OpenWithList");

                if (key == null)
                    return new string[0];

                return key.GetSubKeyNames();
            }

            return null;
        }

        /// <summary>
        ///     Sets array of containing program file names which should be displayed in the Open With List.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> that provides specifics of the extension to be changed.</param>
        /// <param name="programList">Program file names</param>
        protected void SetOpenWithList(FileAssociationInfo file, string[] programList)
        {
            if (!file.Exists)
                throw new Exception("Extension does not exist");

            var root = Registry.ClassesRoot;

            var key = root.OpenSubKey(file.Extension, true);
            if (key != null)
            {
                var tmpkey = key.OpenSubKey("OpenWithList", true);
                if (tmpkey != null)
                    key.DeleteSubKeyTree("OpenWithList");

                key = key.CreateSubKey("OpenWithList");
                foreach (var s in programList)
                    if (key != null)
                        key.CreateSubKey(s);
            }

            ShellNotification.NotifyOfChange();
        }

        /// <summary>
        ///     Gets or value that determines the <see cref="PerceivedType" />PerceivedType of the file.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> that provides specifics of the extension to be changed.</param>
        /// <returns><see cref="PerceivedTypes" /> that specifies Perceived Type of extension.</returns>
        protected PerceivedTypes GetPerceivedType(FileAssociationInfo file)
        {
            if (!file.Exists)
                throw new Exception("Extension does not exist");

            var val = _registryWrapper.Read(file.Extension, "PerceivedType");
            var actualType = PerceivedTypes.None;

            if (val == null)
                return actualType;

            try
            {
                actualType = (PerceivedTypes) Enum.Parse(typeof(PerceivedTypes), val.ToString(), true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            return actualType;
        }

        /// <summary>
        ///     Sets a value that determines the <see cref="PerceivedType" />PerceivedType of the file.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> that provides specifics of the extension to be changed.</param>
        /// <param name="type"><see cref="PerceivedTypes" /> to be set that specifies Perceived Type of extension.</param>
        protected void SetPerceivedType(FileAssociationInfo file, PerceivedTypes type)
        {
            if (!file.Exists)
                throw new Exception("Extension does not exist");

            _registryWrapper.Write(file.Extension, "PerceivedType", type.ToString());

            ShellNotification.NotifyOfChange();
        }


        /// <summary>
        ///     Gets a value that indicates the filter component that is used to search for text within documents of this type.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> that provides specifics of the extension to be changed.</param>
        /// <returns>Guid of filter component.</returns>
        protected Guid GetPersistentHandler(FileAssociationInfo file)
        {
            if (!file.Exists)
                throw new Exception("Extension does not exist");

            var val = _registryWrapper.Read(file.Extension + "\\PersistentHandler", string.Empty);

            if (val == null)
                return new Guid();
            return new Guid(val.ToString());
        }

        /// <summary>
        ///     Sets a value that indicates the filter component that is used to search for text within documents of this type.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> that provides specifics of the extension to be changed.</param>
        /// <param name="persistentHandler">Guid of filter component.</param>
        protected void SetPersistentHandler(FileAssociationInfo file, Guid persistentHandler)
        {
            if (!file.Exists)
                throw new Exception("Extension does not exist");

            if (persistentHandler == Guid.Empty)
                return;

            _registryWrapper.Write(file.Extension + "\\" + PersistentHandler, string.Empty, persistentHandler);

            ShellNotification.NotifyOfChange();
        }

        /// <summary>
        ///     Gets a value that determines the MIME type of the file.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> that provides specifics of the extension to be changed.</param>
        /// <returns>MIME content type of extension.</returns>
        protected string GetContentType(FileAssociationInfo file)
        {
            if (!file.Exists)
                throw new Exception("Extension does not exist");

            var val = _registryWrapper.Read(file.Extension, "Content Type");

            if (val == null)
                return string.Empty;
            return val.ToString();
        }

        /// <summary>
        ///     Sets a value that determines the MIME type of the file.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> that provides specifics of the extension to be changed.</param>
        /// <param name="type">MIME content type of extension.</param>
        protected void SetContentType(FileAssociationInfo file, string type)
        {
            if (!file.Exists)
                throw new Exception("Extension does not exist");

            _registryWrapper.Write(file.Extension, "Content Type", type);

            ShellNotification.NotifyOfChange();
        }


        /// <summary>
        ///     Gets a value that indicates the name of the associated application with the behavior to handle this extension.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> that provides specifics of the extension to be changed.</param>
        /// <returns>Associated Program ID of handling program.</returns>
        protected string GetProgId(FileAssociationInfo file)
        {
            if (!file.Exists)
                throw new Exception("Extension does not exist");

            var val = _registryWrapper.Read(file.Extension, string.Empty);

            if (val == null)
                return string.Empty;

            return val.ToString();
        }

        /// <summary>
        ///     Set a value that indicates the name of the associated application with the behavior to handle this extension.
        /// </summary>
        /// <param name="file"><see cref="FileAssociationInfo" /> that provides specifics of the extension to be changed.</param>
        /// <param name="progId">Associated Program ID of handling program.</param>
        protected void SetProgId(FileAssociationInfo file, string progId)
        {
            if (!file.Exists)
                throw new Exception("Extension does not exist");

            _registryWrapper.Write(file.Extension, string.Empty, progId);

            ShellNotification.NotifyOfChange();
        }

        #endregion
    }
}