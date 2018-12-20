#!/usr/bin/env bash

echo "Running pre-commit hook"
dotnet test test/MVS.Template.CSharp.UnitTests/MVS.Template.CSharp.UnitTests.csproj
# $? stores exit value of the last command
if [ $? -ne 0 ]; then
 echo "Tests must pass before commit!"
 exit 1
fi