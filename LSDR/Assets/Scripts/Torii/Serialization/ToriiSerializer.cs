using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using ProtoBuf;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProtoBuf.Meta;
using Torii.Util;
using UnityEngine;
using ProtoBufSerializer = ProtoBuf.Serializer;

namespace Torii.Serialization
{
    /// <summary>
    /// ToriiSerializer is used to serialize/deserialize data to/from both JSON and protobuf.
    /// </summary>
    public class ToriiSerializer
    {
        private readonly JsonSerializer _json; // reference to JSON serializer
        
        // map types to serializer settings
        private readonly Dictionary<Type, JsonSerializerSettings> _serializationSettingsTypeMap;
        
        static ToriiSerializer()
        {
            // add some protobuf converters for common Unity3D types
            RuntimeTypeModel.Default.Add(typeof(Vector3), true).Add("x").Add("y").Add("z");
            RuntimeTypeModel.Default.Add(typeof(Quaternion), true).Add("x").Add("y").Add("z").Add("w");
            RuntimeTypeModel.Default.Add(typeof(Color), true).Add("r").Add("g").Add("b").Add("a");
        }

        /// <summary>
        /// Create a new Serializer
        /// </summary>
        public ToriiSerializer()
        {
            _json = new JsonSerializer();
            _serializationSettingsTypeMap = new Dictionary<Type, JsonSerializerSettings>();
            
            // add some JSON converters for common Unity3D types
            _json.Converters.Add(new JsonVector3Converter());
            _json.Converters.Add(new JsonQuaternionConverter());
            _json.Converters.Add(new JsonColorConverter());
            _json.Converters.Add(new StringEnumConverter());
        }

        /// <summary>
        /// Register JSON serialization settings for a given type.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <param name="settings">The settings.</param>
        public void RegisterJsonSerializationSettings(Type t, JsonSerializerSettings settings)
        {
            _serializationSettingsTypeMap[t] = settings;
        }

        /// <summary>
        /// Deserialize a JSON file to a given type.
        /// </summary>
        /// <param name="filePath">The path to the JSON file.</param>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <returns>The deserialized data.</returns>
        public T JsonDeserialize<T>(string filePath) where T : class { return jsonDeserialize<T>(filePath); }

        /// <summary>
        /// Deserialize a data file. If file has .json extension, then JSON is deserialized, otherwise protobuf.
        /// </summary>
        /// <param name="filePath">The path to the data.</param>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <returns>The deserialized data.</returns>
        public T Deserialize<T>(string filePath) where T : class
        {
            string ext = Path.GetExtension(filePath);

            // check to see if we should deserialize from JSON or protobuf
            if (ext != null && ext.Equals(".json"))
            {
                return jsonDeserialize<T>(filePath);
            }
            else
            {
                return protoBufDeserialize<T>(filePath);
            }
        }

