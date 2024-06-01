using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LSDR.Dream;
using LSDR.Game;
using LSDR.InputManagement;
using LSDR.SDK.Lua;
using LSDR.SDK.Lua.Actions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using UnityEngine;

namespace LSDR.Lua
{
    public class LuaEngine : ILuaEngine
    {
        protected readonly Dictionary<string, object> _registeredObjects;
        protected readonly SettingsSystem _settingsSystem;

        public Persistence.Persistence Persistence { get; }

        public LuaEngine(DreamSystem dreamSystem, SettingsSystem settingsSystem,
            ControlSchemeLoaderSystem controlSchemeLoaderSystem)
        {
            _registeredObjects = new Dictionary<string, object>();

            _settingsSystem = settingsSystem;

            // register types
            UserData.RegisterAssembly();
            UserData.RegisterType<LuaAsyncActionRunner>();
            UserData.RegisterType<LuaAsyncAction>();

            Persistence = new(dreamSystem.GameSave);

            _registeredObjects["DreamSystem"] = dreamSystem;
            _registeredObjects["Input"] = controlSchemeLoaderSystem;
            _registeredObjects["PauseSystem"] = dreamSystem.PauseSystem;
            _registeredObjects["Persistence"] = Persistence;

            // create converters
            createConverters();
        }

        public Script CreateBaseAPI()
        {
            Script script = new Script(CoreModules.Preset_SoftSandbox | CoreModules.LoadMethods)
            {
                Options =
                {
                    ScriptLoader = new JournalScriptLoader(_settingsSystem.CurrentJournal),
                    DebugPrint = Debug.Log,
                    UseLuaErrorLocations = false,
                }
            };

            createNamespacedStaticAPI(new UnityAPI(), script, "Unity");
            createNamespacedStaticAPI(new DebugAPI(), script, "Debug");
            createStaticAPI(new LSDAPI(), script);
            createNamespacedStaticAPI(new ActionPredicates(), script, "Condition");
            createNamespacedStaticAPI(new ColorAPI(), script, "Colors");
            createNamespacedStaticAPI(new RandomAPI(), script, "Random");

            createRegisteredGlobalObjects(script);

            return script;
        }

        public void RegisterEnum<T>() where T : Enum
        {
            UserData.RegisterType<T>();
            DynValue userData = UserData.CreateStatic(typeof(T));
            RegisterGlobalObject(userData, typeof(T).Name);
        }

        public void RegisterGlobalObject(object obj, string alias = "")
        {
            string objName = string.IsNullOrEmpty(alias)
                ? obj.GetType().Name
                : alias;
            _registeredObjects[objName] = obj;
        }

        private void createRegisteredGlobalObjects(Script script)
        {
            foreach (KeyValuePair<string, object> registeredObj in _registeredObjects)
            {
                string objName = registeredObj.Key;
                object obj = registeredObj.Value;
                script.Globals[objName] = obj;
            }
        }

        private void createConverters()
        {
            // Action
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Function, typeof(Action),
                value =>
                {
                    Closure func = value.Function;
                    return new Action(() => func.Call());
                }
            );

            // Func<bool>
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Function,
                typeof(Func<bool>),
                value =>
                {
                    Closure func = value.Function;
                    return new Func<bool>(() => func.Call().Boolean);
                }
            );

            // Vector2
            Vector2 vec2FromTable(Table table)
            {
                return new Vector2((float)table.Get("x").Number, (float)table.Get("y").Number);
            }

