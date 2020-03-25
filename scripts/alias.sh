alias dn=dotnet
alias dnb='dotnet build'
alias dnt='dotnet test'
alias dnbc='dotnet build --no-incremental --force'

alias dnc='dotnet aspnet-codegenerator -p .'
alias cc='rm -r ~/.nuget/packages/mvs.scaffolding; rm -r ../../artifacts'
alias bp='dnbc ../../tools/MVS.Scaffolding/MVS.Scaffolding.csproj && dotnet pack ../../tools/MVS.Scaffolding -o ../../artifacts --version-suffix dev-1 && dotnet restore'