        // deserialize from JSON
        private T jsonDeserialize<T>(string filePath) where T : class
        {
            // apply settings to the serializer
            JsonSerializerSettings settings;
            if (_serializationSettingsTypeMap.TryGetValue(typeof(T), out settings))
            {
                applyJsonSettings(settings);
            }

            // try to deserialize, and log any errors that occur
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    return _json.Deserialize<T>(reader);
                }
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError($"Deserialization error: Could not deserialize from \"{filePath}\", file not found");
                Debug.LogException(e);
                return null;
            }
            catch (ArgumentException e)
            {
                Debug.LogError($"Deserialization error: Could not deserialize from \"{filePath}\", malformed path");
                Debug.LogException(e);
                return null;
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.LogError(
                    $"Deserialization error: Could not deserialize from \"{filePath}\", directory not found or path invalid");
                Debug.LogException(e);
                return null;
            }
            catch (IOException e)
            {
                Debug.LogError(
                    $"Deserialization error: Could not deserialize from \"{filePath}\", invalid path syntax");
                Debug.LogException(e);
                return null;
            }
            catch (JsonException e)
            {
                Debug.LogError($"Deserialization error: Could not deserialize from \"{filePath}\", JSON was in unexpected format: {e.Message}");
                Debug.LogException(e);
                return null;
            }
        }

        // deserialize from protobuf
        private T protoBufDeserialize<T>(string filePath) where T : class
        {
            // try to deserialize from protobuf, and log any errors that occur
            try
            {
                using (var file = File.OpenRead(filePath))
                {
                    return ProtoBufSerializer.Deserialize<T>(file);
                }
            }
            catch (ArgumentException e)
            {
                Debug.LogError($"Deserialization error: Could not deserialize from \"{filePath}\", malformed path");
                Debug.LogException(e);
                return null;
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.LogError(
                    $"Deserialization error: Could not deserialize from \"{filePath}\", directory not found or path invalid");
                Debug.LogException(e);
                return null;
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.LogError(
                    $"Deserialization error: Could not deserialize from \"{filePath}\", path was directory or caller does not have required permission");
                Debug.LogException(e);
                return null;
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError($"Deserialization error: Could not deserialize from \"{filePath}\", file not found");
                Debug.LogException(e);
                return null;
            }
            catch (NotSupportedException e)
            {
                Debug.LogError(
                    $"Deserialization error: Could not deserialize from \"{filePath}\", path is in invalid format");
                Debug.LogException(e);
                return null;
            }
            catch (IOException e)
            {
                Debug.LogError($"Deserialization error: Could not deserialize from \"{filePath}\", an error occurred opening the file");
                Debug.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// Serialize some data to a file. If the data object has attribute JsonObjectAttribute, then it's serialized
        /// to JSON, and if it has ProtoContractAttribute then it's serialized to protobuf.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="filePath">The file to serialize to.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>True of successfully serialized, false otherwise.</returns>
        public bool Serialize<T>(T obj, string filePath)
            where T : class
        {
            Type tType = typeof(T);
            if (AttributeUtil.HasAttribute<JsonObjectAttribute>(tType))
            {
                return jsonSerialize(obj, filePath);
            }
            else if (AttributeUtil.HasAttribute<ProtoContractAttribute>(tType))
            {
                return protoBufSerialize(obj, filePath);
            }

            return false;
        }

        // serialize an object to JSON
        private bool jsonSerialize<T>(T obj, string filePath) where T : class
        {
            // apply serializer settings
            JsonSerializerSettings settings;
            if (_serializationSettingsTypeMap.TryGetValue(typeof(T), out settings))
            {
                applyJsonSettings(settings);
            }

            // try to serialize, log any errors if they occurred
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    _json.Serialize(writer, obj);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.LogError($"Serialization error: Could not serialize to \"{filePath}\", access denied");
                Debug.LogException(e);
                return false;

            }
            catch (ArgumentException e)
            {
                Debug.LogError($"Serialization error: Could not serialize to \"{filePath}\", malformed path");
                Debug.LogException(e);
                return false;
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.LogError(
                    $"Serialization error: Could not serialize to \"{filePath}\", directory not found or path invalid");
                Debug.LogException(e);
                return false;
            }
            catch (IOException e)
            {
                Debug.LogError($"Serialization error: Could not serialize to \"{filePath}\", invalid path syntax");
                Debug.LogException(e);
                return false;
            }
            catch (SecurityException e)
            {
                Debug.LogError($"Serialization error: Could not serialize to \"{filePath}\", caller has incorrect permission");
                Debug.LogException(e);
                return false;
            }

            return true;

        }

        // serialize an object to protobuf
        private bool protoBufSerialize<T>(T obj, string filePath) where T : class
        {
            // try to serialize, log any errors if they occurred
            try
            {
                using (var file = File.Create(filePath))
                {
                    ProtoBufSerializer.Serialize(file, obj);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.LogError($"Serialization error: Could not serialize to \"{filePath}\", incorrect permission or readonly file");
                Debug.LogException(e);
            }
            catch (ArgumentException e)
            {
                Debug.LogError($"Serialization error: Could not serialize to \"{filePath}\", malformed path");
                Debug.LogException(e);
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.LogError($"Serialization error: Could not serialize to \"{filePath}\", directory not found or path invalid");
                Debug.LogException(e);
            }
            catch (IOException e)
            {
                Debug.LogError($"Serialization error: Could not serialize to \"{filePath}\", error occurred creating the file");
                Debug.LogException(e);

            }
            catch (NotSupportedException e)
            {
                Debug.LogError($"Serialization error: Could not serialize to \"{filePath}\", path is in invalid format");
                Debug.LogException(e);
            }

            return true;
        }

        // apply JSON serializer settings
        private void applyJsonSettings(JsonSerializerSettings settings)
        {
            _json.CheckAdditionalContent = settings.CheckAdditionalContent;
            _json.ConstructorHandling = settings.ConstructorHandling;
            _json.Context = settings.Context;
            _json.ContractResolver = settings.ContractResolver;
            _json.Culture = settings.Culture;
            _json.DateFormatString = settings.DateFormatString;
            _json.DateFormatHandling = settings.DateFormatHandling;
            _json.DateParseHandling = settings.DateParseHandling;
            _json.DateTimeZoneHandling = settings.DateTimeZoneHandling;
            _json.DateParseHandling = settings.DateParseHandling;
            _json.DefaultValueHandling = settings.DefaultValueHandling;
            _json.EqualityComparer = _json.EqualityComparer;
            _json.FloatFormatHandling = settings.FloatFormatHandling;
            _json.FloatParseHandling = settings.FloatParseHandling;
            _json.Formatting = settings.Formatting;
            _json.MaxDepth = settings.MaxDepth;
            _json.MetadataPropertyHandling = settings.MetadataPropertyHandling;
            _json.MissingMemberHandling = settings.MissingMemberHandling;
            _json.NullValueHandling = settings.NullValueHandling;
            _json.ObjectCreationHandling = settings.ObjectCreationHandling;
            _json.PreserveReferencesHandling = settings.PreserveReferencesHandling;
            _json.ReferenceLoopHandling = settings.ReferenceLoopHandling;
            _json.SerializationBinder = settings.SerializationBinder;
            _json.StringEscapeHandling = settings.StringEscapeHandling;
            _json.TraceWriter = settings.TraceWriter;
            _json.TypeNameAssemblyFormatHandling = settings.TypeNameAssemblyFormatHandling;
            _json.TypeNameHandling = settings.TypeNameHandling;
        }

    }
}