using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using BayatGames.SaveGameFree.Encoders;
using BayatGames.SaveGameFree.Serializers;

using UnityEngine;

namespace BayatGames.SaveGameFree
{

    /// <summary>
    /// Save game path. base paths for your save games.
    /// </summary>
    public enum SavePathType
    {
        /// <summary>
        /// The persistent data path. Application.persistentDataPath
        /// </summary>
        PersistentDataPath,

        /// <summary>
        /// The data path. Application.dataPath
        /// </summary>
        DataPath
    }

    /// <summary>
    /// Save Game.
    /// Use these APIs to Save & Load game data.
    /// If you are looking for Web saving and loading use SaveGameWeb.
    /// </summary>
    public static class SaveGame
    {

        /// <summary>
        /// Save handler.
        /// </summary>
        public delegate void SaveHandler(object obj, string identifier, bool encode, string password,
         ISaveGameSerializer serializer, ISaveGameEncoder encoder, Encoding encoding, SavePathType path);

        /// <summary>
        /// Load handler.
        /// </summary>
        public delegate void LoadHandler(object loadedObj, string identifier, bool encode, string password,
         ISaveGameSerializer serializer, ISaveGameEncoder encoder, Encoding encoding, SavePathType path);

        /// <summary>
        /// Occurs when started saving.
        /// </summary>
        public static event SaveHandler OnSaving;

        /// <summary>
        /// Occurs when on saved.
        /// </summary>
        public static event SaveHandler OnSaved;

        /// <summary>
        /// Occurs when started loading.
        /// </summary>
        public static event LoadHandler OnLoading;

        /// <summary>
        /// Occurs when on loaded.
        /// </summary>
        public static event LoadHandler OnLoaded;

        /// <summary>
        /// The save callback.
        /// </summary>
        public static SaveHandler SaveCallback;

        /// <summary>
        /// The load callback.
        /// </summary>
        public static LoadHandler LoadCallback;

        private static ISaveGameSerializer serializer = new SaveGameJsonSerializer();
        private static ISaveGameEncoder encoder = new SaveGameSimpleEncoder();
        private static Encoding encoding = Encoding.UTF8;
        private static bool doEncode = true;
        private static SavePathType savePath = SavePathType.DataPath;
        private static string encodePassword = "h@e#ll$o%^";
        private static bool doLogError = false;
        private static bool doUsePlayerPrefs = false;
        private static List<string> ignoredFiles = new List<string>()
        {
            "Player.log",
            "output_log.txt"
        };
        private static List<string> ignoredDirectories = new List<string>()
        {
            "Analytics"
        };

        /// <summary>
        /// Gets or sets the serializer.
        /// </summary>
        /// <value>The serializer.</value>
        public static ISaveGameSerializer Serializer
        {
            get
            {
                if (serializer == null)
                {
                    serializer = GetDefaultSaveGameSerializer();
                }
                return serializer;
            }
            set
            {
                serializer = value;
            }
        }

        private static ISaveGameSerializer GetDefaultSaveGameSerializer() => new SaveGameJsonSerializer();

