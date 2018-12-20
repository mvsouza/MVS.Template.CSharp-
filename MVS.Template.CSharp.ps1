[CmdletBinding()]
param(
    [parameter(Mandatory=$true, Position=1)]
    [ValidateSet("openCover", "sonarqubeBuild", "statusSonarqube", "startSonarqube","stopSonarqube", "installDependencies", "sonarCloudBuild")]
    [string]$action,
    [parameter(Mandatory=$false)]
    [switch]$all
)
process {
    #Requires -Modules Set-PsEnv
    Set-PsEnv

    $tasks = @{};

    $tasks.Add("sonarCloudBuild",@{
        description="Runs build and Sonnar Scanner on SonarCloud.";
        script = {
            #Requires -Modules Set-PsEnv
            SonarQube.Scanner.MSBuild.exe begin /k:"$env:sonarcloud_key" /d:sonar.organization="$env:sonarcloud_org" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.login="$env:sonarcloud_login" /d:sonar.cs.opencover.reportsPaths="OpenCover.xml";
            dotnet msbuild;
            OpenCover.Console.exe -register:user -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:"test --logger:trx;LogFileName=results.trx /p:DebugType=full test\MVS.Template.CSharp.UnitTest\MVS.Template.CSharp.UnitTest.csproj" -filter:"+[MVS.Template.CSharp*]* -[*.Test*]*" -oldStyle -output:"OpenCover.xml";
            SonarQube.Scanner.MSBuild.exe end /d:sonar.login="$env:sonarcloud_login";
            codecov -f .\OpenCover.xml -t $env:codecov_token;
        }
    });
    
    $task = $tasks.Get_Item($action)
    if ($task) {
        Invoke-Command $task.script
    }

    function SonarqubeStatus($add){
        $status = Invoke-RestMethod "$add/api/system/status";
        return $status.status -eq "UP";
    }
    $unitTestProj = Get-ChildItem "$PSScriptRoot\*$_*UnitTest.csproj" -Recurse;
    $projectName = "MVS.Template.CSharp";
    $sln = Get-ChildItem "$PSScriptRoot\$projectName.sln" -Recurse;

    $outputDir="$($sln.DirectoryName)\CodeCover";
    if(!(Get-ChildItem $outputDir -ErrorAction SilentlyContinue)){
        mkdir $outputDir;
    }
    $opencoverFile="$outputDir\OpenCover.xml";
    if($action -eq "sonarqubeLocalBuild"){
        SonarQube.Scanner.MSBuild.exe begin /k:"$projectName" /v:"1.0" /n:"$projectName" /d:sonar.cs.opencover.reportsPaths="$opencoverFile";
        dotnet restore $sln.FullName;
        dotnet msbuild $sln.FullName;
    }
    if($action -eq "openCover" -or $action -eq "sonarqubeLocalBuild" ){
        $dotnet = "C:\Program Files\dotnet\dotnet.exe";
        $opencoverFilter = "+[$projectName*]* -[*UnitTest]*";
        $target = "test --logger:trx;LogFileName=$outputDir\results.trx $($unitTestProj.FullName)";
        OpenCover.Console.exe -register:user -target:"$dotnet" -targetargs:"$target" -filter:"$opencoverFilter" -oldStyle -output:"$opencoverFile";
    }
    if($action -eq "sonarqubeLocalBuild"){
        SonarQube.Scanner.MSBuild.exe end;
        [System.Diagnostics.Process]::Start("http://localhost:9000/dashboard?id=$projectName");
    }
    if($action -eq "openCover"){
        reportgenerator -reports:"$opencoverFile" -targetdir:"$outputDir";
        & "$outputDir\Index.htm";
    }
    if($action -eq "startSonarqubeContainer"){
        docker run -d -p 9000:9000 sonarqube;
    }
    if($action -eq "stopSonarqubeContainer"){
        docker rm $(docker ps -f ancestor=sonarqube -a -q);
    }
    if($action -eq "installDependencies"){
        if(Get-Command $cmdName -errorAction SilentlyContinue){
            choco install -y reportgenerator.portable OpenCover msbuild-sonarqube-runner docker-for-windows;
        } 
        else{
            Write-Output "Install chocolatey first using: Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))";
        }
    }
    if($action -eq "statusSonarqube"){
        Invoke-RestMethod "http://localhost:9000/api/system/status";
    }
}
