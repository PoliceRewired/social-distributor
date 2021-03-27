#!/bin/zsh

PAYLOAD=$(cat << 'EOF'
{ 
    "command": "recalculate"
}
EOF
)

echo $PAYLOAD

dotnet lambda invoke-function DistributeSocialLambda \
  --region eu-west-2 \
  --profile sa-social-distributor \
  --payload $PAYLOAD
