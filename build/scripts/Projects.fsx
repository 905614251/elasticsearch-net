﻿#I @"../../packages/build/FAKE/tools"
#r @"FakeLib.dll"
#r @"System.IO.Compression.FileSystem.dll"

open System
open System.IO
open System.Diagnostics
open System.Net

open Fake

[<AutoOpen>]
module Projects = 
    type DotNetFrameworkIdentifier = { MSBuild: string; Nuget: string; DefineConstants: string; }

    type DotNetFramework = 
        | NetStandard2_0
        | NetCoreApp2_1
        static member All = [NetStandard2_0] 
        member this.Identifier = 
            match this with
            | NetStandard2_0 -> { MSBuild = "netstandard2.0"; Nuget = "netstandard2.0"; DefineConstants = ""; }
            | NetCoreApp2_1 -> { MSBuild = "netcoreapp2.1"; Nuget = "netcoreapp2.1"; DefineConstants = ""; }

    type Project =
        | Nest
        | ElasticsearchNet
        | NestJsonNetSerializer
        | ElasticsearchNetHttpWebRequestConnection

    type PrivateProject =
        | Tests
        | DocGenerator

    type DotNetProject = 
        | Project of Project
        | PrivateProject of PrivateProject

        static member All = 
            seq [
                Project Project.ElasticsearchNet; 
                Project Project.Nest; 
                Project Project.NestJsonNetSerializer;
                Project ElasticsearchNetHttpWebRequestConnection;
                PrivateProject PrivateProject.Tests
            ]

        static member AllPublishable = 
            seq [
                Project Project.ElasticsearchNet; 
                Project Project.Nest; 
                Project Project.NestJsonNetSerializer;
                Project ElasticsearchNetHttpWebRequestConnection;
            ] 
        static member Tests = seq [PrivateProject PrivateProject.Tests;] 

        member this.Name =
            match this with
            | Project p ->
                match p with
                | Nest -> "Nest"
                | ElasticsearchNet -> "Elasticsearch.Net"
                | NestJsonNetSerializer -> "Nest.JsonNetSerializer"
                | ElasticsearchNetHttpWebRequestConnection -> "Elasticsearch.Net.Connections.HttpWebRequestConnection"
            | PrivateProject p ->
                match p with
                | Tests -> "Tests"
                | DocGenerator -> "DocGenerator"
       
        static member TryFindName (name: string) =
            DotNetProject.All
            |> Seq.map(fun p -> p.Name)
            |> Seq.tryFind(fun p -> p.ToLowerInvariant() = name.ToLowerInvariant())

    type DotNetFrameworkProject = { framework: DotNetFramework; project: DotNetProject }
    let AllPublishableProjectsWithSupportedFrameworks = seq {
        for framework in DotNetFramework.All do
        for project in DotNetProject.AllPublishable do
            yield { framework = framework; project= project}
        }

