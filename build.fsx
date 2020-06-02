#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target 
nuget FSharp.Data 
nuget Fake.DotNet.Testing.Coverlet
nuget Fake.Testing.ReportGenerator //"

#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.DotNet.Testing
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.Testing
open FSharp.Data
open System


let coverageSource = fun (p: Coverlet.CoverletParams) ->
    { p with
        OutputFormat = Coverlet.OutputFormat.OpenCover
        Output = "coverage.xml"
        Include = [
            "MVS.Template.CSharp.*", "*"
        ] 
        Exclude = [
            "*d   Tests?", "*"
            "*", "System.*"
        ]
    }

let coverageOutput = fun (config: Coverlet.CoverletParams) ->
    { config with
        OutputFormat = Coverlet.OutputFormat.OpenCover
        Output = "coverage.xml"
    } |> coverageSource

let coverageThreshold = fun (config: Coverlet.CoverletParams) ->
    { config with
        Threshold = Some 90
    } |> coverageSource

let runCoverage = fun (config: Coverlet.CoverletParams -> Coverlet.CoverletParams) ->
  DotNet.test (fun p ->
      { p with 
          Configuration = DotNet.BuildConfiguration.Release
      }
      |> Coverlet.withDotNetTestOptions config)
      "test/MVS.Template.CSharp.UnitTest"


Target.create "Stats" (fun _ ->
  runCoverage coverageOutput
)

Target.create "TestThreshold" (fun _ ->
  runCoverage coverageThreshold
)

Target.create "BDD" (fun _ ->
    !! "test/**/*.*FunctionalTest.csproj"
    |> Seq.iter (DotNet.test id)
)

Target.create "Libtest" (fun _ ->
    !! "test/**/*.*Test.fsproj"
    |> Seq.iter (DotNet.test id)
)

Target.create "Reports" (fun _ ->
    let local =  Path.getFullName "./"
    let path = sprintf "%sreports/" local
    let parameters p: ReportGenerator.ReportGeneratorParams = 
      { p with TargetDir = path; ToolType = ToolType.CreateLocalTool()}
    !! "**/coverage.xml" |> Seq.toList |> ReportGenerator.generateReports parameters
 )

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    |> Shell.cleanDirs 
)

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "All" ignore

Target.create "Cover" ignore

Target.create "Pipeline" ignore

"TestThreshold"
  ==> "Libtest"
  ==> "BDD"
  ==> "Pipeline"

"Stats"
  ==> "Reports"
  ==> "Cover"

"Clean"
  ==> "Build"
  ==> "All"

Target.runOrDefault "All"
