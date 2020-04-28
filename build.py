#!/usr/bin/env python3
"""build.py

Used for building Unity projects.

Expects 'storageconf.py' for uploading builds to S3 storage:
region_name: storage region
endpoint_url: storage endpoint
access_key: snip
secret_key: snip
bucket_name: lsdr

Usage:
    - To build the game, run:
        pipenv run python build.py
"""

import argparse
import json
import logging
from os.path import join, exists
import shutil
import subprocess
from typing import List, Tuple

import boto3
from marshmallow import Schema, fields, ValidationError, post_load, validate, validates_schema
import yaml

DEFAULT_CONFIG_FILE = "buildconf.yaml"
"""The default config file to load in."""

STORAGE_CONFIG_FILE = "storageconf.yaml"
"""The config file containing storage API keys to load in."""

UNITY_ARGS = "-batchmode -nographics -quit -logFile - -executeMethod {} -projectPath {}"
"""The args to give to Unity to build from the command line. Needs -executeMethod
and -projectPath to be substituted in, in that order, via a .format()."""

BUILD_NUMBER_LOCATION = "lastbuildnumber.txt"
"""The file which the build number is stored in."""

VALID_BUILD_TARGETS = [
    "StandaloneOSX", "StandaloneWindows", "iOS", "Android",
    "StandaloneWindows64", "WebGL", "WSAPlayer", "StandaloneLinux64", "PS4",
    "XboxOne", "tvOS", "Switch"
]
"""Unity build targets. Source: https://docs.unity3d.com/ScriptReference/BuildTarget.html"""


def validate_path_exists(path: str):
    """Marshmallow validator to ensure a path exists."""
    if not exists(path):
        raise ValidationError(f"Path '{path}' did not exist.'")


class BuildDefSchema(Schema):
    target = fields.Str(data_key="Target",
                        required=True,
                        validate=validate.OneOf(VALID_BUILD_TARGETS))
    executable_name = fields.Str(data_key="ExecutableName", required=True)
    build_folder = fields.Str(data_key="BuildFolder", required=True)

    @post_load
    def make_builddef(self, data, **kwargs):
        return BuildDef(**data)


BUILDDEF_SCHEMA = BuildDefSchema(many=True)


class BuildDef:
    def __init__(self, target: str, executable_name: str,
                 build_folder: str) -> None:
        self.target = target
        self.executable_name = executable_name
        self.build_folder = build_folder


class BuildConfSchema(Schema):
    unity_location = fields.String(required=True,
                                   validate=validate_path_exists)
    execute_method = fields.String(required=True)
    project_path = fields.String(required=True, validate=validate_path_exists)
    build_defs_path = fields.String(required=True)
    releases_path = fields.String(required=True)

    @validates_schema
    def validate_project_paths(self, data, **kwargs):
        validate_path_exists(
            join(data["project_path"], data["build_defs_path"]))

    @post_load
    def make_buildconf(self, data, **kwargs):
        return BuildConf(**data)


BUILDCONF_SCHEMA = BuildConfSchema()


class BuildConf:
    def __init__(self, unity_location: str, execute_method: str,
                 project_path: str, build_defs_path: str,
                 releases_path: str) -> None:
        self.unity_location = unity_location
        self.execute_method = execute_method
        self.project_path = project_path
        self.build_defs_path = build_defs_path
        self.releases_path = releases_path


class StorageConfSchema(Schema):
    region_name = fields.Str(required=True)
    endpoint_url = fields.Url(required=True)
    access_key = fields.Str(required=True)
    secret_key = fields.Str(required=True)
    bucket_name = fields.Str(required=True)

    @post_load
    def make_storageconf(self, data, **kwargs):
        return StorageConf(**data)


STORAGECONF_SCHEMA = StorageConfSchema()


class StorageConf:
    def __init__(self, region_name: str, endpoint_url: str, access_key: str,
                 secret_key: str, bucket_name: str) -> None:
        self.region_name = region_name
        self.endpoint_url = endpoint_url
        self.access_key = access_key
        self.secret_key = secret_key
        self.bucket_name = bucket_name


class ArgsSchema(Schema):
    release = fields.Bool(default=False)
    prod = fields.Bool(default=False)

    @post_load
    def make_args(self, data, **kwargs):
        return Args(**data)


ARGS_SCHEMA = ArgsSchema()


class Args:
    def __init__(self, release: bool, prod: bool) -> None:
        self.release = release
        self.prod = prod


def load_build_config(conf_path: str) -> BuildConf:
    """Load a build config file from a given path.

    Args:
        conf_path (str): The path to the YAML config file.

    Returns:
        BuildConf: The loaded config file, or None if there was an error.
    """
    print(f"--> Loading config '{conf_path}'...")
    try:
        with open(conf_path, "r") as conf_file:
            return BUILDCONF_SCHEMA.load(yaml.safe_load(conf_file))
    except FileNotFoundError as err:
        logging.error(err)
        return None
    except ValidationError as err:
        logging.error(f"Invalid config file: {err.messages}")
        return None


