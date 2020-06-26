alias dn=dotnet
alias dnb='dotnet build'
alias dnt='dotnet test'
alias dnbc='dotnet build --no-incremental --force'

alias dnc='dotnet aspnet-codegenerator -p .'
_dotnet_zsh_complete()
{
  local completions=("$(dotnet complete "$words")")

  reply=( "${(ps:\n:)completions}" )
}
compctl -K _dotnet_zsh_complete dotnet