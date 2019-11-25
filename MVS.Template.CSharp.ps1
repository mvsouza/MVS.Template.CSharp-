[CmdletBinding()]
param(
    [parameter(Mandatory=$true, Position=0)]
    [ValidateSet("openCover", "lightBddResult",
                 "sonarqubeLocalBuild", "statusSonarqube", "startSonarqubeContainer","stopSonarqubeContainer", "sonarCloudBuild",
                 "installDependencies", 
                 "plantumlSVG",
                 "herokuPush","cfPush",
                 "help")]
    [string]$action
)
process {
    #Requires -Modules Set-PsEnv
    Set-PsEnv
    function SonarqubeStatus($add){
        $status = Invoke-RestMethod "$add/api/system/status";
        return $status.status -eq "UP";
    }
    function RunOpenCover($projectName, $outputDir, $opencoverFile){
        $unitTestProj = Get-ChildItem "$PSScriptRoot\*$_*UnitTest.csproj" -Recurse;
        $dotnet = "C:\Program Files\dotnet\dotnet.exe";
        $filter = @("+[$projectName*]*","-[*UnitTest]*","-[$projectName.Infrastructure]*Program","-[$projectName.Infrastructure]*Startup",
            "-[$projectName.Infrastructure]$projectName.Infrastructure.EF.Migrations.*",
            "-[$projectName.Infrastructure]$projectName.Infrastructure.EF.EventStoreContext",
            "-[$projectName.Infrastructure]$projectName.Infrastructure.EF.Mapping*");
        $opencoverFilter = $filter -join " ";
        $target = "test --logger:trx;LogFileName=$outputDir\results.trx $($unitTestProj.FullName)";
        OpenCover.Console.exe -register:user -target:"$dotnet" -targetargs:"$target" -filter:"$opencoverFilter" -oldStyle -output:"$opencoverFile";
    }

    $tasks = @{};

    $tasks.Add("help", @{
        description="List all actions available.";
        script = {
            "$($MyInvocation.MyCommand.Name) ACTION"
            "Actions parameter"
            (Get-Variable "action").Attributes.ValidValues | %{
                "`t$_`:`n`t`t$($tasks[$_].Description)"
            };
        };
    });

    $tasks.Add("plantumlSVG", @{
        description="Convert all platuml files to SVG.";
        script = {
            ls *.puml -Recurse | %{ plantuml $_.FullName -tsvg } ;
        };
    });
    
    $tasks.Add("cfPush", @{
        description="Push it to the PCF.";
        script = {
            cd src\MVS.Template.CSharp.Infrastructure;
            dotnet publish -c Release -o .\app;
            cd app;
            cf push -f '..\manifest.yml'
            cd ..\..\..
        };
    });

    $tasks.Add("sonarCloudBuild",@{
        description="Runs build and Sonnar Scanner on SonarCloud.";
        script = {
            #Requires -Modules Set-PsEnv
            $projectName = "MVS.Template.CSharp";
            $currentBranch = $(if ($env:APPVEYOR_PULL_REQUEST_NUMBER) {$env:APPVEYOR_PULL_REQUEST_HEAD_REPO_BRANCH ;} else {$env:APPVEYOR_REPO_BRANCH;});
            #$currentBranch = $(if ($currentBranch) {$currentBranch ;} else {$(git rev-parse --abbrev-ref HEAD);});
            
            SonarQube.Scanner.MSBuild.exe begin /k:"$env:sonarcloud_key" /d:sonar.organization="$env:sonarcloud_org" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.login="$env:sonarcloud_login" /d:sonar.cs.opencover.reportsPaths="OpenCover.xml"  /d:sonar.coverage.exclusions="**/Startup.cs,**/Program.cs" /d:sonar.branch.name="$currentBranch";
            dotnet msbuild;
            RunOpenCover $projectName ".\" ".\OpenCover.xml";
            SonarQube.Scanner.MSBuild.exe end /d:sonar.login="$env:sonarcloud_login";
            codecov -f .\OpenCover.xml -t $env:codecov_token;
        }
    });
    $tasks.Add("startSonarqubeContainer",@{
        description="";
        script = {
            docker run -d -p 9000:9000 sonarqube:7.5-developer;
        }
    });
    $tasks.Add("stopSonarqubeContainer",@{
        description="";
        script = {
            docker rm $(docker ps -f ancestor=sonarqube -a -q);
        }
    });
    $tasks.Add("installDependencies",@{
        description="";
        script = {
            if(Get-Command choco -errorAction SilentlyContinue){
                choco install -y dotnetcore-sdk reportgenerator.portable OpenCover msbuild-sonarqube-runner docker-for-windows plantuml;
            } 
            else{
                Write-Output "Install chocolatey first using: Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))";
            }
        }
    });
    $tasks.Add("statusSonarqube",@{
        description="";
        script = {
            Invoke-RestMethod "http://localhost:9000/api/system/status";
        }
    });
    $tasks.Add("sonarqubeLocalBuild",@{
        description="";
        script = {
            $projectName = "MVS.Template.CSharp";
            $sln = Get-ChildItem "$PSScriptRoot\$projectName.sln" -Recurse;
            $outputDir="$($sln.DirectoryName)\CodeCover";
            if(!(Test-Path $outputDir)){
                mkdir $outputDir;
            }
            $opencoverFile="$outputDir\OpenCover.xml";
            $currentBranch = $(git rev-parse --abbrev-ref HEAD);
            SonarQube.Scanner.MSBuild.exe begin /k:"$projectName" /v:"1.0" /n:"$projectName" /d:sonar.cs.opencover.reportsPaths="$opencoverFile" /d:sonar.coverage.exclusions="**/Startup.cs,**/Program.cs" /d:sonar.branch.name="$currentBranch";
            dotnet restore $sln.FullName;
            dotnet msbuild $sln.FullName;
            RunOpenCover $projectName $outputDir $opencoverFile;
            SonarQube.Scanner.MSBuild.exe end;
            [System.Diagnostics.Process]::Start("http://localhost:9000/dashboard?id=$projectName");
        }
    });
    $tasks.Add("openCover",@{
        description="";
        script = {
            $projectName = "MVS.Template.CSharp";
            $sln = Get-ChildItem "$PSScriptRoot\$projectName.sln" -Recurse;
            $outputDir="$($sln.DirectoryName)\CodeCover";
            if(!(Test-Path $outputDir)){
                mkdir $outputDir;
            }
            $opencoverFile="$outputDir\OpenCover.xml";
            RunOpenCover $projectName $outputDir $opencoverFile;
            reportgenerator -reports:"$opencoverFile" -targetdir:"$outputDir";
            & "$outputDir\Index.htm";
        }
    });

    $tasks.Add("lightBddResult",@{
        description="Run all test and show the lightBDD results.";
        script = {
            dotnet test .\test\MVS.Template.CSharp.FunctionalTest\MVS.Template.CSharp.FunctionalTest.csproj;
            ls .\*\FeaturesReport.html -Recurse | %{& $_.FullName};
        }
    });

    $tasks.Add("herokuPush",@{
        description="";
        script = {
            docker-compose build --force-rm;
            docker login --username=_ --password=$env:api_key registry.heroku.com;
            docker tag $env:docker_image_name registry.heroku.com/$env:docker_image_name/web;
            docker push registry.heroku.com/$env:docker_image_name/web;
        }
    });
    $task = $tasks.Get_Item($action)
    if ($task) {
        Invoke-Command $task.script
    }
}