def load_storage_config(conf_path: str) -> StorageConf:
    """Load a storage config file from a given path.

    Args:
        conf_path (str): The path to the YAML config file.

    Returns:
        StorageConf: The loaded config file, or None if there was an error.
    """
    print(f"--> Loading config '{conf_path}'...")
    try:
        with open(conf_path, "r") as conf_file:
            return STORAGECONF_SCHEMA.load(yaml.safe_load(conf_file))
    except FileNotFoundError as err:
        logging.error(err)
        return None
    except ValidationError as err:
        logging.error(f"Invalid config file: {err.messages}")
        return None


def load_build_defs(file_path: str) -> List[BuildDef]:
    """Load the build defs file from a path.

    Args:
        file_path (str): The path to the build defs file.
    
    Returns:
        List[BuildDef]: The loaded build definitions, or None if there was an error.
    """
    print(f"--> Loading build defs from: '{file_path}'...")
    try:
        with open(file_path, "r") as build_defs_file:
            return BUILDDEF_SCHEMA.load(json.load(build_defs_file))
    except FileNotFoundError as err:
        logging.error(err)
        return None
    except ValidationError as err:
        logging.error(f"Invalid build definition: {err.messages}")
        return None


def build(unity_path: str, execute_method: str,
          project_path: str) -> Tuple[bool, int]:
    """Launch Unity in batchmode to build the player.

    Args:
        unity_path (str): The path to the Unity executable.
        execute_method (str): The method to execute in Unity.
        project_path (str): The path to the project to open.

    Returns:
        bool, int: Indicating whether the build ran successfully or not, and the exit code.
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

    # wait for it to exit before returning
    p.wait()
    return p.returncode == 0, p.returncode


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
    object_name = f"{env_folder}/{build_target}/{build_number}.zip"
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
        f"--> Creating build archive for {build_target} '{build_number}' at '{build_path}'..."
    )
    archive_location = join(release_path, build_target, build_number)
    shutil.make_archive(f"{archive_location}", "zip", build_path)


def get_build_number(project_path: str) -> str:
    """From the project path, read the build number file to get the last build number."""
    with open(join(project_path, BUILD_NUMBER_LOCATION),
              "r") as build_number_file:
        return build_number_file.read()


def connect_to_storage(storage_conf: StorageConf):
    """Connect to the storage provider given the storage config.

    Args:
        storage_conf (StorageConf): The storage config file that has been loaded in.

    Returns:
        The client used to perform operations on the storage container.
    """
    session = boto3.session.Session()
    return session.client('s3',
                          region_name=storage_conf.region_name,
                          endpoint_url=storage_conf.endpoint_url,
                          aws_access_key_id=storage_conf.access_key,
                          aws_secret_access_key=storage_conf.secret_key)


def release_builds(build_defs: List[BuildDef], project_path: str,
                   release_path: str, build_number: str,
                   storage_conf: StorageConf, prod: bool) -> None:
    """Go through the build defs file after building to figure out what we need to
    package and upload to storage.
    
    Args:
        build_defs (List[BuildDef]): The build definitions loaded from the given JSON file.
        project_path (str): The path to the Unity project, given in the config file.
        release_path (str): The path to store releases.
        build_number (str): The build number to process build defs for.
        storage_conf (StorageConf): The storage access config file containing API keys.
        prod (bool): Whether to release to the prod folder or not.
    """
    conn = connect_to_storage(storage_conf)
    for bd in build_defs:
        build_path = join(project_path, bd.build_folder, build_number)
        full_release_path = join(project_path, release_path)
        print(
            f"--> Found completed build '{bd.build_target}/{build_number}' from build definition"
        )
        package_build_for_release(build_number, build_path, bd.build_target,
                                  full_release_path)
        upload_build_to_storage(conn, build_number, full_release_path,
                                bd.build_target, storage_conf.bucket_name,
                                prod)


def parse_args() -> Args:
    """Parse the args of the app.
    
    Returns:
        Args: The args.
    """
    parser = argparse.ArgumentParser(description="Build the game.")
    parser.add_argument(
        "-r",
        "--release",
        help="Release (upload) the build to the storage provider.",
        action="store_true")
    parser.add_argument(
        "-p",
        "--prod",
        help=
        "Upload this release to the prod folder, releasing it for realsies.",
        action="store_true")
    return ARGS_SCHEMA.load(parser.parse_args().__dict__)


def main():
    args = parse_args()

    config = load_build_config(DEFAULT_CONFIG_FILE)
    if config is None:
        raise SystemExit(1)

    # load storage API keys
    storage_conf = load_storage_config(STORAGE_CONFIG_FILE)
    if storage_conf is None:
        raise SystemExit(1)

    build_defs = load_build_defs(
        join(config.project_path, config.build_defs_path))
    if build_defs is None:
        raise SystemExit(1)

    print("--> Running Unity build...")
    success, exit_code = build(config.unity_location, config.execute_method,
                               config.project_path)
    if not success:
        logging.error("Build failure")
        raise SystemExit(exit_code)

    if args.release or args.prod:
        print("--> Processing build definitions to upload files to storage...")
        build_number = get_build_number(config.project_path)
        release_builds(build_defs, config.project_path, config.releases_path,
                       build_number, storage_conf, args.prod)


if __name__ == "__main__":
    main()