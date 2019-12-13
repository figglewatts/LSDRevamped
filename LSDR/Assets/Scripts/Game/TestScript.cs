using System;
using LSDR.Dream;
using Torii.Console;
using Torii.Serialization;
using UnityEngine;

namespace LSDR.Game
{
    public class TestScript : MonoBehaviour
    {
        public DevConsoleSystem Console;

        [Console]
        public string TestProperty { get; set; }

        [Console]
        public int TestVar;

        public bool SubtractiveFog;

        public void Start()
        {
            Console.Init();
            Console.Register(this);
            
            executeStatement("TestScript.PrintTest Hello world!");
            executeStatement("TestScript.TestVar 20");
            executeStatement("TestScript.TestProperty Testing");

            foreach (var v in Console.Completions("Tes"))
            {
                Debug.Log($"Obj: {v}");
            }

            foreach (var v in Console.Completions("TestScript", "Te"))
            {
                Debug.Log($"Spec: {v}");
            }
            
            foreach (var v in Console.Completions(""))
            {
                Debug.Log($"All: {v}");
            }
        }

        public void Update()
        {
            Shader.SetGlobalInt("_SubtractiveFog", SubtractiveFog ? 1 : 0);
        }

        [Console]
        public void PrintTest(string arg)
        {
            Debug.Log(arg);
        }

        private void executeStatement(string statement)
        {
            var result = Console.Execute(statement);
            if (result.Failed)
            {
                Debug.LogException(result.Error);
            }
            else if (!String.IsNullOrEmpty(result.Message))
            {
                Debug.Log(result.Message);
            }
        }
    }
}