        /// <summary>
        /// Gets or sets the encoder.
        /// </summary>
        /// <value>The encoder.</value>
        public static ISaveGameEncoder Encoder
        {
            get
            {
                if (encoder == null)
                {
                    encoder = new SaveGameSimpleEncoder();
                }
                return encoder;
            }
            set
            {
                encoder = value;
            }
        }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public static Encoding DefaultEncoding
        {
            get
            {
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }
                return encoding;
            }
            set
            {
                encoding = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SaveGameFree.SaveGame"/> encrypt data by default.
        /// </summary>
        /// <value><c>true</c> if encode; otherwise, <c>false</c>.</value>
        public static bool DoEncode
        {
            get
            {
                return doEncode;
            }
            set
            {
                doEncode = value;
            }
        }

        private static readonly string pathDirectoryIdentifier = "Saved";

        /// <summary>
        /// Gets or sets the save path.
        /// </summary>
        /// <value>The save path.</value>
        public static SavePathType SavePath
        {
            get
            {
                return savePath;
            }
            set
            {
                savePath = value;
            }
        }

        /// <summary>
        /// Gets or sets the encryption password.
        /// </summary>
        /// <value>The encryption password.</value>
        public static string EncodePassword
        {
            get
            {
                return encodePassword;
            }
            set
            {
                encodePassword = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SaveGameFree.SaveGame"/> log error.
        /// </summary>
        /// <value><c>true</c> if log error; otherwise, <c>false</c>.</value>
        public static bool LogError
        {
            get => doLogError;
            set => doLogError = value;
        }

        /// <summary>
        /// Gets or sets whether to use PlayerPrefs as storage or not.
        /// </summary>
        public static bool DoUsePlayerPrefs
        {
            get => doUsePlayerPrefs;
            set => doUsePlayerPrefs = value;
        }

        /// <summary>
        /// Gets the list of ignored files.
        /// </summary>
        public static List<string> IgnoredFiles => ignoredFiles;

        /// <summary>
        /// Gets the list of ignored directories.
        /// </summary>
        public static List<string> IgnoredDirectories => ignoredDirectories;

        /// <summary>
        /// Saves data using the identifier.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="obj">Object to save.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Save<T>(string identifier, T obj)
        {
            Save<T>(identifier, obj, DoEncode, EncodePassword, Serializer, Encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Save the specified identifier, obj, encode and encodePassword.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="obj">Object.</param>
        /// <param name="encode">If set to <c>true</c> encode.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Save<T>(string identifier, T obj, bool encode)
        {
            Save<T>(identifier, obj, encode, EncodePassword, Serializer, Encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Save the specified identifier, obj and encodePassword.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="obj">Object.</param>
        /// <param name="encodePassword">Encode password.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Save<T>(string identifier, T obj, string encodePassword)
        {
            Save<T>(identifier, obj, DoEncode, encodePassword, Serializer, Encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Save the specified identifier, obj and serializer.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="obj">Object.</param>
        /// <param name="serializer">Serializer.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Save<T>(string identifier, T obj, ISaveGameSerializer serializer)
        {
            Save<T>(identifier, obj, DoEncode, EncodePassword, serializer, Encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Save the specified identifier, obj and encoder.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="obj">Object.</param>
        /// <param name="encoder">Encoder.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Save<T>(string identifier, T obj, ISaveGameEncoder encoder)
        {
            Save<T>(identifier, obj, DoEncode, EncodePassword, Serializer, encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Save the specified identifier, obj and encoding.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="obj">Object.</param>
        /// <param name="encoding">Encoding.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Save<T>(string identifier, T obj, Encoding encoding)
        {
            Save<T>(identifier, obj, DoEncode, EncodePassword, Serializer, Encoder, encoding, SavePath);
        }

        /// <summary>
        /// Save the specified identifier, obj and savePath.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="obj">Object.</param>
        /// <param name="savePath">Save path.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Save<T>(string identifier, T obj, SavePathType savePath)
        {
            Save<T>(identifier, obj, DoEncode, EncodePassword, Serializer, Encoder, DefaultEncoding, savePath);
        }

        /// <summary>
        /// Saves data using the identifier.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="obj">Object to save.</param>
        /// <param name="encode">Encrypt the data?</param>
        /// <param name="password">Encryption Password.</param>
        /// <param name="serializer">Serializer.</param>
        /// <param name="encoder">Encoder.</param>
        /// <param name="encoding">Encoding.</param>
        /// <param name="path">Path.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Save<T>(string identifier, T obj, bool encode, string password,
        ISaveGameSerializer serializer, ISaveGameEncoder encoder, Encoding encoding, SavePathType path)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new System.ArgumentNullException("identifier");
            }
            if (OnSaving != null)
            {
                OnSaving(
                    obj,
                    identifier,
                    encode,
                    password,
                    serializer,
                    encoder,
                    encoding,
                    path);
            }

            serializer = serializer ?? SaveGame.Serializer;
            encoding = encoding ?? SaveGame.DefaultEncoding;

            string filePath = IsFilePath(identifier) ? identifier : GetPath(path, identifier);

            if (obj == null)
            {
                obj = default(T);
            }

            Stream stream = null;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            if (encode)
            {
                stream = new MemoryStream();
            }
            else
            {
                if (!DoUsePlayerPrefs)
                {
                    stream = File.Create(filePath);
                }
                else
                {
                    stream = new MemoryStream();
                }
            }

            serializer.Serialize(obj, stream, encoding);
            if (encode)
            {
                string data = System.Convert.ToBase64String(((MemoryStream)stream).ToArray());
                string encoded = encoder.Encode(data, password);
                if (!DoUsePlayerPrefs)
                {
                    File.WriteAllText(filePath, encoded, encoding);
                }
                else
                {
                    PlayerPrefs.SetString(filePath, encoded);
                    PlayerPrefs.Save();
                }
            }
            else if (DoUsePlayerPrefs)
            {
                string data = encoding.GetString(((MemoryStream)stream).ToArray());
                PlayerPrefs.SetString(filePath, data);
                PlayerPrefs.Save();
            }
            stream.Dispose();

            if (SaveCallback != null)
            {
                SaveCallback.Invoke(
                    obj,
                    identifier,
                    encode,
                    password,
                    serializer,
                    encoder,
                    encoding,
                    path);
            }
            if (OnSaved != null)
            {
                OnSaved(
                    obj,
                    identifier,
                    encode,
                    password,
                    serializer,
                    encoder,
                    encoding,
                    path);
            }
        }

        /// <summary>
        /// Loads data using identifier.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier)
        {
            return Load<T>(identifier, default(T), DoEncode, EncodePassword, Serializer, Encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Load the specified identifier and defaultValue.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, T defaultValue)
        {
            return Load<T>(identifier, defaultValue, DoEncode, EncodePassword, Serializer, Encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Load the specified identifier and encodePassword.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="encodePassword">Encode password.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, bool encode, string encodePassword)
        {
            return Load<T>(identifier, default(T), encode, encodePassword, Serializer, Encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Load the specified identifier and serializer.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="serializer">Serializer.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, ISaveGameSerializer serializer)
        {
            return Load<T>(identifier, default(T), DoEncode, EncodePassword, serializer, Encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Load the specified identifier and encoder.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="encoder">Encoder.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, ISaveGameEncoder encoder)
        {
            return Load<T>(identifier, default(T), DoEncode, EncodePassword, Serializer, encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Load the specified identifier and encoding.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="encoding">Encoding.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, Encoding encoding)
        {
            return Load<T>(identifier, default(T), DoEncode, EncodePassword, Serializer, Encoder, encoding, SavePath);
        }

        /// <summary>
        /// Load the specified identifier and savePath.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="savePath">Save path.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, SavePathType savePath)
        {
            return Load<T>(identifier, default(T), DoEncode, EncodePassword, Serializer, Encoder, DefaultEncoding, savePath);
        }

        /// <summary>
        /// Load the specified identifier, defaultValue and encode.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="encode">If set to <c>true</c> encode.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, T defaultValue, bool encode)
        {
            return Load<T>(identifier, defaultValue, encode, EncodePassword, Serializer, Encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Load the specified identifier, defaultValue and encodePassword.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="encodePassword">Encode password.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, T defaultValue, string encodePassword)
        {
            return Load<T>(identifier, defaultValue, DoEncode, encodePassword, Serializer, Encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Load the specified identifier, defaultValue and serializer.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="serializer">Serializer.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, T defaultValue, ISaveGameSerializer serializer)
        {
            return Load<T>(identifier, defaultValue, DoEncode, EncodePassword, serializer, Encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Load the specified identifier, defaultValue and encoder.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="encoder">Encoder.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, T defaultValue, ISaveGameEncoder encoder)
        {
            return Load<T>(identifier, defaultValue, DoEncode, EncodePassword, Serializer, encoder, DefaultEncoding, SavePath);
        }

        /// <summary>
        /// Load the specified identifier, defaultValue and encoding.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="encoding">Encoding.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, T defaultValue, Encoding encoding)
        {
            return Load<T>(identifier, defaultValue, DoEncode, EncodePassword, Serializer, Encoder, encoding, SavePath);
        }

        /// <summary>
        /// Load the specified identifier, defaultValue and savePath.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="savePath">Save path.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, T defaultValue, SavePathType savePath)
        {
            return Load<T>(identifier, defaultValue, DoEncode, EncodePassword, Serializer, Encoder, DefaultEncoding, savePath);
        }

        /// <summary>
        /// Loads data using identifier.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="defaultValue">Default Value.</param>
        /// <param name="encode">Load encrypted data? (set it to true if you have used encryption in save)</param>
        /// <param name="password">Encryption Password.</param>
        /// <param name="serializer">Serializer.</param>
        /// <param name="encoder">Encoder.</param>
        /// <param name="encoding">Encoding.</param>
        /// <param name="path">Path.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string identifier, T defaultValue, bool encode, string password,
         ISaveGameSerializer serializer, ISaveGameEncoder encoder, Encoding encoding, SavePathType path)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new System.ArgumentNullException("identifier");
            }
            if (OnLoading != null)
            {
                OnLoading(
                    null,
                    identifier,
                    encode,
                    password,
                    serializer,
                    encoder,
                    encoding,
                    path);
            }
            serializer = serializer ?? SaveGame.Serializer;

            encoding = encoding ?? SaveGame.DefaultEncoding;

            defaultValue = defaultValue ?? default(T);

            T result = defaultValue;
            string filePath = "";
            if (!IsFilePath(identifier))
            {
                filePath = GetPath(path, identifier);
            }
            else
            {
                filePath = identifier;
            }
            if (!Exists(filePath, path))
            {
                Debug.LogWarningFormat(
                    "The specified identifier ({1}) does not exists. please use Exists () to check for existent before calling Load.\n" +
                    "returning the default(T) instance.",
                    filePath,
                    identifier);
                return result;
            }
            Stream stream = null;
            if (encode)
            {
                string data = "";
                if (!DoUsePlayerPrefs)
                {
                    data = File.ReadAllText(filePath, encoding);
                }
                else
                {
                    data = PlayerPrefs.GetString(filePath);
                }
                string decoded = encoder.Decode(data, password);
                stream = new MemoryStream(System.Convert.FromBase64String(decoded), true);
            }
            else
            {
                if (!DoUsePlayerPrefs)
                {
                    stream = File.OpenRead(filePath);
                }
                else
                {
                    string data = PlayerPrefs.GetString(filePath);
                    stream = new MemoryStream(encoding.GetBytes(data));
                }
            }
            result = serializer.Deserialize<T>(stream, encoding);
            stream.Dispose();

            if (result == null)
            {
                result = defaultValue;
            }

            if (LoadCallback != null)
            {
                LoadCallback.Invoke(
                    result,
                    identifier,
                    encode,
                    password,
                    serializer,
                    encoder,
                    encoding,
                    path);
            }
            if (OnLoaded != null)
            {
                OnLoaded(
                    result,
                    identifier,
                    encode,
                    password,
                    serializer,
                    encoder,
                    encoding,
                    path);
            }
            return result;
        }

        /// <summary>
        /// Checks whether the specified identifier exists or not.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        public static bool Exists(string identifier)
        {
            return Exists(identifier, SavePath);
        }

        /// <summary>
        /// Checks whether the specified identifier exists or not.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="path">Path.</param>
        /// <param name="web">Check in Web?</param>
        /// <param name="webUsername">Web username.</param>
        /// <param name="webPassword">Web password.</param>
        /// <param name="webURL">Web URL.</param>
        public static bool Exists(string identifier, SavePathType path)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new System.ArgumentNullException("identifier");
            }


            string filePath = IsFilePath(identifier) ? identifier : GetPath(path, identifier);

            if (!DoUsePlayerPrefs)
            {
                bool exists = Directory.Exists(filePath);
                if (!exists)
                {
                    exists = File.Exists(filePath);
                }
                return exists;
            }
            else
            {
                return PlayerPrefs.HasKey(filePath);
            }
        }

        /// <summary>
        /// Delete the specified identifier.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        public static void Delete(string identifier)
        {
            Delete(identifier, SavePath);
        }

        private static string GetDirectory(SavePathType path)
        {
            switch (path)
            {
                case SavePathType.PersistentDataPath:
                    return Path.Combine(Application.persistentDataPath, pathDirectoryIdentifier);

                case SavePathType.DataPath:
                    return Path.Combine(Application.dataPath, pathDirectoryIdentifier);
            }
            return null;
        }

        private static string GetPath(SavePathType path, string identifier)
        {
            switch (path)
            {
                case SavePathType.PersistentDataPath:
                    return Path.Combine(Application.persistentDataPath, pathDirectoryIdentifier, identifier);

                case SavePathType.DataPath:
                    return Path.Combine(Application.dataPath, pathDirectoryIdentifier, identifier);

            }
            return null;
        }

        /// <summary>
        /// Delete the specified identifier and path.
        /// </summary>
        /// <param name="identifier">Identifier.</param>
        /// <param name="path">Path.</param>
        public static void Delete(string identifier, SavePathType path)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new System.ArgumentNullException("identifier");
            }

            string filePath = IsFilePath(identifier) ? identifier : GetPath(path, identifier);

            if (!Exists(filePath, path))
            {
                //throw new System.ArgumentException("Delte path doesn't exist!");
                return;
            }

            if (!DoUsePlayerPrefs)
            {
                var fileName = Path.GetFileName(filePath);
                if (ignoredFiles.Contains(fileName) || ignoredDirectories.Contains(fileName))
                {
                    return;
                }
                if (File.Exists(filePath))
                    File.Delete(filePath);
                else if (Directory.Exists(filePath))
                    Directory.Delete(filePath, true);
            }
            else
            {
                PlayerPrefs.DeleteKey(filePath);
            }
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        public static void DeleteAll()
        {
            DeleteAll(SavePath);
        }

        /// <summary>
        /// Deletes all.
        /// </summary>
        /// <param name="path">Path.</param>
        public static void DeleteAll(SavePathType path)
        {
            string dirPath = GetDirectory(path);

            if (!DoUsePlayerPrefs)
            {
                DeleteNonPlayerPrefs(dirPath);
            }
            else
            {
                PlayerPrefs.DeleteAll();
            }
        }

        private static void DeleteNonPlayerPrefs(string dirPath)
        {
            DirectoryInfo info = new DirectoryInfo(dirPath);
            FileInfo[] files = info.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                if (ignoredFiles.Contains(files[i].Name))
                {
                    continue;
                }
                // files[i].Delete();
                Debug.Log(files[i].Name);
            }

            DirectoryInfo[] dirs = info.GetDirectories();

            for (int i = 0; i < dirs.Length; i++)
            {
                if (ignoredDirectories.Contains(dirs[i].Name))
                {
                    continue;
                }
                Debug.Log(dirs[i].Name);

                // dirs[i].Delete(true);
            }
        }

        /// <summary>
        /// Retrieves files from the save path home.
        /// </summary>
        /// <returns></returns>
        public static FileInfo[] GetFiles() => GetFiles(string.Empty, SavePath);

        /// <summary>
        /// Retrieves files from the given directory path.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static FileInfo[] GetFiles(string identifier) => GetFiles(identifier, SavePath);

        /// <summary>
        /// Retrieves files from the given directory path.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FileInfo[] GetFiles(string identifier, SavePathType path)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = string.Empty;
            }

            string filePath = IsFilePath(identifier) ? identifier : GetPath(path, identifier);

            FileInfo[] files = new FileInfo[0];
            if (!Exists(filePath, path))
            {
                return files;
            }
            if (Directory.Exists(filePath))
            {
                DirectoryInfo info = new DirectoryInfo(filePath);
                files = info.GetFiles();
            }
            return files;
        }

        /// <summary>
        /// Retrieves directories from the save path home.
        /// </summary>
        /// <returns></returns>
        public static DirectoryInfo[] GetDirectories()
        {
            return GetDirectories(string.Empty, SavePath);
        }

        /// <summary>
        /// Retrieves directories from the given directory path.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static DirectoryInfo[] GetDirectories(string identifier)
        {
            return GetDirectories(identifier, SavePath);
        }

        /// <summary>
        /// Retrieves directories from the given directory path.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="path"></param>
        /// <returns></returns>

        public static DirectoryInfo[] GetDirectories(string identifier, SavePathType path)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = string.Empty;
            }

            string filePath = "";
            if (!IsFilePath(identifier))
            {
                filePath = GetPath(path, identifier);
            }
            else
            {
                filePath = identifier;
            }

            DirectoryInfo[] directories = new DirectoryInfo[0];
            if (!Exists(filePath, path))
            {
                return directories;
            }
            if (Directory.Exists(filePath))
            {
                DirectoryInfo info = new DirectoryInfo(filePath);
                directories = info.GetDirectories();
            }
            return directories;
        }

        /// <summary>
        /// Determines if the string is file path.
        /// </summary>
        /// <returns><c>true</c> if is file path the specified str; otherwise, <c>false</c>.</returns>
        /// <param name="str">String.</param>
        public static bool IsFilePath(string str)
        {
            bool result = false;
#if !UNITY_SAMSUNGTV && !UNITY_TVOS && !UNITY_WEBGL
            if (Path.IsPathRooted(str))
            {
                try
                {
                    Path.GetFullPath(str);
                    result = true;
                }
                catch (System.Exception)
                {
                    result = false;
                }
            }
#endif
            return result;
        }

    }

}
