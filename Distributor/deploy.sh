#!/bin/sh

pushd DistributeSocialLambda
dotnet build
dotnet lambda deploy-function --profile sa-social-distributor DistributeSocialLambda --function-role role-social-distributor
popd
