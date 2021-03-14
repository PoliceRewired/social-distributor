#!/bin/sh

pushd DistributeSocialLambda
dotnet build
dotnet tool install -g Amazon.Lambda.Tools
dotnet tool update -g Amazon.Lambda.Tools
dotnet lambda deploy-function --profile sa-social-distributor DistributeSocialLambda --function-role role-social-distributor
popd
