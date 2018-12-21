using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using ProtoBuf;
using Newtonsoft.Json;
using Torii.Util;
using UnityEngine;
using ProtoBufSerializer = ProtoBuf.Serializer;

namespace Torii.Serialization
{
    public class ToriiSerializer
    {
        private readonly JsonSerializer _json;
        private readonly Dictionary<Type, JsonSerializerSettings> _serializationSettingsTypeMap;

        public ToriiSerializer()
        {
            _json = new JsonSerializer();
            _serializationSettingsTypeMap = new Dictionary<Type, JsonSerializerSettings>();
        }

        public void RegisterJsonSerializationSettings(Type t, JsonSerializerSettings settings)
        {
            _serializationSettingsTypeMap[t] = settings;
        }

        public T JsonDeserialize<T>(string filePath) where T : class { return jsonDeserialize<T>(filePath); }

        public T Deserialize<T>(string filePath) where T : class
        {
            string ext = Path.GetExtension(filePath);

            if (ext != null && ext.Equals(".json"))
            {
                return jsonDeserialize<T>(filePath);
            }
            else
            {
                return protoBufDeserialize<T>(filePath);
            }
        }

        private T jsonDeserialize<T>(string filePath) where T : class
        {
            JsonSerializerSettings settings;
            if (_serializationSettingsTypeMap.TryGetValue(typeof(T), out settings))
            {
                applyJsonSettings(settings);
            }

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
                Debug.LogError($"Deserialization error: Could not deserialize from \"{filePath}\", directory not found or path invalid");
                Debug.LogException(e);
                return null;
            }
            catch (IOException e)
            {
                Debug.LogError($"Deserialization error: Could not deserialize from \"{filePath}\", invalid path syntax");
                Debug.LogException(e);
                return null;
            }
        }

        private T protoBufDeserialize<T>(string filePath) where T : class
        {
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

        private bool jsonSerialize<T>(T obj, string filePath) where T : class
        {
            JsonSerializerSettings settings;
            if (_serializationSettingsTypeMap.TryGetValue(typeof(T), out settings))
            {
                applyJsonSettings(settings);
            }

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

        private bool protoBufSerialize<T>(T obj, string filePath) where T : class
        {
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