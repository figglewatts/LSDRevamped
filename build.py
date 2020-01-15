#!/usr/bin/env python3
"""build.py

Used for building Unity projects.

Expects 'storageconf.py' for uploading builds to S3 storage:
region_name: storage region
endpoint_url: storage endpoint
access_key: snip
secret_key: snip
bucket_name: lsdr
"""

import argparse
import json
from os.path import join
import shutil
import subprocess

import boto3
import yaml

DEFAULT_CONFIG_FILE = "buildconf.yaml"
"""The default config file to load in."""

STORAGE_CONFIG_FILE = "storageconf.yaml"
"""The config file containing storage API keys to load in."""

UNITY_ARGS = "-batchmode -nographics -quit -logFile - -executeMethod {} -projectPath {}"
"""The args to give to Unity to build from the command line. Needs -executeMethod
and -projectPath to be substituted in, in that order, via a .format()."""

BUILD_NUMBER_LOCATION = "buildnumber.txt"
"""The file which the build number is stored in."""


def load_config(conf_path: str) -> dict:
    """Load a the build config file from a given path.

    Args:
        conf_path (str): The path to the YAML config file.

    Returns:
        dict: The loaded config file.
    """
    print(f"--> Loading config '{conf_path}'...")
    with open(conf_path, "r") as conf_file:
        return yaml.safe_load(conf_file)


def load_build_defs(file_path: str) -> list:
    """Load the build defs file from a path.

    Args:
        file_path (str): The path to the build defs file.
    
    Returns:
        list: The loaded build definitions.
    """
    print(f"--> Loading build defs from: '{file_path}'...")
    with open(file_path, "r") as build_defs_file:
        return json.load(build_defs_file)


def build(unity_path: str, execute_method: str, project_path: str) -> None:
    """Launch Unity in batchmode to build the player.

    Args:
        unity_path (str): The path to the Unity executable.
        execute_method (str): The method to execute in Unity.
        project_path (str): The path to the project to open.
    """
    unity_args = UNITY_ARGS.format(execute_method, project_path)
    print(f"--> {unity_path} {unity_args}")
    p = subprocess.Popen([unity_path] + unity_args.split(" "),
                         stdout=subprocess.PIPE,
                         stderr=subprocess.STDOUT,
                         shell=True)

    # print the output of the subprocess call realtime
    for line in iter(p.stdout.readline, b""):
        print(f"--> {line.decode('utf-8').rstrip()}")


def upload_build_to_storage(conn, build_number: str, release_path: str,
                            build_target: str, bucket_name: str,
                            prod: bool) -> None:
    """Upload the packaged build to the storage provider.

    Args:
        conn: The storage provider client.
        build_number (str): The build number of the build.
        release_path (str): The path to store releases in.
        build_target (str): The target of the build, i.e. 'StandaloneWindows'.
        bucket_name (str): The storage bucket name to use to upload to.
        prod (bool): Whether to put the release in the prod env folder or not.
    """
    build_package_path = join(release_path, build_target,
                              f"{build_number}.zip")
    env_folder = "prod" if prod else "dev"
    object_name = f"{build_target}/{env_folder}/{build_number}.zip"
    print(
        f"--> Uploading build package '{object_name}' to bucket '{bucket_name}'..."
    )
    conn.upload_file(build_package_path, bucket_name, object_name)

    print(f"--> Uploaded file. Setting to public...")

    # make sure we set the file to be public
    conn.put_object_acl(Bucket=bucket_name, Key=object_name, ACL="public-read")


def package_build_for_release(build_number: str, build_path: str,
                              build_target: str, release_path: str) -> None:
    """Zip up the contents of the build, ready for releasing.

    Args:
        build_number (str): The build number of the build.
        build_path (str): The path to the build.
        build_target (str): The target of the build, i.e. StandaloneWindows.
        release_path (str): The path to store releases in.
    """
    print(
        f"--> Creating build archive for '{build_number}' at '{build_path}'..."
    )
    archive_location = join(release_path, build_target, build_number)
    shutil.make_archive(f"{archive_location}", "zip", build_path)


def get_build_number(build_path: str) -> str:
    """From the build path, read the build number file to get the build number."""
    with open(join(build_path, BUILD_NUMBER_LOCATION),
              "r") as build_number_file:
        return build_number_file.read()


def connect_to_storage(storage_conf: dict):
    """Connect to the storage provider given the storage config.

    Args:
        storage_conf (dict): The storage config file that has been loaded in.

    Returns:
        The client used to perform operations on the storage container.
    """
    session = boto3.session.Session()
    return session.client('s3',
                          region_name=storage_conf["region_name"],
                          endpoint_url=storage_conf["endpoint_url"],
                          aws_access_key_id=storage_conf["access_key"],
                          aws_secret_access_key=storage_conf["secret_key"])


def process_build_defs(build_defs: list, project_path: str, release_path: str,
                       storage_conf: dict, prod: bool) -> None:
    """Go through the build defs file after building to figure out what we need to
    package and upload to storage.
    
    Args:
        build_defs (list): The build definitions loaded from the given JSON file.
        project_path (str): The path to the Unity project, given in the config file.
        release_path (str): The path to store releases.
        storage_conf (dict): The storage access config file containing API keys.
        prod (bool): Whether to release to the prod folder or not.
    """
    conn = connect_to_storage(storage_conf)
    for bd in build_defs:
        build_path = join(project_path, bd["BuildFolder"])
        build_number = get_build_number(build_path)
        build_target = bd["Target"]
        full_release_path = join(project_path, release_path)
        print(
            f"--> Found completed build '{build_target}/{build_number}' from build definition"
        )
        package_build_for_release(build_number, build_path, build_target,
                                  full_release_path)
        upload_build_to_storage(conn, build_number, full_release_path,
                                build_target, storage_conf["bucket_name"],
                                prod)


def parse_args() -> dict:
    """Parse the args of the app."""
    parser = argparse.ArgumentParser(description="Build the game.")
    parser.add_argument("-d",
                        "--dry-run",
                        help="Don't upload the build to storage",
                        action="store_true")
    parser.add_argument(
        "-p",
        "--prod",
        help=
        "Upload this release to the prod folder, releasing it for realsies.",
        action="store_true")
    return parser.parse_args()


def main():
    config = load_config(DEFAULT_CONFIG_FILE)
    unity_path = config["unity_location"]
    execute_method = config["execute_method"]
    project_path = config["project_path"]
    releases_path = config["releases_path"]
    build_defs_path = join(project_path, config["build_defs_path"])
    build_defs = load_build_defs(build_defs_path)
    args = parse_args()

    print("--> Running Unity build...")
    build(unity_path, execute_method, project_path)

    # load storage API keys
    storage_conf = load_config(STORAGE_CONFIG_FILE)

    if not args.dry_run:
        print("--> Processing build definitions to upload files to storage...")
        process_build_defs(build_defs, project_path, releases_path,
                           storage_conf, args.prod)


if __name__ == "__main__":
    main()