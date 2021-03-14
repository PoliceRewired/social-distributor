#!/bin/bash

dotnet lambda invoke-function DistributeSocialLambda --profile sa-social-distributor --payload '{ "command": "post", "networks": [ "facebook", "twitter", "discord" ], "message": "'"$1"'" }'
