#!/bin/zsh

pushd DistributeSocialLambda
dotnet build
dotnet lambda deploy-function --profile sa-social-distributor --function-timeout 600 DistributeSocialLambda --function-role role-social-distributor --region eu-west-2
popd
