using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Torii.Util;
using UnityEngine;

namespace Torii.Console
{
    public static class DevConsole
    {
        /// <summary>
        ///     Matches commands in the format:
        ///     Object.Specifier Arguments
        /// </summary>
        private const string COMMAND_REGEX = @"^(\w*)\.(\w*)(?: +(.*))?$";
        private static readonly Dictionary<string, ObjectInfo> _registered = new Dictionary<string, ObjectInfo>();
        private static readonly Regex compiledCommandRegex = new Regex(COMMAND_REGEX);

        public static void Initialise()
        {
            _registered.Clear();
        }

        public static ExecutionResult Execute(string statement)
        {
            Match matches = compiledCommandRegex.Match(statement);
            if (matches.Groups.Count < 4)
            {
                return new ExecutionResult
                {
                    Failed = true,
                    Error = new ArgumentException("Invalid command syntax - syntax is 'Object.Specifier [Arguments]'")
                };
            }
            ParsedStatement parsedStatement = new ParsedStatement(matches);
            return execute(parsedStatement);
        }

        public static void Register(object obj, string alias = "")
        {
            Type objType = obj.GetType();
            string key = string.IsNullOrEmpty(alias) ? objType.Name : alias;
            if (_registered.ContainsKey(key))
            {
                Debug.LogWarning($"DevConsole already contains object '{key}'");
            }

            _registered[key] = new ObjectInfo(obj);
        }

        public static void Register(Type t, string alias = "")
        {
            string key = string.IsNullOrEmpty(alias) ? t.Name : alias;
            if (_registered.ContainsKey(key))
            {
                Debug.LogWarning($"DevConsole already contains type '{key}'");
            }

            _registered[key] = new ObjectInfo(t);
        }

        public static void Deregister(object obj)
        {
            Deregister(obj.GetType());
        }

        public static void Deregister(Type t)
        {
            _registered.Remove(t.Name);
        }

        public static List<string> Completions(string objFragment)
        {
            if (!objFragment.Contains(value: '.'))
            {
                return _registered.Keys.Where(val => val.StartsWith(objFragment)).ToList();
            }

            string[] split = objFragment.Split('.');
            string objName = split[0];
            string specifierFrag = split[1];

            ObjectInfo obj;
            if (_registered.TryGetValue(objFragment.Split('.')[0], out obj))
            {
                return Completions(objName, specifierFrag);
            }

            return new List<string>();
        }

        public static List<string> Completions(string obj, string specifierFragment)
        {
            IEnumerable<string> fields = _registered[obj].Fields.Keys.Where(val => val.StartsWith(specifierFragment));
            IEnumerable<string> properties =
                _registered[obj].Properties.Keys.Where(val => val.StartsWith(specifierFragment));
            IEnumerable<string> methods =
                _registered[obj].Methods.Keys.Where(val => val.StartsWith(specifierFragment));

            return fields.Concat(properties).Concat(methods).ToList();
        }

        private static ExecutionResult execute(ParsedStatement statement)
        {
            ObjectInfo obj;
            if (!_registered.TryGetValue(statement.Object, out obj))
            {
                return new ExecutionResult
                {
                    Error = new ArgumentException($"Unknown object '{statement.Object}'"),
                    Failed = true
                };
            }

            return applyStatementToObject(statement, obj);
        }

        private static ExecutionResult applyStatementToObject(ParsedStatement statement, ObjectInfo obj)
        {
            FieldInfo field;
            if (obj.Fields.TryGetValue(statement.Specifier, out field))
            {
                return applyStatementToObjectField(statement, obj, field);
            }

            PropertyInfo property;
            if (obj.Properties.TryGetValue(statement.Specifier, out property))
            {
                return applyStatementToObjectProperty(statement, obj, property);
            }

            MethodInfo method;
            if (obj.Methods.TryGetValue(statement.Specifier, out method))
            {
                return applyStatementToObjectMethod(statement, obj, method);
            }

            return new ExecutionResult
            {
                Error = new KeyNotFoundException(
                    $"Could not find member '{statement.Specifier}' on object '{statement.Object}'"),
                Failed = true
            };
        }

