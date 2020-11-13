using System;
using System.Collections.Generic;
using System.Reflection;
using LSDR.Lua.Actions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using Torii.Util;
using UnityEngine;

namespace LSDR.Lua
{
    public static class LuaEngine
    {
        private static readonly Dictionary<string, object> _registeredObjects;

        static LuaEngine()
        {
            _registeredObjects = new Dictionary<string, object>();

            // register types
            UserData.RegisterAssembly();
            UserData.RegisterType<LuaAsyncActionRunner>();
            UserData.RegisterType<LuaAsyncAction>();

            // create converters
            createConverters();
        }

        public static Script CreateBaseAPI()
        {
            Script script = new Script(CoreModules.Preset_SoftSandbox)
            {
                Options =
                {
                    ScriptLoader = new FileSystemScriptLoader(),
                    DebugPrint = Debug.Log
                }
            };

            createStaticAPI<UnityAPI>(script);
            createStaticAPI<MiscAPI>(script);
            createStaticAPI<LSDAPI>(script);
            LSDAPI.Register();
            createNamespacedStaticAPI<ActionPredicates>(script, "Condition");
            createNamespacedStaticAPI<ResourceAPI>(script, "Resources");
            createNamespacedStaticAPI<ColorAPI>(script, "Color");

            createRegisteredGlobalObjects(script);

            return script;
        }

        public static void LoadScript(string scriptPath, Script script)
        {
            try
            {
                script.DoFile(PathUtil.Combine(Application.streamingAssetsPath, scriptPath));
            }
            catch (InternalErrorException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (SyntaxErrorException e)
            {
                Console.WriteLine($"Lua Syntax Error: {e.DecoratedMessage}");
            }
            catch (ScriptRuntimeException e)
            {
                Console.WriteLine($"Lua Script Error: {e.DecoratedMessage}");
            }
        }

        public static void RegisterGlobalObject(object obj, string alias = "")
        {
            var objName = string.IsNullOrEmpty(alias)
                ? obj.GetType().Name
                : alias;
            _registeredObjects[objName] = obj;
        }

        private static void createRegisteredGlobalObjects(Script script)
        {
            foreach (var registeredObj in _registeredObjects)
            {
                var objName = registeredObj.Key;
                var obj = registeredObj.Value;
                script.Globals[objName] = obj;
            }
        }

        private static void createConverters()
        {
            // Action
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Function, typeof(Action),
                value =>
                {
                    var func = value.Function;
                    return new Action(() => func.Call());
                }
            );

            // Func<bool>
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Function,
                typeof(Func<bool>),
                value =>
                {
                    var func = value.Function;
                    return new Func<bool>(() => func.Call().Boolean);
                }
            );

            // Vector3
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Vector3),
                dynVal =>
                {
                    Table table = dynVal.Table;
                    float x = (float)((Double)table[1]);
                    float y = (float)((Double)table[2]);
                    float z = (float)((Double)table[3]);
                    return new Vector3(x, y, z);
                }
            );
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Vector3>(
                (script, vector) =>
                {
                    DynValue x = DynValue.NewNumber((double)vector.x);
                    DynValue y = DynValue.NewNumber((double)vector.y);
                    DynValue z = DynValue.NewNumber((double)vector.z);
                    DynValue dynVal = DynValue.NewTable(script, new DynValue[] {x, y, z});
                    dynVal.Table.MetaTable = DynValue.FromObject(script, new Dictionary<string, object>
                    {
                        {
                            "__tostring", new Func<Table, string>(t => $"({x}, {y}, {z})")
                        },
                        {
                            "__add", new Func<Table, Table, Table>((a, b) =>
                            {
                                DynValue rX = DynValue.NewNumber(a.Get(1).Number + b.Get(1).Number);
                                DynValue rY = DynValue.NewNumber(a.Get(2).Number + b.Get(2).Number);
                                DynValue rZ = DynValue.NewNumber(a.Get(3).Number + b.Get(3).Number);
                                return new Table(script, rX, rY, rZ);
                            })
                        },
                        {
                            "__sub", new Func<Table, Table, Table>((a, b) =>
                            {
                                DynValue rX = DynValue.NewNumber(a.Get(1).Number - b.Get(1).Number);
                                DynValue rY = DynValue.NewNumber(a.Get(2).Number - b.Get(2).Number);
                                DynValue rZ = DynValue.NewNumber(a.Get(3).Number - b.Get(3).Number);
                                return new Table(script, rX, rY, rZ);
                            })
                        },
                        {
                            "__mul", new Func<Table, double, Table>((v, s) =>
                            {
                                DynValue rX = DynValue.NewNumber(v.Get(1).Number * s);
                                DynValue rY = DynValue.NewNumber(v.Get(2).Number * s);
                                DynValue rZ = DynValue.NewNumber(v.Get(3).Number * s);
                                return new Table(script, rX, rY, rZ);
                            })
                        },
                        {
                            "__div", new Func<Table, double, Table>((v, s) =>
                            {
                                DynValue rX = DynValue.NewNumber(v.Get(1).Number / s);
                                DynValue rY = DynValue.NewNumber(v.Get(2).Number / s);
                                DynValue rZ = DynValue.NewNumber(v.Get(3).Number / s);
                                return new Table(script, rX, rY, rZ);
                            })
                        }
                    }).Table;
                    return dynVal;
                }
            );

            // color
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Color),
                dynVal =>
                {
                    Table table = dynVal.Table;
                    float r = (float)((Double)table[1]);
                    float g = (float)((Double)table[2]);
                    float b = (float)((Double)table[3]);
                    float a = (float)((Double)table[4]);
                    return new Color(r, g, b, a);
                }
            );
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Color>(
                (script, color) =>
                {
                    DynValue r = DynValue.NewNumber((double)color.r);
                    DynValue g = DynValue.NewNumber((double)color.g);
                    DynValue b = DynValue.NewNumber((double)color.b);
                    DynValue a = DynValue.NewNumber((double)color.a);
                    DynValue dynVal = DynValue.NewTable(script, new DynValue[] {r, g, b, a});
                    dynVal.Table.MetaTable = DynValue.FromObject(script, new Dictionary<string, object>
                    {
                        {
                            "__tostring", new Func<Table, string>(t => $"(R: {r}, G: {g}, B: {b}, A: {a})")
                        },
                    }).Table;
                    return dynVal;
                }
            );
        }

        private static void createStaticAPI<T>(Script script)
        {
            MethodInfo[] api = typeof(T).GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var method in api)
            {
                script.Globals[method.Name] = method;
            }
        }

        private static void createNamespacedStaticAPI<T>(Script script, string namespace_)
        {
            Dictionary<string, MethodInfo> apiTableData = new Dictionary<string, MethodInfo>();
            MethodInfo[] api = typeof(T).GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var method in api)
            {
                apiTableData[method.Name] = method;
            }

            DynValue apiTable = DynValue.FromObject(script, apiTableData);
            script.Globals[namespace_] = apiTable;
        }
    }
}
