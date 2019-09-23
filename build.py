#!/usr/bin/env python3
"""build.py

Used for building Unity projects.
"""

from os.path import join
import subprocess

import yaml

DEFAULT_CONFIG_FILE = "buildconf.yaml"
"""The default config file to load in."""

# "C:/Program Files/Unity/Hub/Editor/2017.4.31f1/Editor/Unity.exe" -batchmode -nographics -quit -projectPath "C:/Users/figgl/Documents/git-repos/LSDRevamped/LSDR" -executeMethod LSDR.Editor.BuildSystem.BuildScript.Build
UNITY_COMMAND = "-batchmode -nographics -quit -logFile - -executeMethod {} -projectPath {}"


def load_config(conf_path: str) -> dict:
    """Load a the build config file from a given path.

    Args:
        conf_path (str): The path to the YAML config file.

    Returns:
        dict: The loaded config file.
    """
    print(f"Loading config '{conf_path}'...")
    with open(conf_path, "r") as conf_file:
        return yaml.safe_load(conf_file)


def build(unity_path: str, execute_method: str, project_path: str) -> None:
    """Launch Unity in batchmode to build the player.

    Args:
        unity_path (str): The path to the Unity executable.
        execute_method (str): The method to execute in Unity.
        project_path (str): The path to the project to open.
    """
    unity_args = UNITY_COMMAND.format(execute_method, project_path)
    print(f"--> {unity_path} {unity_args}")
    p = subprocess.Popen([unity_path] + unity_args.split(" "),
                         stdout=subprocess.PIPE,
                         stderr=subprocess.STDOUT,
                         shell=True)
    for line in iter(p.stdout.readline, b""):
        print(f"--> {line.decode('utf-8').rstrip()}")


def main():
    config = load_config(DEFAULT_CONFIG_FILE)
    unity_path = config["unity_location"]
    execute_method = config["execute_method"]
    project_path = config["project_path"]

    print("Running Unity build...")
    build(unity_path, execute_method, project_path)


if __name__ == "__main__":
    main()