        private static ExecutionResult applyStatementToObjectField(ParsedStatement statement, ObjectInfo obj,
            FieldInfo field)
        {
            if (statement.Arguments.Length > 1)
            {
                return new ExecutionResult
                {
                    Error = new ArgumentException($"One argument needed, {statement.Arguments.Length} given"),
                    Failed = true
                };
            }

            if (field.FieldType == typeof(float))
            {
                float value;
                if (!parseArg(statement.Arguments[0], out value))
                {
                    return new ExecutionResult
                    {
                        Error = new FormatException($"Value '{statement.Arguments[0]}' was not a float"),
                        Failed = true
                    };
                }
                field.SetValue(obj.Instance, value);
            }
            else if (field.FieldType == typeof(int))
            {
                int value;
                if (!parseArg(statement.Arguments[0], out value))
                {
                    return new ExecutionResult
                    {
                        Error = new FormatException($"Value '{statement.Arguments[0]}' was not an int"),
                        Failed = true
                    };
                }
                field.SetValue(obj.Instance, value);
            }
            else if (field.FieldType == typeof(bool))
            {
                bool value;
                if (!parseArg(statement.Arguments[0], out value))
                {
                    return new ExecutionResult
                    {
                        Error = new FormatException($"Value '{statement.Arguments[0]}' was not a bool"),
                        Failed = true
                    };
                }
                field.SetValue(obj.Instance, value);
            }
            else if (field.FieldType == typeof(string))
            {
                field.SetValue(obj.Instance, statement.Arguments[0]);
            }

            return new ExecutionResult
            {
                Failed = false,
                Message = $"Set {statement.Object}.{statement.Specifier} to {statement.Arguments[0]}"
            };
        }

        private static ExecutionResult applyStatementToObjectProperty(ParsedStatement statement, ObjectInfo obj,
            PropertyInfo property)
        {
            if (statement.Arguments.Length > 1)
            {
                return new ExecutionResult
                {
                    Error = new ArgumentException($"One argument needed, {statement.Arguments.Length} given"),
                    Failed = true
                };
            }

            if (property.PropertyType == typeof(float))
            {
                float value;
                if (!parseArg(statement.Arguments[0], out value))
                {
                    return new ExecutionResult
                    {
                        Error = new FormatException($"Value '{statement.Arguments[0]}' was not a float"),
                        Failed = true
                    };
                }
                property.SetValue(obj.Instance, value);
            }
            else if (property.PropertyType == typeof(int))
            {
                int value;
                if (!parseArg(statement.Arguments[0], out value))
                {
                    return new ExecutionResult
                    {
                        Error = new FormatException($"Value '{statement.Arguments[0]}' was not an int"),
                        Failed = true
                    };
                }
                property.SetValue(obj.Instance, value);
            }
            else if (property.PropertyType == typeof(bool))
            {
                bool value;
                if (!parseArg(statement.Arguments[0], out value))
                {
                    return new ExecutionResult
                    {
                        Error = new FormatException($"Value '{statement.Arguments[0]}' was not a bool"),
                        Failed = true
                    };
                }
                property.SetValue(obj.Instance, value);
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(obj.Instance, statement.Arguments[0]);
            }

            return new ExecutionResult
            {
                Failed = false,
                Message = $"Set {statement.Object}.{statement.Specifier} to {statement.Arguments[0]}"
            };
        }

        private static ExecutionResult applyStatementToObjectMethod(ParsedStatement statement, ObjectInfo obj,
            MethodInfo method)
        {
            ParameterInfo[] methodParams = method.GetParameters();
            if (statement.Arguments.Length != methodParams.Length)
            {
                return new ExecutionResult
                {
                    Error = new ArgumentException($"Invalid number of arguments, got: {statement.Arguments.Length}, " +
                                                  $"expected: {method.GetParameters().Length}"),
                    Failed = true
                };
            }

            var args = new List<object>();
            for (int i = 0; i < statement.Arguments.Length; i++)
            {
                ParameterInfo param = methodParams[i];

                if (param.ParameterType == typeof(float))
                {
                    float value;
                    if (!parseArg(statement.Arguments[i], out value))
                    {
                        return new ExecutionResult
                        {
                            Error = new FormatException($"Value '{statement.Arguments[i]}' was not a float"),
                            Failed = true
                        };
                    }
                    args.Add(value);
                }
                else if (param.ParameterType == typeof(int))
                {
                    int value;
                    if (!parseArg(statement.Arguments[i], out value))
                    {
                        return new ExecutionResult
                        {
                            Error = new FormatException($"Value '{statement.Arguments[i]}' was not an int"),
                            Failed = true
                        };
                    }
                    args.Add(value);
                }
                else if (param.ParameterType == typeof(bool))
                {
                    bool value;
                    if (!parseArg(statement.Arguments[i], out value))
                    {
                        return new ExecutionResult
                        {
                            Error = new FormatException($"Value '{statement.Arguments[i]}' was not a bool"),
                            Failed = true
                        };
                    }
                    args.Add(value);
                }
                else if (param.ParameterType == typeof(string))
                {
                    args.Add(statement.Arguments[i]);
                }
            }

            object result = method.Invoke(obj.Instance, args.ToArray());

            return new ExecutionResult
            {
                Failed = false,
                Message = result != null ? result.ToString() : ""
            };
        }

        private static bool parseArg(string arg, out bool result)
        {
            if (!bool.TryParse(arg, out result))
            {
                return false;
            }

            return true;
        }