            Table makeVec2Table(Vector2 vector, Script script)
            {
                DynValue x = DynValue.NewNumber(vector.x);
                DynValue y = DynValue.NewNumber(vector.y);
                Table vec = new Table(script)
                {
                    ["x"] = x,
                    ["y"] = y,
                    MetaTable = DynValue.FromObject(script, new Dictionary<string, object>
                    {
                        {
                            "__tostring", new Func<Table, string>(t => $"({x}, {y})")
                        },
                        {
                            "__add", new Func<Table, Table, Table>((a, b) =>
                            {
                                var aVec = vec2FromTable(a);
                                var bVec = vec2FromTable(b);
                                return makeVec2Table(aVec + bVec, script);
                            })
                        },
                        {
                            "__sub", new Func<Table, Table, Table>((a, b) =>
                            {
                                var aVec = vec2FromTable(a);
                                var bVec = vec2FromTable(b);
                                return makeVec2Table(aVec - bVec, script);
                            })
                        },
                        {
                            "__mul", new Func<Table, double, Table>((v, s) =>
                            {
                                var vec = vec2FromTable(v);
                                return makeVec2Table(vec * (float)s, script);
                            })
                        },
                        {
                            "__div", new Func<Table, double, Table>((v, s) =>
                            {
                                var vec = vec2FromTable(v);
                                return makeVec2Table(vec / (float)s, script);
                            })
                        },
                    }).Table
                };

                vec["normalise"] = new CallbackFunction((context, args) =>
                {
                    var vec2 = vec2FromTable(vec);
                    vec2.Normalize();
                    return DynValue.NewTable(makeVec2Table(vec2, script));
                });
                vec["length"] = new CallbackFunction((context, args) =>
                {
                    var vec2 = vec2FromTable(vec);
                    return DynValue.NewNumber(vec2.magnitude);
                });

                return vec;
            }

            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Vector2),
                dynVal => vec2FromTable(dynVal.Table));
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Vector2>(
                (script, vector) => DynValue.NewTable(makeVec2Table(vector, script)));

            // Vector3
            Vector3 vec3FromTable(Table table)
            {
                return new Vector3((float)table.Get("x").Number, (float)table.Get("y").Number,
                    (float)table.Get("z").Number);
            }

            Table makeVec3Table(Vector3 vector, Script script)
            {
                DynValue x = DynValue.NewNumber(vector.x);
                DynValue y = DynValue.NewNumber(vector.y);
                DynValue z = DynValue.NewNumber(vector.z);
                Table vec = new Table(script)
                {
                    ["x"] = x,
                    ["y"] = y,
                    ["z"] = z,
                    MetaTable = DynValue.FromObject(script, new Dictionary<string, object>
                    {
                        {
                            "__tostring", new Func<Table, string>(t => $"({x}, {y}, {z})")
                        },
                        {
                            "__add", new Func<Table, Table, Table>((a, b) =>
                            {
                                var aVec = vec3FromTable(a);
                                var bVec = vec3FromTable(b);
                                return makeVec3Table(aVec + bVec, script);
                            })
                        },
                        {
                            "__sub", new Func<Table, Table, Table>((a, b) =>
                            {
                                var aVec = vec3FromTable(a);
                                var bVec = vec3FromTable(b);
                                return makeVec3Table(aVec - bVec, script);
                            })
                        },
                        {
                            "__mul", new Func<Table, double, Table>((v, s) =>
                            {
                                var vec = vec3FromTable(v);
                                return makeVec3Table(vec * (float)s, script);
                            })
                        },
                        {
                            "__div", new Func<Table, double, Table>((v, s) =>
                            {
                                var vec = vec3FromTable(v);
                                return makeVec3Table(vec / (float)s, script);
                            })
                        },
                    }).Table
                };

                vec["normalise"] = new CallbackFunction((context, args) =>
                {
                    var vec3 = vec3FromTable(vec);
                    vec3.Normalize();
                    return DynValue.NewTable(makeVec3Table(vec3, script));
                });
                vec["length"] = new CallbackFunction((context, args) =>
                {
                    var vec3 = vec3FromTable(vec);
                    return DynValue.NewNumber(vec3.magnitude);
                });
                vec["negated"] = new CallbackFunction((context, args) =>
                {
                    var vec3 = vec3FromTable(vec);
                    return DynValue.NewTable(makeVec3Table(-vec3, script));
                });
                vec["dot"] = new CallbackFunction((context, args) =>
                {
                    var a = vec3FromTable(vec);
                    var bTable = args[0].Table;
                    var b = vec3FromTable(bTable);
                    return DynValue.NewNumber(Vector3.Dot(a, b));
                });
                vec["project"] = new CallbackFunction((context, args) =>
                {
                    var a = vec3FromTable(vec);
                    var bTable = args[0].Table;
                    var b = vec3FromTable(bTable);
                    var result = Vector3.Project(a, b);
                    return DynValue.NewTable(makeVec3Table(result, script));
                });

                return vec;
            }

            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Vector3),
                dynVal => vec3FromTable(dynVal.Table));
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Vector3>(
                (script, vector) => DynValue.NewTable(makeVec3Table(vector, script)));

            // color
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Color),
                dynVal =>
                {
                    Table table = dynVal.Table;
                    float r = (float)(double)table[key: "r"];
                    float g = (float)(double)table[key: "g"];
                    float b = (float)(double)table[key: "b"];
                    float a = (float)(double)table[key: "a"];
                    return new Color(r, g, b, a);
                }
            );
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Color?),
                dynVal =>
                {
                    if (dynVal.IsNil()) return null;
                    Table table = dynVal.Table;
                    float r = (float)(double)table[key: "r"];
                    float g = (float)(double)table[key: "g"];
                    float b = (float)(double)table[key: "b"];
                    float a = (float)(double)table[key: "a"];
                    return new Color(r, g, b, a);
                }
            );
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Color>(
                (script, color) =>
                {
                    DynValue r = DynValue.NewNumber(color.r);
                    DynValue g = DynValue.NewNumber(color.g);
                    DynValue b = DynValue.NewNumber(color.b);
                    DynValue a = DynValue.NewNumber(color.a);
                    Table colorTable = new Table(script)
                    {
                        ["r"] = r,
                        ["g"] = g,
                        ["b"] = b,
                        ["a"] = a,
                        MetaTable = DynValue.FromObject(script, new Dictionary<string, object>
                        {
                            {
                                "__tostring", new Func<Table, string>(t => $"(R: {r}, G: {g}, B: {b}, A: {a})")
                            }
                        }).Table
                    };
                    return DynValue.FromObject(script, colorTable);
                }
            );
        }

        private void createStaticAPI<T>(T instance, Script script) where T : ILuaAPI
        {
            instance.Register(this, script);
            foreach (var method in typeof(T).GetMethods(BindingFlags.Static |
                                                        BindingFlags.Public))
            {
                var methodParams = method.GetParameters();
                var delegateType = method.ReturnType == typeof(void)
                    ? Expression.GetActionType(methodParams.Select(p => p.ParameterType).ToArray())
                    : Expression.GetFuncType(methodParams.Select(p => p.ParameterType)
                                                         .Concat(new[] { method.ReturnType }).ToArray());

                var del = method.CreateDelegate(delegateType);
                script.Globals[method.Name] = del;
            }

            foreach (var property in typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                script.Globals[property.Name] = property.GetValue(instance);
            }
        }

        private void createNamespacedStaticAPI<T>(T instance, Script script, string namespace_) where T : ILuaAPI
        {
            instance.Register(this, script);
            var apiTableData = new Dictionary<string, object>();

            foreach (var method in typeof(T).GetMethods(BindingFlags.Static |
                                                        BindingFlags.Public))
            {
                var methodParams = method.GetParameters();
                var delegateType = method.ReturnType == typeof(void)
                    ? Expression.GetActionType(methodParams.Select(p => p.ParameterType).ToArray())
                    : Expression.GetFuncType(methodParams.Select(p => p.ParameterType)
                                                         .Concat(new[] { method.ReturnType }).ToArray());

                var del = method.CreateDelegate(delegateType);
                apiTableData[method.Name] = del;
            }

            foreach (var property in typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                apiTableData[property.Name] = property.GetValue(instance);
            }

            DynValue apiTable = DynValue.FromObject(script, apiTableData);
            script.Globals[namespace_] = apiTable;
        }
    }
}
