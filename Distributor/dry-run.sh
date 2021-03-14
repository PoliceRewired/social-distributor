#!/bin/sh

dotnet lambda invoke-function DistributeSocialLambda --profile sa-social-distributor --payload '{ "command": "dry-run", "networks": [ "facebook", "twitter", "discord" ], "message": "test message invocation" }'
