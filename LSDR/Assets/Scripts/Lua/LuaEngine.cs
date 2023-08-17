using System;
using System.Collections.Generic;
using System.Reflection;
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

        public LuaEngine()
        {
            _registeredObjects = new Dictionary<string, object>();

            // register types
            UserData.RegisterAssembly();
            UserData.RegisterType<LuaAsyncActionRunner>();
            UserData.RegisterType<LuaAsyncAction>();

            // create converters
            createConverters();
        }

        public Script CreateBaseAPI()
        {
            Script script = new Script(CoreModules.Preset_SoftSandbox)
            {
                Options =
                {
                    ScriptLoader = new FileSystemScriptLoader(),
                    DebugPrint = Debug.Log
                }
            };

            createStaticAPI(new UnityAPI(), script);
            createStaticAPI(new MiscAPI(), script);
            createStaticAPI(new LSDAPI(), script);
            createNamespacedStaticAPI(new ActionPredicates(), script, "Condition");
            createNamespacedStaticAPI(new ColorAPI(), script, "Color");

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

            // Vector3
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(DataType.Table, typeof(Vector3),
                dynVal =>
                {
                    Table table = dynVal.Table;
                    float x = (float)(double)table[key: 1];
                    float y = (float)(double)table[key: 2];
                    float z = (float)(double)table[key: 3];
                    return new Vector3(x, y, z);
                }
            );
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion<Vector3>(
                (script, vector) =>
                {
                    DynValue x = DynValue.NewNumber(vector.x);
                    DynValue y = DynValue.NewNumber(vector.y);
                    DynValue z = DynValue.NewNumber(vector.z);
                    DynValue dynVal = DynValue.NewTable(script, x, y, z);
                    dynVal.Table.MetaTable = DynValue.FromObject(script, new Dictionary<string, object>
                    {
                        {
                            "__tostring", new Func<Table, string>(t => $"({x}, {y}, {z})")
                        },
                        {
                            "__add", new Func<Table, Table, Table>((a, b) =>
                            {
                                DynValue rX = DynValue.NewNumber(a.Get(key: 1).Number + b.Get(key: 1).Number);
                                DynValue rY = DynValue.NewNumber(a.Get(key: 2).Number + b.Get(key: 2).Number);
                                DynValue rZ = DynValue.NewNumber(a.Get(key: 3).Number + b.Get(key: 3).Number);
                                return new Table(script, rX, rY, rZ);
                            })
                        },
                        {
                            "__sub", new Func<Table, Table, Table>((a, b) =>
                            {
                                DynValue rX = DynValue.NewNumber(a.Get(key: 1).Number - b.Get(key: 1).Number);
                                DynValue rY = DynValue.NewNumber(a.Get(key: 2).Number - b.Get(key: 2).Number);
                                DynValue rZ = DynValue.NewNumber(a.Get(key: 3).Number - b.Get(key: 3).Number);
                                return new Table(script, rX, rY, rZ);
                            })
                        },
                        {
                            "__mul", new Func<Table, double, Table>((v, s) =>
                            {
                                DynValue rX = DynValue.NewNumber(v.Get(key: 1).Number * s);
                                DynValue rY = DynValue.NewNumber(v.Get(key: 2).Number * s);
                                DynValue rZ = DynValue.NewNumber(v.Get(key: 3).Number * s);
                                return new Table(script, rX, rY, rZ);
                            })
                        },
                        {
                            "__div", new Func<Table, double, Table>((v, s) =>
                            {
                                DynValue rX = DynValue.NewNumber(v.Get(key: 1).Number / s);
                                DynValue rY = DynValue.NewNumber(v.Get(key: 2).Number / s);
                                DynValue rZ = DynValue.NewNumber(v.Get(key: 3).Number / s);
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
                    float r = (float)(double)table[key: 1];
                    float g = (float)(double)table[key: 2];
                    float b = (float)(double)table[key: 3];
                    float a = (float)(double)table[key: 4];
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
                    DynValue dynVal = DynValue.NewTable(script, r, g, b, a);
                    dynVal.Table.MetaTable = DynValue.FromObject(script, new Dictionary<string, object>
                    {
                        {
                            "__tostring", new Func<Table, string>(t => $"(R: {r}, G: {g}, B: {b}, A: {a})")
                        }
                    }).Table;
                    return dynVal;
                }
            );
        }

        private void createStaticAPI<T>(T instance, Script script) where T : ILuaAPI
        {
            Type t = typeof(T);

            instance.Register(this);

            MethodInfo[] api = t.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo method in api) script.Globals[method.Name] = method;
        }

        private void createNamespacedStaticAPI<T>(T instance, Script script, string namespace_) where T : ILuaAPI
        {
            var apiTableData = new Dictionary<string, MethodInfo>();

            Type t = typeof(T);

            instance.Register(this);

            MethodInfo[] api = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in api) apiTableData[method.Name] = method;

            DynValue apiTable = DynValue.FromObject(script, apiTableData);
            script.Globals[namespace_] = apiTable;
        }
    }
}