        private static bool parseArg(string arg, out int result)
        {
            if (!int.TryParse(arg, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return false;
            }

            return true;
        }

        private static bool parseArg(string arg, out float result)
        {
            if (!float.TryParse(arg, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return false;
            }

            return true;
        }


        public struct ExecutionResult
        {
            public Exception Error;
            public bool Failed;
            public string Message;

            public void Log()
            {
                if (Failed)
                {
                    Debug.LogError(Error.Message);
                    return;
                }

                if (!string.IsNullOrEmpty(Message)) Debug.Log(Message);
            }
        }

        private struct ParsedStatement
        {
            public readonly string Object;
            public readonly string Specifier;
            public readonly string[] Arguments;

            public ParsedStatement(Match regexMatch)
            {
                Object = regexMatch.Groups[groupnum: 1].Value;
                Specifier = regexMatch.Groups[groupnum: 2].Value;
                Arguments = regexMatch.Groups[groupnum: 3].Value
                                      .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < Arguments.Length; i++)
                {
                    Arguments[i] = Arguments[i].Trim();
                }
            }
        }

        private class ObjectInfo
        {
            private readonly Type[] ALLOWED_TYPES =
            {
                typeof(string),
                typeof(bool),
                typeof(int),
                typeof(float)
            };
            public readonly Dictionary<string, FieldInfo> Fields;
            public readonly object Instance;
            public readonly Dictionary<string, MethodInfo> Methods;
            public readonly Dictionary<string, PropertyInfo> Properties;

            public ObjectInfo(object obj)
            {
                Methods = makeDictionary(AttributeUtil.GetMethodsWithAttribute<ConsoleAttribute>(
                    obj.GetType()));
                postProcessMethods(Methods);
                Fields = makeDictionary(AttributeUtil.GetFieldsWithAttribute<ConsoleAttribute>(
                    obj.GetType()));
                postProcessFields(Fields);
                Properties = makeDictionary(AttributeUtil.GetPropertiesWithAttribute<ConsoleAttribute>(
                    obj.GetType()));
                postProcessProperties(Properties);
                Instance = obj;
            }

            public ObjectInfo(Type t)
            {
                Methods = makeDictionary(AttributeUtil.GetMethodsWithAttribute<ConsoleAttribute>(t));
                postProcessMethods(Methods);
                Fields = makeDictionary(AttributeUtil.GetFieldsWithAttribute<ConsoleAttribute>(t));
                postProcessFields(Fields);
                Properties = makeDictionary(AttributeUtil.GetPropertiesWithAttribute<ConsoleAttribute>(t));
                postProcessProperties(Properties);
            }

            private Dictionary<string, T> makeDictionary<T>(IEnumerable<T> members) where T : MemberInfo
            {
                var result = new Dictionary<string, T>();
                foreach (T member in members)
                {
                    result[member.Name] = member;
                }

                return result;
            }

            private void postProcessMethods(Dictionary<string, MethodInfo> methods)
            {
                var toPrune = new List<string>();
                foreach (KeyValuePair<string, MethodInfo> method in methods)
                {
                    foreach (ParameterInfo param in method.Value.GetParameters())
                    {
                        if (!typeIsAllowed(param.ParameterType))
                        {
                            toPrune.Add(method.Key);
                            Debug.LogWarning($"Method '{method.Key}' has non-allowed " +
                                             $"param type '{param.ParameterType}'");
                            break;
                        }
                    }
                }

                foreach (string prune in toPrune)
                {
                    methods.Remove(prune);
                }
            }

            private void postProcessFields(Dictionary<string, FieldInfo> fields)
            {
                var toPrune = new List<string>();
                foreach (KeyValuePair<string, FieldInfo> field in fields)
                {
                    if (!typeIsAllowed(field.Value.FieldType))
                    {
                        toPrune.Add(field.Key);
                        Debug.LogWarning($"Field '{field.Key}' has non-allowed type '{field.Value.FieldType}'");
                    }
                }

                foreach (string prune in toPrune)
                {
                    fields.Remove(prune);
                }
            }

            private void postProcessProperties(Dictionary<string, PropertyInfo> properties)
            {
                var toPrune = new List<string>();
                foreach (KeyValuePair<string, PropertyInfo> property in properties)
                {
                    if (!typeIsAllowed(property.Value.PropertyType))
                    {
                        toPrune.Add(property.Key);
                        Debug.LogWarning($"Property '{property.Key}' has non-allowed " +
                                         $"type '{property.Value.PropertyType}'");
                    }
                }

                foreach (string prune in toPrune)
                {
                    properties.Remove(prune);
                }
            }

            private bool typeIsAllowed(Type t)
            {
                return ALLOWED_TYPES.Contains(t);
            }
        }
    }
}
