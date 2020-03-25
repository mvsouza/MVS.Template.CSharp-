alias dnbc='dotnet build --no-incremental --force'
alias cc='rm -r ~/.nuget/packages/mvs.scaffolding; rm -r ../../artifacts'
alias bp='dnbc ../../tools/MVS.Scaffolding/MVS.Scaffolding.csproj && dotnet pack ../../tools/MVS.Scaffolding -o ../../artifacts --version-suffix dev-1 && dotnet restore'